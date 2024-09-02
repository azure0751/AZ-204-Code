# Create Azure Traffic manager profile with Two endspoints we application deployed in  EastUS and WestUS

## Variables
```plaintext
resourceGroup="TMDemoResourceGroup"

trafficManagerName="myTrafficManager"
```


## Generate random names for the App Services and Plans
```plaintext
generate_random_name() {

    echo $(openssl rand -hex 6)
}
```


```plaintext
appServicePlanName1="appServicePlan1-$(generate_random_name)"

appServicePlanName2="appServicePlan2-$(generate_random_name)"

appServiceName1="appService1-$(generate_random_name)"

appServiceName2="appService2-$(generate_random_name)"

domainName="mytraf$(generate_random_name)"
```


## Locations
```plaintext
location1="eastus"
location2="westus"
```


### 1. Create a resource group
```plaintext
az group create --name $resourceGroup --location $location1
```


### 2. Create App Service Plans with Standard S1 SKU
```plaintext
az appservice plan create --name $appServicePlanName1 --resource-group $resourceGroup --location $location1 --sku S1
az appservice plan create --name $appServicePlanName2 --resource-group $resourceGroup --location $location2 --sku S1
```


## 3. Create Azure App Services in different regions
```plaintext
az webapp create --name $appServiceName1 --resource-group $resourceGroup --plan $appServicePlanName1 
az webapp create --name $appServiceName2 --resource-group $resourceGroup --plan $appServicePlanName2
```
### Deploy Location viewer App directly from GitHub Repository
```plaintext
az webapp deployment source config --name $appServiceName1 --resource-group $resourceGroup --repo-url https://github.com/azure0751/locationViewer.git --branch main --manual-integration
```
```plaintext
az webapp deployment source config --name $appServiceName2 --resource-group $resourceGroup --repo-url https://github.com/azure0751/locationViewer.git --branch main --manual-integration
```


## 4. Create Traffic Manager Profile with Performance routing

```plaintext
az network traffic-manager profile create --name $trafficManagerName --resource-group $resourceGroup --routing-method Performance --unique-dns-name $domainName
```




## 5. Create Traffic Manager Endpoints
```plaintext
az network traffic-manager endpoint create --name "${appServiceName1}Endpoint" --profile-name $trafficManagerName --resource-group $resourceGroup --type AzureEndpoints --target-resource-id "/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$resourceGroup/providers/Microsoft.Web/sites/$appServiceName1" --endpoint-status Enabled

az network traffic-manager endpoint create --name "${appServiceName2}Endpoint" --profile-name $trafficManagerName --resource-group $resourceGroup --type AzureEndpoints --target-resource-id "/subscriptions/$(az account show --query id -o tsv)/resourceGroups/$resourceGroup/providers/Microsoft.Web/sites/$appServiceName2" --endpoint-status Enabled
```




## 6. Output Traffic Manager URL
```plaintext
trafficManagerUrl=$(az network traffic-manager profile show  --name $trafficManagerName --resource-group $resourceGroup --query dnsConfig.fqdn -o tsv)
echo "Traffic Manager URL: http://$trafficManagerUrl"
```
