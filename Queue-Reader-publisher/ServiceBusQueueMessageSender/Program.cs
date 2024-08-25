using Azure.Messaging.ServiceBus;
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage
using Figgle;
using Microsoft.Extensions.Configuration;
using System; // Namespace for Console output
using System.Configuration; // Namespace for ConfigurationManager
using System.Drawing;
using System.Threading.Tasks; // Namespace for Task

public class Program
{
    static ServiceBusClient client;
    static ServiceBusSender sender;
    private const int numOfMessages = 10;
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("AppSettings.json", true)
               .AddEnvironmentVariables();

        var config = configuration.Build();


        string titleOfProgram = config["ProgramTitle"];

        string defaultfont = config["DefaultFont"];
        short defaultFontSize = Convert.ToInt16(config["DefaultFontSize"]);
        //DefaultFont
        ConsoleHelper.SetCurrentFont(defaultfont, defaultFontSize);

        var defaultFontColor = config["DefaultFontColor"];

        ConsoleColor consoleColor = ConsoleColor.White;
        try
        {
            consoleColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), defaultFontColor, true);
        }
        catch (Exception)
        {
            //Invalid color
        }

        Console.ForegroundColor = consoleColor;


        Console.Title = titleOfProgram;


        Console.WriteLine(FiggleFonts.Standard.Render(titleOfProgram));




        Console.WriteLine("Welcome to Service Bus Queue Submitter Console application");

        Console.WriteLine("Provide Azure Service Bus Connection String: :");
        string servicebusConnectionString = Console.ReadLine();

        Console.WriteLine("Provide Azure Service Bus Queue Name: ");
        string servicebusQueueName = Console.ReadLine();





        string continueoperation = "y";

        try
        {

            do
            {

                Console.WriteLine("How many messages , you want to sent: ");
                int noOfMessages = Convert.ToInt32(Console.ReadLine());

                client = new ServiceBusClient(servicebusConnectionString);
                sender = client.CreateSender(servicebusQueueName);

                using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

                for (int i = 1; i <= noOfMessages; i++)
                {
                    string datetimenow = System.DateTime.Now.ToString() + ": "+ System.DateTime.Now.Millisecond.ToString();
                        if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i} is generated at {datetimenow}")))
                        {
                            throw new Exception($"The message {i} is too large to fit in the batch.");
                        }
                   
                    Console.WriteLine($"{i} inserted to queue.");
                }

                await sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");

                Console.WriteLine("Do you want to continue ? y/n:");
                continueoperation = Console.ReadLine();




            } while (continueoperation.ToLower() == "y");

            Console.WriteLine("Exiting:");
        }
        catch (Exception ex)
        {
            WriteException($"Exception: {ex.Message}\n\n");
            //Console.WriteLine($"Exception: {ex.Message}\n\n");
        }
        finally
        {
            await sender.DisposeAsync();
            await client.DisposeAsync();
        }
    }

    public void CreateQueueClient(string queueName, string storageConnectionSTring)
    {
        // Instantiate a QueueClient which will be used to create and manipulate the queue
        QueueClient queueClient = new QueueClient(storageConnectionSTring, queueName);
    }

    public bool CreateQueue(string queueName, string storageConnectionSTring)
    {
        try
        {

            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(storageConnectionSTring, queueName);

            // Create the queue
            queueClient.CreateIfNotExists();

            if (queueClient.Exists())
            {
                Console.WriteLine($"Queue created: '{queueClient.Name}'");
                return true;
            }
            else
            {
                Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}\n\n");
            Console.WriteLine($"Make sure the Azurite storage emulator running and try again.");
            return false;
        }
    }

    public static void InsertMessage(string queueName, string message, string storageConnectionSTring)
    {

        // Instantiate a QueueClient which will be used to create and manipulate the queue
        QueueClient queueClient = new QueueClient(storageConnectionSTring, queueName);

        // Create the queue if it doesn't already exist
        queueClient.CreateIfNotExists();

        if (queueClient.Exists())
        {
            // Send a message to the queue
            queueClient.SendMessage(message);
        }

        Console.WriteLine($"Inserted: {message}");
    }

    public static void WriteException(string message)
    {
        ConsoleColor foreground = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = foreground;


    }
}