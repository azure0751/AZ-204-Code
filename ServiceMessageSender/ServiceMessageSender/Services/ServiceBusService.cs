using Azure.Messaging.ServiceBus;
using System.Threading.Tasks;

namespace ServiceMessageSender.Services
{
    public class ServiceBusService
    {
        public async Task SendMessagesAsync(string connectionString, string queueName, string messageContent, int messageCount)
        {
            await using var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queueName);

            for (int i = 1; i <= messageCount; i++)
            {
                var message = new ServiceBusMessage($"{messageContent} - Message {i}");
                await sender.SendMessageAsync(message);
            }
        }

        public async Task<List<string>> ReceiveMessagesAsync(string connectionString, string queueName, int maxMessages)
        {
            var messages = new List<string>();
            await using var client = new ServiceBusClient(connectionString);
            var receiver = client.CreateReceiver(queueName);

            var receivedMessages = await receiver.ReceiveMessagesAsync(maxMessages);

            foreach (var message in receivedMessages)
            {
                messages.Add(message.Body.ToString());
                await receiver.CompleteMessageAsync(message);
            }

            return messages;
        }
    }


}
