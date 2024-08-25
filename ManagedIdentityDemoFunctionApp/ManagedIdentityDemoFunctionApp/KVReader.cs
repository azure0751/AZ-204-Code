using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

namespace ManagedIdentityDemoFunctionApp
{
    public static class KVReader
    {
        [FunctionName("KVReader")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string kvname = req.Query["kvname"];
            string secretName = req.Query["secretname"];
            string secretValue = string.Empty;
            
            if(string.IsNullOrEmpty(kvname))
            {
                return new BadRequestObjectResult("key vault name [kvname] is not specified in query");
            }
            if (string.IsNullOrEmpty(secretName))
            {
                return new BadRequestObjectResult("Secret Name  [secretname] is not specified in query");
            }

            var kvUri = "https://" + kvname + ".vault.azure.net";
            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
            secretValue = GetKeyValultSecret(client, secretName);


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            kvname = kvname ?? data?.name;

            SecretDetail receivedsecretdetail = new SecretDetail();
            {
                receivedsecretdetail.KeyVaultName = kvname;
                receivedsecretdetail.SecretName = secretName;
                receivedsecretdetail.SecretValue = secretValue;
                receivedsecretdetail.KeyVaultAddress = kvUri;

            };

           // string responseMessage = kvname;

            return new OkObjectResult(receivedsecretdetail);
        }


        static string GetKeyValultSecret(SecretClient secretclient, string secretname)
        {
            try
            {
                var secret = secretclient.GetSecret(secretname);
                string secretValue = (secret.Value).Value.ToString();
                return secretValue;
            }

            catch (Exception ex)
            {
               return "exception" +ex.Message;
            }
        }
    }

    public class SecretDetail
    {
        public string KeyVaultName;
        public string SecretName;
        public string SecretValue;
        public string KeyVaultAddress;

    }
}
