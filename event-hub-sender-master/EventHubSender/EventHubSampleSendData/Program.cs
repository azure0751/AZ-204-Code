using Azure.Messaging.EventHubs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs.Producer;

namespace SendSampleData
{
    class Program
    {
        static string eventHubName = "";
        static string connectionString = "";
        

        public static async Task Main(string[] args)
        {
            string docontinue=string.Empty;
            Console.WriteLine("wlcome to Eventgrid Sender console application!");

            Console.WriteLine("Input Connection String for event hub");
            connectionString = Console.ReadLine();

            Console.WriteLine("Input Event HUB name");
            eventHubName = Console.ReadLine();
            int noofevents = 1;

            do
            {

                await EventHubIngestionAsync(noofevents);
                Console.WriteLine("Do you want to send other batch of events");
                docontinue = Console.ReadLine().ToLower();
                Console.WriteLine("How many events in this batch");
                noofevents = Convert.ToInt32(Console.ReadLine());

            } while (docontinue=="y");
        }

        public static async Task EventHubIngestionAsync(int noOfevents)
        {
            await using (var producerClient = new EventHubProducerClient(connectionString, eventHubName))
            {
                Console.WriteLine($"{noOfevents} events will be send.");
                int counter = 0;
                for (int i = 0; i < noOfevents; i++)
                {
                    int recordsPerMessage = 3;
                    try
                    {
                        var records = Enumerable
                            .Range(0, recordsPerMessage)
                            .Select(recordNumber => $"{{\"timeStamp\": \"{DateTime.UtcNow.AddSeconds(100 * counter)}\", \"name\": \"{$"name {counter}"}\", \"metric\": {counter + recordNumber}, \"source\": \"EventHubMessage\"}}");
                        
                        string recordString = string.Join(Environment.NewLine, records);

                        EventData eventData = new EventData(Encoding.UTF8.GetBytes(recordString));
                        Console.WriteLine($"sending message {counter}");
                        // Optional "dynamic routing" properties for the database, table, and mapping you created. 
                        eventData.Properties.Add("Table", "TestTable");
                        eventData.Properties.Add("IngestionMappingReference", "TestMapping");
                        eventData.Properties.Add("Format", "json");

                        using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
                        eventBatch.TryAdd(eventData);

                        await producerClient.SendAsync(eventBatch);
                    }
                    catch (Exception exception)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("{0} > Exception: {1}", DateTime.Now, exception.Message);
                        Console.ResetColor();
                    }

                    // counter += recordsPerMessage;
                    counter = counter + 1;
                }
            }
        }
    }
}
