# Create Azure Traffic manager profile with Two endspoints we application deployed in EastUS and WestUS (Azure PowerShell)

# Variables
```powershell
$resourceGroup = "myResourceGroup"
$trafficManagerName = "myTrafficManager"
```


# Function to generate random names
```powershell
function Generate-RandomName {
    return [System.Guid]::NewGuid().ToString("N").Substring(0, 8)
}
```


```powershell
$appServicePlanName1 = "appServicePlan1-" + (Generate-RandomName)
$appServicePlanName2 = "appServicePlan2-" + (Generate-RandomName)
$appServiceName1 = "appService1-" + (Generate-RandomName)
$appServiceName2 = "appService2-" + (Generate-RandomName)
$domainName = "mytraf" + (Generate-RandomName)
```

# Locations
```powershell
$location1 = "eastus"
$location2 = "westus"
```


# 1. Create a resource group
```powershell
az group create --name $resourceGroup --location $location1
```


# 2. Create App Service Plans with Standard S1 SKU
```powershell
az appservice plan create --name $appServicePlanName1 --resource-group $resourceGroup --location $location1 --sku S1 
az appservice plan create --name $appServicePlanName2 --resource-group $resourceGroup --location $location2 --sku S1
```

# 3. Create Azure App Services in different regions
```powershell
az webapp create --name $appServiceName1 --resource-group $resourceGroup --plan $appServicePlanName1
```

```powershell
az webapp create --name $appServiceName2 --resource-group $resourceGroup --plan $appServicePlanName2
```


# 4. Create Traffic Manager Profile with Performance routing
```powershell
az network traffic-manager profile create --name $trafficManagerName --resource-group $resourceGroup --routing-method Performance --unique-dns-name $domainName
```


# 5. Create Traffic Manager Endpoints
```powershell
$appServiceId1 = az webapp show --name $appServiceName1 --resource-group $resourceGroup --query id -o tsv
```

```powershell
$appServiceId2 = az webapp show --name $appServiceName2 --resource-group $resourceGroup --query id -o tsv
```


```powershell
az network traffic-manager endpoint create --name ($appServiceName1 + "Endpoint") --profile-name $trafficManagerName --resource-group $resourceGroup --type AzureEndpoints --target-resource-id $appServiceId1 --endpoint-status Enabled
```

```powershell
az network traffic-manager endpoint create --name ($appServiceName2 + "Endpoint") --profile-name $trafficManagerName --resource-group $resourceGroup --type AzureEndpoints --target-resource-id $appServiceId2 --endpoint-status Enabled
```


# 6. Output Traffic Manager URL
```powershell
$trafficManagerUrl = az network traffic-manager profile show --name $trafficManagerName --resource-group $resourceGroup --query dnsConfig.fqdn -o tsv
Write-Output "Traffic Manager URL: http://$trafficManagerUrl"
```

