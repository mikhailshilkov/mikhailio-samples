using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.ServiceBus;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.Fluent;

namespace MyDurableTaskApp
{
    public static class DurableScaling
    {
        private const int ThresholdUp = 5000;
        private const int ThresholdDown = 1000;
        private static readonly TimeSpan Period = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan CooldownPeriod = TimeSpan.FromMinutes(10);

        [FunctionName(nameof(MetricCollector))]
        public static async Task MetricCollector(
            [TimerTrigger("0 */1 * * * *")] TimerInfo myTimer,
            [OrchestrationClient] DurableOrchestrationClient client,
            TraceWriter log)
        {
            log.Info($"MetricCollector function executed at: {DateTime.Now}");
            var resource = Environment.GetEnvironmentVariable("ResourceToScale");

            //await client.TerminateAsync(resource, "");
            var status = await client.GetStatusAsync(resource);
            if (status == null)
            {
                await client.StartNewAsync(nameof(ScalingLogic), resource, new ScalingState());
            }
            else
            {
                var metric = ServiceBusHelper.GetSubscriptionMetric(resource);
                log.Info($"Collector: Current metric value is {metric.Value.Value} at {DateTime.Now}");
                await client.RaiseEventAsync(resource, nameof(Metric), metric);
            }
        }

        [FunctionName(nameof(ScalingLogic))]
        public static async Task<ScalingState> ScalingLogic([OrchestrationTrigger] DurableOrchestrationContext context, TraceWriter log)
        {
            var state = context.GetInput<ScalingState>();
            log.Info($"Current state is {state}. Waiting for next value.");

            var metric = await context.WaitForExternalEvent<Metric>(nameof(Metric));
            var history = state.History;
            log.Info($"Scaling logic: Received {metric.Name}, previous state is {string.Join(", ", history)}");

            // 2. Add current metric value, remove old values
            history.Add(metric.Value);
            history.RemoveAll(e => e.Time < metric.Value.Time.Subtract(Period));

            // 3. Compare the aggregates to thresholds, produce scaling action if needed
            ScaleAction action = null;
            if (history.Count >= 5
                && context.CurrentUtcDateTime - state.LastScalingActionTime > CooldownPeriod)
            {
                var average = (int)history.Average(e => e.Value);
                var maximum = history.Max(e => e.Value);
                if (average > ThresholdUp)
                {
                    log.Info($"Scaling logic: Value {average} is too high, scaling {metric.ResourceName} up...");
                    action = new ScaleAction(metric.ResourceName, ScaleActionType.Up);
                }
                else if (maximum < ThresholdDown)
                {
                    log.Info($"Scaling logic: Value {maximum} is low, scaling {metric.ResourceName} down...");
                    action = new ScaleAction(metric.ResourceName, ScaleActionType.Down);
                }
            }

            // 4. If scaling is needed, call Scaler
            if (action != null)
            {
                var result = await context.CallFunctionAsync<int>(nameof(Scaler), action);
                log.Info($"Scaling logic: Scaled to {result} instances.");
                state.LastScalingActionTime = context.CurrentUtcDateTime;
            }

            context.ContinueAsNew(state);
            return state;
        }

        [FunctionName(nameof(Scaler))]
        public static int Scaler([ActivityTrigger] DurableActivityContext context, TraceWriter log)
        {
            log.Info($"Scaler executed at: {DateTime.Now}");

            var action = context.GetInput<ScaleAction>();

            var newCapacity = ScalingHelper.ChangeAppServiceInstanceCount(
                action.ResourceName,
                action.Type == ScaleActionType.Down ? -1 : +1);

            return newCapacity;
        }
    }

    public class ScalingState
    {
        public List<MetricValue> History { get; } = new List<MetricValue>();

        public DateTime LastScalingActionTime { get; set; } = DateTime.MinValue;
    }

    public class Metric
    {
        public Metric(string resourceName, string name, MetricValue value)
        {
            this.ResourceName = resourceName;
            this.Name = name;
            this.Value = value;
        }

        public string ResourceName { get; }

        public string Name { get; }

        public MetricValue Value { get; }
    }

    public class MetricValue
    {
        public MetricValue(DateTime time, int value)
        {
            this.Time = time;
            this.Value = value;
        }

        public DateTime Time { get; }

        public int Value { get; }

        public override string ToString()
        {
            return $"{this.Time.ToShortTimeString()}: {this.Value}";
        }
    }

    public class ScaleAction
    {
        public ScaleAction(string resourceName, ScaleActionType type)
        {
            this.ResourceName = resourceName;
            this.Type = type;
        }

        public string ResourceName { get; }

        public ScaleActionType Type { get; }
    }

    public enum ScaleActionType
    {
        Up,
        Down
    }

    public static class ServiceBusHelper
    {
        public static Metric GetSubscriptionMetric(string resource)
        {
            var topic = Environment.GetEnvironmentVariable("Topic");
            var subscription = Environment.GetEnvironmentVariable("Subscription");
            var backlog = ServiceBusHelper.GetSubscriptionBacklog(topic, subscription);

            var value = new MetricValue(DateTime.UtcNow, (int)backlog);
            return new Metric(resource, $"{topic}-{subscription}-backlog", value);
        }

        public static long GetSubscriptionBacklog(string topic, string subscription)
        {
            var connectionString = Environment.GetEnvironmentVariable("ServiceBusConnection");

            var nsmgr = NamespaceManager.CreateFromConnectionString(connectionString);
            var subscriptionClient = nsmgr.GetSubscription(topic, subscription);
            return subscriptionClient.MessageCountDetails.ActiveMessageCount;
        }
    }

    public static class ScalingHelper
    {
        public static int ChangeAppServiceInstanceCount(string resourceName, int delta)
        {
            var secrets = Environment.GetEnvironmentVariable("ServicePrincipal").Split(',');
            var credentials = SdkContext.AzureCredentialsFactory
                .FromServicePrincipal(secrets[0], secrets[1], secrets[2], AzureEnvironment.AzureGlobalCloud);
            var azure = Azure.Configure()
                .Authenticate(credentials)
                .WithDefaultSubscription();

            var plan = azure.AppServices
                .AppServicePlans
                .List()
                .First(p => p.Name.Contains(resourceName));

            var newCapacity = plan.Capacity + delta;

            plan.Update()
                .WithCapacity(newCapacity)
                .Apply();

            return newCapacity;
        }
    }
}