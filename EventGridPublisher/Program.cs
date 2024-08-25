using Azure;
using Azure.Messaging.EventGrid;
using System;
using System.Threading.Tasks;

public class Program
{
    private static string topicEndpoint = string.Empty;

    private static string topicKey = string.Empty;

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Welcome to Eventgrid Submitter application");

        Console.WriteLine("Provide Azure Eventgrid Topic Endpoint :");
        topicEndpoint = Console.ReadLine();

        Console.WriteLine("Provide Azure Eventgrid Topic Key :");
        topicKey = Console.ReadLine();

        string continueoperation = "y";

        do
        {
            Uri endpoint = new Uri(topicEndpoint);
            AzureKeyCredential credential = new AzureKeyCredential(topicKey);
            EventGridPublisherClient client = new EventGridPublisherClient(endpoint, credential);

            string[] employeestatus = new string[] { "New Joinee", "Resigned", "Terminated", "On Medical Leave" };
            string[] employeenames = new string[] { "Tinku", "Vinay", "Vijay", "Gayathri", "Vinayak", "Diwakar", "Vinay", "Rambabu", "Mayuresh" };

            for (int i = 0; i <= 50; i++)
            {
                string randomemployeestatus = GetRandom(employeestatus);
                string randomemployeeName = GetRandom(employeenames);

                EventGridEvent egevent = new EventGridEvent(
               subject: $"Employee: {randomemployeeName} is {randomemployeestatus}",
               eventType: $"Employee.{randomemployeestatus}",
               dataVersion: "1.0",
               data: new
               {
                   FullName = randomemployeeName,
                   Address = "Dummy Address, Road no 1001, Universe"
               });

                Console.WriteLine($"Submitting Event {i} ");

                try
                {
                    await client.SendEventAsync(egevent);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("Do you want to continue ?:");
            continueoperation = Console.ReadLine();
        } while (continueoperation.ToLower() == "y");

        Console.WriteLine("Exiting:");
    }

    public static string GenerateName(int len)
    {
        Random r = new Random();
        string[] consonants = { "b", "c", "d", "f", "g", "j", "k", "l", "l", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
        string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
        string Name = "";
        Name += consonants[r.Next(consonants.Length)].ToUpper();
        Name += vowels[r.Next(vowels.Length)];
        int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
        while (b < len)
        {
            Name += consonants[r.Next(consonants.Length)];
            b++;
            Name += vowels[r.Next(vowels.Length)];
            b++;
        }

        return Name;
    }

    public static string GetRandom(string[] stringarray)
    {
        Random rnd = new Random();
        int index = rnd.Next(stringarray.Length);
        return stringarray[index];
    }
}