using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ManagedIdentityDemoFunctionApp
{
    public class blob
    {
        [FunctionName("blob")]
        public void Run([BlobTrigger("x1abc/{name}", Connection = "abc")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
