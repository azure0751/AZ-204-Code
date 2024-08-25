using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureKeyVaultManagedIdentity.Models;
using Microsoft.AspNetCore.Mvc;


namespace AzureKeyVaultManagedIdentity.Controllers
{
    public class KeyVaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        
       

        [HttpPost]
        public async Task<IActionResult> GetSecret(KeyVaultRequestModel model)
        {
            if (string.IsNullOrEmpty(model.KeyVaultName) || string.IsNullOrEmpty(model.SecretName))
            {
                return BadRequest("Key Vault name and Secret name are required.");
            }

            try
            {
                // Construct the Key Vault URL
                string keyVaultUrl = $"https://{model.KeyVaultName}.vault.azure.net/";

                // Create a SecretClient using Managed Identity
                var client = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential());

                // Retrieve the secret
                KeyVaultSecret secret = await client.GetSecretAsync(model.SecretName);

                // Pass the secret value to the view
                ViewBag.SecretValue = secret.Value;
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
            }

            return View("ShowSecret");
        }
    }
}