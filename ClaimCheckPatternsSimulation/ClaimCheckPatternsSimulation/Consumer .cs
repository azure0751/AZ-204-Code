using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

public class Consumer
{
    private readonly string _serviceBusConnectionString;
    private readonly string _queueName;
    private readonly string _blobConnectionString;

    public Consumer(IConfiguration configuration)
    {
        _serviceBusConnectionString = configuration["Azure:ServiceBus:ConnectionString"];
        _queueName = configuration["Azure:ServiceBus:QueueName"];
        _blobConnectionString = configuration["Azure:BlobStorage:ConnectionString"];
    }

    public async Task ReceiveMessageAsync()
    {
        // Create a Service Bus client and receive the claim check message
        ServiceBusClient client = new ServiceBusClient(_serviceBusConnectionString);
        ServiceBusProcessor processor = client.CreateProcessor(_queueName, new ServiceBusProcessorOptions());

        processor.ProcessMessageAsync += async args =>
        {
            

            Dictionary<string, string> settings = ParseConnectionString(_blobConnectionString);

            string accountName = settings["AccountName"];
            string accountKey = settings["AccountKey"];







            string claimCheck = args.Message.Body.ToString();
            Console.WriteLine($"Claim Check received: {claimCheck}");



            // Retrieve the data from Blob Storage using the Claim Check
           BlobClient blobClient = new BlobClient(new Uri(claimCheck), new StorageSharedKeyCredential(accountName, accountKey));

            BlobDownloadInfo download = await blobClient.DownloadAsync();

            using (var reader = new StreamReader(download.Content))
            {
                string data = await reader.ReadToEndAsync();
                Console.WriteLine($"Data retrieved: {data}");
            }

            // Complete the message
            await args.CompleteMessageAsync(args.Message);
        };

        processor.ProcessErrorAsync += args =>
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync();
    }


    static Dictionary<string, string> ParseConnectionString(string connectionString)
    {
        var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Split the connection string by ';' and '='
        var keyValues = connectionString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var keyValue in keyValues)
        {
            var parts = keyValue.Split(new[] { '=' }, 2);
            if (parts.Length == 2)
            {
                settings[parts[0]] = parts[1];
            }
        }

        return settings;
    }
}
