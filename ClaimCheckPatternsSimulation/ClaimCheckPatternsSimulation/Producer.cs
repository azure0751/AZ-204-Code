using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

public class Producer
{
    private readonly string _serviceBusConnectionString;
    private readonly string _queueName;
    private readonly string _blobConnectionString;
    private readonly string _containerName;

    public Producer(IConfiguration configuration)
    {
        _serviceBusConnectionString = configuration["Azure:ServiceBus:ConnectionString"];
        _queueName = configuration["Azure:ServiceBus:QueueName"];
        _blobConnectionString = configuration["Azure:BlobStorage:ConnectionString"];
        _containerName = configuration["Azure:BlobStorage:ContainerName"];
    }

    public async Task SendMessageAsync(string data)
    {
        // Upload data to Blob Storage
        BlobServiceClient blobServiceClient = new BlobServiceClient(_blobConnectionString);
        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        BlobClient blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString());

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        await blobClient.UploadAsync(stream, true);

        // Get the Blob URL (Claim Check)
        string claimCheck = blobClient.Uri.ToString();

        // Create a Service Bus client and send the claim check message
        ServiceBusClient client = new ServiceBusClient(_serviceBusConnectionString);
        ServiceBusSender sender = client.CreateSender(_queueName);

        ServiceBusMessage message = new ServiceBusMessage(claimCheck);
        await sender.SendMessageAsync(message);

        Console.WriteLine($"Claim Check sent: {claimCheck}");
    }
}
