using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;

namespace MyExtensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public class ConfigurationAttribute : Attribute
    {
        [AutoResolve]
        public string Key { get; set; }
    }
}
