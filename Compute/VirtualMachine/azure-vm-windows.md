## To enable the RDP (Remote Desktop Protocol) port (port 3389) for the Windows VM, you need to modify the script by adding the command to open port 3389. Here's the updated script:


### Define variables for the deployment
```plaintext
location="eastus"  # Set your desired location
resourceGroup="resourcegroup123"
vmName="MyWindowsVM"
image="Win2019Datacenter"
adminUser="azureuser"
adminPassword="YourPassword123!"  # Set a strong password
vmSize="Standard_D2s_v3"
osDiskSku="Standard_LRS"  # Specify Standard HDD for the OS disk
```


### Create the resource group if it doesn't exist
```plaintext
az group create --name $resourceGroup --location $location
```


### Create the virtual machine with Standard HDD and Windows Server 2019 Datacenter
```plaintext
az vm create \
  --resource-group $resourceGroup \
  --name $vmName \
  --image $image \
  --size $vmSize \
  --admin-username $adminUser \
  --admin-password $adminPassword \
  --location $location \
  --storage-sku $osDiskSku
```


### Open port 80 to allow HTTP traffic
```plaintext
az vm open-port \
  --port 80 \
  --resource-group $resourceGroup \
  --name $vmName
```


### Open port 3389 to allow RDP traffic
```plaintext
az vm open-port \
  --port 3389 \
  --resource-group $resourceGroup \
  --name $vmName
```

### Install IIS (Internet Information Services) on Windows VM

#### Option 1:Install IIS using custom script extension
```plaintext
az vm extension set \
  --publisher Microsoft.Compute \
  --version 1.10 \
  --name CustomScriptExtension \
  --vm-name $vmName \
  --resource-group $resourceGroup \
  --settings '{"commandToExecute":"powershell Add-WindowsFeature Web-Server"}'
```

#### Option 2: Manually Install IIS
Connect to the VM using Remote Desktop Protocol (RDP).


```plaintext
vmPublicIP=$(az vm show -d --resource-group $resourceGroup --name $vmName --query publicIps -o tsv)
echo $vmPublicIP
```

### Use an RDP client to connect:

```plaintext
mstsc /v:$vmPublicIP
```

### Log in using the admin username and password.

Open Server Manager and add the Web Server (IIS) role.

Complete the wizard to install IIS.

### Test the HTTP Server
After installing IIS, access the default IIS page by
```plaintext
navigating to http://$vmPublicIP in your browser.
```
