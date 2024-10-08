﻿using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace MessageReceiver
{
    class Program
    {
       
        static ServiceBusClient client;
        static ServiceBusProcessor processor;

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");
            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        static async Task Main()
        {

            Console.WriteLine("Welcome to Service Bus Queue Submitter Console application");

            Console.WriteLine("Provide Azure Service Bus Connection String: :");
            string servicebusConnectionString = Console.ReadLine();

            Console.WriteLine("Provide Azure Service Bus Queue Name: ");
            string servicebusQueueName = Console.ReadLine();


            client = new ServiceBusClient(servicebusConnectionString);
            processor = client.CreateProcessor(servicebusQueueName, new ServiceBusProcessorOptions());

            try
            {
                processor.ProcessMessageAsync += MessageHandler;
                processor.ProcessErrorAsync += ErrorHandler;

                await processor.StartProcessingAsync();
                Console.WriteLine("Wait for a minute and then press any key to end the processing");
                Console.ReadKey();

                Console.WriteLine("\nStopping the receiver...");
                await processor.StopProcessingAsync();
                Console.WriteLine("Stopped receiving messages");
            }
            finally
            {
                await processor.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}
