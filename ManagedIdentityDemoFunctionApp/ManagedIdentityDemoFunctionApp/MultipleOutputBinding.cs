using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Function_Queue_Table
{
    public static class MyFunction
    {
        [FunctionName("Multipleoutputbindings")]
        // The ICollector interface is to write multiple to the 
        //output
        public static void Run([QueueTrigger("myqueue-items", Connection = "abc")] JObject myQueueItem,
            [Table("shukla", Connection = "abc")] ICollector<MyPoco> outputTable,
            [Blob("shukla/{rand-guid}", FileAccess.Write, Connection = "abc")] TextWriter blobOutput,
            ILogger log)
        {
            log.LogInformation("Adding Customer");
            MyPoco obj = new MyPoco();
            obj.PartitionKey = myQueueItem["Id"].ToString();
            obj.RowKey = myQueueItem["Id"].ToString();
            obj.Text = "abc";
            outputTable.Add(obj); // Use ICollector<T>            

            blobOutput.Write($"Partition Key {obj.PartitionKey}");
            // For blob, you have an output of Stream, 
            //string,CloudBlockBlob}
        }
    }
    public class MyPoco
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Text { get; set; }
    }
}