using System;

using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
namespace EventhubSenderConsole
{
    internal class Program
    {
        
            // connection string to the Event Hubs namespace
       static string connectionString = "";

        // name of the event hub
      static string eventHubName = "";

        // number of events to be sent to the event hub
         static int numOfEvents = 10;


        // The Event Hubs client types are safe to cache and use as a singleton for the lifetime
        // of the application, which is best practice when events are being published or read regularly.
        private static EventHubProducerClient producerClient;
        static async Task Main()
        {
            Console.WriteLine("wlcome to Eventgrid Sender console application!");

            Console.WriteLine("Input Connection String for event hub");
            connectionString = Console.ReadLine();

            Console.WriteLine("Input Event HUB name");
            eventHubName = Console.ReadLine();

            // Create a producer client that you can use to send events to an event hub
            producerClient = new EventHubProducerClient(connectionString, eventHubName);

            string dofurther= string.Empty;

            do
            {
               
                // Create a batch of events 
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                Console.WriteLine($"Sending batch of {numOfEvents}");

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
                    await producerClient.SendAsync(eventBatch);
                    Console.WriteLine($"A batch of {numOfEvents} events has been published.");
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Excaption :  {ex.Message} .");
                }
               

                Console.WriteLine("Do you want to send other batch of events");
                dofurther = Console.ReadLine().ToLower();

                Console.WriteLine("How many events in this batch");
                numOfEvents = Convert.ToInt32(Console.ReadLine());

            } while(dofurther == "y");
             
                await producerClient.DisposeAsync();
            


        }
    }
}