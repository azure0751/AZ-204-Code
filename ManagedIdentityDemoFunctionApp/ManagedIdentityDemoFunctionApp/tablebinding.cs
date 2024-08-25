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
    public class TableStorage
    {
        public class MyPoco
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public string Text { get; set; }
        }

        [FunctionName("TableOutput")]
        [return: Table("shukla", Connection = "abc")]
        public static MyPoco TableOutput([HttpTrigger] dynamic input, ILogger log)
        {
            log.LogInformation($"C# http trigger function processed: {input.Text}");
            var obj = input;
            return new MyPoco { PartitionKey = "Http", RowKey = Guid.NewGuid().ToString(), Text = Convert.ToString(obj) };
        }
    }
}
