using System;
using System.Threading.Tasks;
using Polly;
using Polly.Retry;

namespace RetryPatternDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Define a retry policy using Polly
            var retryPolicy = Policy
                .Handle<Exception>() // Handle any exception
                .WaitAndRetryAsync(
                    retryCount: 5, // Number of retries
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Exponential backoff
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Retry {retryCount} implemented with Polly. Waiting {timeSpan} before next retry.");
                    });

            const int numberOfOperations = 100; // Number of operations to demonstrate

            for (int i = 1; i <= numberOfOperations; i++)
            {
                Console.WriteLine($"\nOperation {i} starting...");

                // Execute the operation with retry policy
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await SimulateOperationAsync();
                });

                Console.WriteLine($"Operation {i} completed.\n");
            }

            Console.WriteLine("All operations completed.");
        }
        static async Task SimulateOperationAsync()
        {
            // Simulate an operation that may fail
            var random = new Random();
            var result =  random.Next(0, 2); // Randomly succeed or fail

            if (result == 0)
            {
                // Simulate failure
                Console.WriteLine("simulation Operation failed.");
                throw new Exception("Simulated operation failure.");
            }

            // Simulate success
            Console.WriteLine("Operation succeeded.");
        }
    }
}
