using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace EventhubSenderFunction
{
    public class EventHubSender
    {
        [FunctionName("EventHubSender")]
        public void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer Event Sender trigger function executed at: {DateTime.Now}");
            // connection string to the Event Hubs namespace
        string connectionString = "Endpoint=sb://az305ehubshukla21sept.servicebus.windows.net/;SharedAccessKeyName=sender;SharedAccessKey=t34Hez2FZKs0eBjq4BRlTz5tkgg3zTHwsi1Kf5aLx68=";

        // name of the event hub
        string eventHubName = "shukla";

        // number of events to be sent to the event hub
       int numOfEvents = 3;
        // The Event Hubs client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when events are being published or read regularly.
       

        // Create a producer client that you can use to send events to an event hub
        EventHubProducerClient producerClient = new EventHubProducerClient(connectionString, eventHubName);

        // Create a batch of events 
        using EventDataBatch eventBatch = producerClient.CreateBatchAsync().Result;

            for (int i = 1; i <= numOfEvents; i++)
            {
                if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Event {i}"))))
                {
                    // if it is too large for the batch
                    throw new Exception($"Event {i} is too large for the batch and cannot be sent.");
                }
            }

            try
            {
                // Use the producer client to send the batch of events to the event hub
                producerClient.SendAsync(eventBatch);
                Console.WriteLine($"A batch of {numOfEvents} events has been published.");
            }
            finally
            {
                producerClient.DisposeAsync();
            }



        }
    }
}
