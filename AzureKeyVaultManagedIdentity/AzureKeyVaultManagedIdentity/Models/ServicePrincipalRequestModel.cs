namespace AzureKeyVaultManagedIdentity.Models
{
    public class ServicePrincipalRequestModel
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string KeyVaultName { get; set; }
        public string SecretName { get; set; }
    }
}
