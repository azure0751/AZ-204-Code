### Create Azure Key vault using Azure  Biceps template 
## Step 1: Create a Bicep file (e.g., keyvault.bicep)

```plaintext
param location string = resourceGroup().location
param keyVaultName string
param secretName string = 'MySecret'
param secretValue string = 'ThisIsASecretValue'
param userObjectId string

resource keyVault 'Microsoft.KeyVault/vaults@2022-11-01' = {
  name: keyVaultName
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    accessPolicies: [
      {
        objectId: userObjectId
        tenantId: subscription().tenantId
        permissions: {
          secrets: [
            'get'
            'list'
            'set'
            'delete'
          ]
          keys: [
            'get'
            'list'
            'create'
            'delete'
          ]
          certificates: [
            'get'
            'list'
          ]
        }
      }
    ]
  }
}

resource secret 'Microsoft.KeyVault/vaults/secrets@2022-11-01' = {
  parent: keyVault
  name: secretName
  properties: {
    value: secretValue
  }
}

output keyVaultUri string = keyVault.properties.vaultUri
```
Explanation:
Parameters:

location: The location of the Key Vault, defaults to the resource group location.

keyVaultName: The name of the Key Vault, passed as a parameter.

secretName: The name of the secret, default is MySecret.
secretValue: The value of the secret, passed as a parameter.
userObjectId: The Object ID of the user to whom you want to assign the access policy.
Resources:

keyVault: This resource defines the Key Vault and sets up an access policy for the specified user, allowing management of secrets, keys, and certificates.
secret: Defines the secret resource inside the Key Vault with the specified value.

### Step 2: Deploy the Bicep file
First, get your user object ID:


```plaintext
USER_OBJECT_ID=$(az ad signed-in-user show --query objectId -o tsv)
```

Then deploy the Bicep file:

bash

```plaintext
az deployment group create \
  --name MyKeyVaultDeployment \
  --resource-group MyResourceGroup \
  --template-file keyvault.bicep \
  --parameters keyVaultName=<your-keyvault-name> userObjectId=$USER_OBJECT_ID secretValue=<your-secret-value>
```

Step 3: Retrieve the secret value using CLI
Once the deployment is complete, you can retrieve the secret value with this command:

bash
```plaintext
az keyvault secret show --vault-name <your-keyvault-name> --name MySecret --query value --output tsv
```

This Bicep file creates a Key Vault, sets a secret, and assigns access policies to your user ID for managing secrets, keys, and certificates.