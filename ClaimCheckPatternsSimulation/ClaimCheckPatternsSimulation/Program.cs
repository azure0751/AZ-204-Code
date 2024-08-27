using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    public static async Task Main(string[] args)
    {
        // Create a new configuration object
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Register the IConfiguration instance to be used by DI
                services.AddSingleton<IConfiguration>(configuration);

                // Register Producer and Consumer classes
                services.AddSingleton<Producer>();
                services.AddSingleton<Consumer>();
            })
            .Build();

        // Example usage of Producer and Consumer
        var producer = host.Services.GetRequiredService<Producer>();

        var randomJsonData = GenerateRandomJson();

        // Convert to JSON string
        string jsonString = JsonSerializer.Serialize(randomJsonData, new JsonSerializerOptions { WriteIndented = true });


        await producer.SendMessageAsync(jsonString);

        var consumer = host.Services.GetRequiredService<Consumer>();
        await consumer.ReceiveMessageAsync();

        await host.RunAsync();
    }

    static Dictionary<string, object> GenerateRandomJson()
    {
        var random = new Random();

        var json = new Dictionary<string, object>
        {
            { "id", Guid.NewGuid().ToString() },
            { "name", $"User{random.Next(1, 1000)}" },
            { "age", random.Next(18, 65) },
            { "email", $"user{random.Next(1, 1000)}@example.com" },
            { "isVerified", random.Next(0, 2) == 1 },
            { "preferences", new Dictionary<string, string>
                {
                    { "color", random.Next(0, 2) == 1 ? "blue" : "green" },
                    { "food", random.Next(0, 2) == 1 ? "pizza" : "pasta" }
                }
            },
            { "createdAt", DateTime.Now }
        };

        return json;
    }
}
