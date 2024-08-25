using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureKeyVaultManagedIdentity.Models;
using Microsoft.AspNetCore.Mvc;

namespace AzureKeyVaultManagedIdentity.Controllers
{
    public class ServicePrincipalController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> GetSecret(ServicePrincipalRequestModel model)
        {
            if (string.IsNullOrEmpty(model.KeyVaultName) || string.IsNullOrEmpty(model.SecretName) ||
                string.IsNullOrEmpty(model.TenantId) || string.IsNullOrEmpty(model.ClientId) ||
                string.IsNullOrEmpty(model.ClientSecret))
            {
                return BadRequest("All fields are required.");
            }

            try
            {
                // Construct the Key Vault URL
                string keyVaultUrl = $"https://{model.KeyVaultName}.vault.azure.net/";

                // Create a SecretClient using the service principal credentials
                var client = new SecretClient(
                    new Uri(keyVaultUrl),
                    new ClientSecretCredential(model.TenantId, model.ClientId, model.ClientSecret)
                );

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

        public IActionResult Index()
        {
            return View();
        }
    }
}