using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ManagedIdentityDemoFunctionApp
{
    public class abc
    {
        [FunctionName("abc")]
        public void Run([QueueTrigger("%queuename%", Connection = "abc")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
