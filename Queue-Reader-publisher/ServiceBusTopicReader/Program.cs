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

    // the processor that reads and processes messages from the subscription
    static ServiceBusProcessor processor;

  static string servicebusTopicSubscriptionName;

    // handle received messages
    static async Task MessageHandler(ProcessMessageEventArgs args)
    {
        string body = args.Message.Body.ToString();
        Console.WriteLine($"Received: {body} from subscription: {servicebusTopicSubscriptionName}");

        // complete the message. messages is deleted from the subscription. 
        await args.CompleteMessageAsync(args.Message);
    }

    // handle any errors when receiving messages
    static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
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




        Console.WriteLine("Welcome to Service Bus Topic Subscription Reader Console application");

        Console.WriteLine("Provide Azure Service Bus Connection String: :");
        string servicebusConnectionString = Console.ReadLine();

        Console.WriteLine("Provide Azure Service BusTopic Name: ");
        string servicebusTopicName = Console.ReadLine();

        Console.WriteLine("Provide Azure Service Topic Subscription Name: ");
        servicebusTopicSubscriptionName = Console.ReadLine();


        client = new ServiceBusClient(servicebusConnectionString);

        // create a processor that we can use to process the messages
        processor = client.CreateProcessor(servicebusTopicName, servicebusTopicSubscriptionName, new ServiceBusProcessorOptions());

        try
        {
            processor.ProcessMessageAsync += MessageHandler;

            // add handler to process any errors
            processor.ProcessErrorAsync += ErrorHandler;

            // start processing 
            await processor.StartProcessingAsync();

            Console.WriteLine("Wait for a minute and then press any key to end the processing");
            Console.ReadKey();

            // stop processing 
            Console.WriteLine("\nStopping the receiver...");
            await processor.StopProcessingAsync();
            Console.WriteLine("Stopped receiving messages");

        }
        catch (Exception ex)
        {
            WriteException($"Exception: {ex.Message}\n\n");
            //Console.WriteLine($"Exception: {ex.Message}\n\n");
        }
        finally
        {
            // Calling DisposeAsync on client types is required to ensure that network
            // resources and other unmanaged objects are properly cleaned up.
            await processor.DisposeAsync();
            await client.DisposeAsync();
        }
    }

   
   

    

    public static void WriteException(string message)
    {
        ConsoleColor foreground = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = foreground;


    }
}