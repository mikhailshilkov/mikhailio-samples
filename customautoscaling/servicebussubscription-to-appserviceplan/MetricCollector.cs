using System;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.ServiceBus;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.Fluent;

namespace AutoScaleFunctionApp
{
    public static class AutoScalingFunctions
    {
        [FunctionName("MetricCollector")]
        [return: Queue("Metrics")]
        public static Metric MetricCollector([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, TraceWriter log)
        {
            var connectionString = Environment.GetEnvironmentVariable("ServiceBusConnection");
            var topic = Environment.GetEnvironmentVariable("Topic");
            var subscription = Environment.GetEnvironmentVariable("Subscription");

            var nsmgr = NamespaceManager.CreateFromConnectionString(connectionString);
            var subscriptionClient = nsmgr.GetSubscription(topic, subscription);
            var backlog = subscriptionClient.MessageCountDetails.ActiveMessageCount;

            log.Info($"Collector: Current metric value is {backlog} at {DateTime.Now}");

            var resource = Environment.GetEnvironmentVariable("ResourceToScale");
            var value = new MetricValue(DateTime.Now, (int)backlog);
            return new Metric(resource, $"{topic}-{subscription}-backlog", value);
        }

        private static readonly TimeSpan period = TimeSpan.FromMinutes(10);
        private static readonly TimeSpan cooldownPeriod = TimeSpan.FromMinutes(10);
        private const int thresholdUp = 500;
        private const int thresholdDown = 300;

        [FunctionName("ScalingLogic")]
        [return: Queue("Actions")]
        public static ScaleAction ScalingLogic(
            [QueueTrigger("Metrics")] Metric metric, 
            [Table("Scaling", "{ResourceName}", "{Name}")] ScalingStateEntity stateEntity, 
            [Table("Scaling", "{ResourceName}", "{Name}")] out ScalingStateEntity newStateEntity,
            TraceWriter log)
        {
            // 1. Deserialize state
            var state = stateEntity?.SerializedState != null 
                ? JsonConvert.DeserializeObject<ScalingState>(stateEntity.SerializedState) 
                : new ScalingState();
            var history = state.History;
            log.Info($"Scaling logic: Received {metric.Name}, previous state is {string.Join(", ", history)}");

            // 2. Add current metric value, remove old values
            history.Add(metric.Value);
            history.RemoveAll(e => e.Time < metric.Value.Time.Subtract(period));

            // 3. Compare the aggregates to thresholds, produce scaling action if needed
            ScaleAction action = null;
            if (history.Count >= 5
                && DateTime.Now - state.LastScalingActionTime > cooldownPeriod)
            {
                var average = (int)history.Average(e => e.Value);
                var maximum = (int)history.Max(e => e.Value);
                if (average > thresholdUp)
                {
                    log.Info($"Scaling logic: Value {average} is too high, scaling {metric.ResourceName} up...");
                    state.LastScalingActionTime = DateTime.Now;
                    action = new ScaleAction(metric.ResourceName, ScaleActionType.Up);
                }
                else if (maximum < thresholdDown)
                {
                    log.Info($"Scaling logic: Value {maximum} is low, scaling {metric.ResourceName} down...");
                    state.LastScalingActionTime = DateTime.Now;
                    action = new ScaleAction(metric.ResourceName, ScaleActionType.Down);
                }
            }

            // 4. Serialize the state back and return the action
            newStateEntity = stateEntity != null ? stateEntity : new ScalingStateEntity { PartitionKey = metric.ResourceName, RowKey = metric.Name };
            newStateEntity.SerializedState = JsonConvert.SerializeObject(state);
            return action;
        }

        [FunctionName("Scaler")]
        public static void Scaler([QueueTrigger("Actions")] ScaleAction action, TraceWriter log)
        {
            log.Info($"Scaler executed at: {DateTime.Now}");

            var secrets = Environment.GetEnvironmentVariable("ServicePrincipal").Split(',');
            var credentials = SdkContext.AzureCredentialsFactory
                .FromServicePrincipal(secrets[0], secrets[1], secrets[2], AzureEnvironment.AzureGlobalCloud);
            var azure = Azure.Configure()
                .Authenticate(credentials)
                .WithDefaultSubscription();

            var plan = azure.AppServices
                .AppServicePlans
                .List()
                .First(p => p.Name.Contains(action.ResourceName));

            var newCapacity = action.Type == ScaleActionType.Down ? plan.Capacity - 1 : plan.Capacity + 1;
            log.Info($"Scaler: Switching {action.ResourceName} from {plan.Capacity} {action.Type} to {newCapacity}");

            plan.Update()
                .WithCapacity(newCapacity)
                .Apply();
        }
    }

    public enum ScaleActionType
    {
        Up,
        Down
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
   

    public class ScalingState
    {
        public List<MetricValue> History { get; } = new List<MetricValue>();

        public DateTime LastScalingActionTime { get; set; } = DateTime.MinValue;
    }

    public class ScalingStateEntity : TableEntity
    {
        public string SerializedState { get; set; }
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
}