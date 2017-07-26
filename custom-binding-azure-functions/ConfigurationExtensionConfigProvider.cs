using System;
using System.Configuration;
using System.Linq;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Config;

namespace MyExtensions
{
    /// <summary>
    /// Extension for binding <see cref="ConfigurationAttribute"/>.
    /// This reads values from <see cref="ConfigurationManager.AppSettings"/>. 
    /// </summary>
    public class ConfigurationExtensionConfigProvider : IExtensionConfigProvider
    {
        /// <summary>
        /// This callback is invoked by the WebJobs framework before the host starts execution. 
        /// It should add the binding rules and converters for <see cref="ConfigurationAttribute"/>.
        /// </summary>
        /// <param name="context"></param>
        public void Initialize(ExtensionConfigContext context)
        {
            var rule = context.AddBindingRule<ConfigurationAttribute>();
            rule.BindToInput<string>(a => ConfigurationManager.AppSettings[a.Key]);

            rule.BindToInput<Env>(_ => new Env());
            var cm = context.Config.GetService<IConverterManager>();
            cm.AddConverter<Env, OpenType, ConfigurationAttribute>(typeof(PocoConverter<>));
        }

        private class Env
        {
            public string GetValue(string key) => ConfigurationManager.AppSettings[key];
        }

        private class PocoConverter<T> : IConverter<Env, T>
        {
            public T Convert(Env env)
            {
                var values = typeof(T)
                    .GetProperties()
                    .Select(p => p.Name)
                    .Select(env.GetValue)
                    .Cast<object>()
                    .ToArray();

                var constructor = typeof(T).GetConstructor(values.Select(v => v.GetType()).ToArray());
                if (constructor == null)
                {
                    throw new Exception("We tried to bind to your C# class, but it looks like there's no constructor which accepts all property values");
                }

                return (T) constructor.Invoke(values);
            }
        }
    }    
}