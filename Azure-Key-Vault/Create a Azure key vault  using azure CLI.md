# Create a Azure key vault  using azure CLI

### Set the required variables
```plaintext
LOCATION="eastus" # You can change this to your preferred Azure region
RESOURCE_GROUP="MyResourceGroup" # Replace with your resource group name
KV_NAME="kv-$RANDOM" # Random Key Vault name
SECRET_NAME="MySecret"
SECRET_VALUE="ThisIsASecretValue"
```


Step 2: Create a resource group (if it doesn't exist)
bash
Copy code
### Create a resource group
```plaintext
az group create --name $RESOURCE_GROUP --location $LOCATION
```

Step 3: Create the Key Vault
bash
Copy code
### Create a Key Vault
```plaintext
az keyvault create --name $KV_NAME --resource-group $RESOURCE_GROUP --location $LOCATION
```

Step 4: Insert a secret into the Key Vault
bash
Copy code
### Set a secret in the Key Vault
```plaintext
az keyvault secret set --vault-name $KV_NAME --name $SECRET_NAME --value $SECRET_VALUE
```

Step 5: Retrieve the secret value
bash
Copy code
### Get the secret value from the Key Vault
```plaintext
az keyvault secret show --vault-name $KV_NAME --name $SECRET_NAME --query value --output tsv
```




To assign a Key Vault access policy to your user ID using Azure CLI, follow these steps:


### Get your Azure User ID (Object ID)
```plaintext
USER_ID=$(az ad signed-in-user show --query objectId -o tsv)
```

Step 3: Assign Key Vault access policy to your user ID
bash
Copy code
### Assign access policy to allow your user to manage secrets, keys, and certificates
```plaintext
az keyvault set-policy --name $KV_NAME --resource-group $RESOURCE_GROUP --object-id $USER_ID --secret-permissions get list set delete --key-permissions get list create delete --certificate-permissions get list
```



