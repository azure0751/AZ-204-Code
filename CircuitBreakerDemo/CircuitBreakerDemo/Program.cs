using System;
using System.Net.Http;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;

class Program
{
    private static async Task Main(string[] args)
    {
        // Create a Circuit Breaker policy
        var circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 2, // Number of exceptions before circuit breaker opens
                durationOfBreak: TimeSpan.FromSeconds(10), // Duration to keep the circuit open
                onBreak: (exception, duration) => Console.WriteLine($"Circuit breaker opened: {exception.Message}"),
                onReset: () => Console.WriteLine("Circuit breaker reset"),
                onHalfOpen: () => Console.WriteLine("Circuit breaker is half-open")
            );

        var httpClient = new HttpClient();

        for (int i = 0; i < 500; i++)
        {
            try
            {
                // Use the Circuit Breaker policy to execute the HTTP request
                await circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    // Simulate an HTTP request
                    var response = await httpClient.GetAsync("https://demoappconfiginstace2.azurewebsites.net");

                    // Simulate failure
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        throw new HttpRequestException("Request failed.");
                    }

                    Console.WriteLine("Request succeeded.");
                });
            }
            catch (BrokenCircuitException)
            {
                Console.WriteLine("Circuit breaker is open, request was not executed.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Request failed: {ex.Message}");
            }

            // Wait before the next iteration
            await Task.Delay(1000);
        }
    }
}
