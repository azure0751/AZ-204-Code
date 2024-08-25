using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using System.Diagnostics;

namespace EventhubFunction
{

    public class EventProcessorFunction
    {
        [FunctionName("EventProcessorFunction")]


        public void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            string ehubNamespaceConnectionString = "";
       string eventHubName = "shukla";
        string blobStorageConnectionString = "";
        string blobContainerName = "chkpoint";

        log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            

        BlobContainerClient storageClient;

            // The Event Hubs client types are safe to cache and use as a singleton for the lifetime
            // of the application, which is best practice when events are being published or read regularly.        
            EventProcessorClient processor;

        // Read from the default consumer group: $Default
        string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

        // Create a blob container client that the event processor will use 
        storageClient = new BlobContainerClient(blobStorageConnectionString, blobContainerName);

        // Create an event processor client to process events in the event hub
        processor = new EventProcessorClient(storageClient, consumerGroup, ehubNamespaceConnectionString, eventHubName);

        // Register handlers for processing events and handling errors
        processor.ProcessEventAsync += ProcessEventHandler;
        processor.ProcessErrorAsync += ProcessErrorHandler;

        // Start the processing
        processor.StartProcessingAsync();

        // Wait for 30 seconds for the events to be processed
        Task.Delay(TimeSpan.FromSeconds(30));

        // Stop the processing
        processor.StopProcessingAsync();
    }

Task ProcessEventHandler(ProcessEventArgs eventArgs)
    {
        // Write the body of the event to the console window
        tConsole.WriteLine("\tReceived event: {0}", Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray()));

        // Update checkpoint in the blob storage so that the app receives only new events the next time it's run
        eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
            return Task.CompletedTask; ;

    }

        Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
    {
        // Write details about the error to the console window
        Console.WriteLine($"\tPartition '{eventArgs.PartitionId}': an unhandled exception was encountered. This was not expected to happen.");
        Console.WriteLine(eventArgs.Exception.Message);
        return Task.CompletedTask;
    }
}
}
