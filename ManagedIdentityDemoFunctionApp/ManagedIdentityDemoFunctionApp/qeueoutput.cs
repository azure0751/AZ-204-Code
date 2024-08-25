using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ManagedIdentityDemoFunctionApp
{
    public static class qeueoutput
    {
        [FunctionName("qeueoutput")]
        [return: Queue("myqueue-items", Connection = "abc")]
        public static string QueueOutput([HttpTrigger] dynamic input, ILogger log)
        {
            log.LogInformation($"C# function processed: {input}");
            var obj = input;
            
            return Convert.ToString(obj);
        }
    }
}
