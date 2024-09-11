## To create a complete virtual machine (VM) with all required components, including a Network Security Group (NSG), Virtual Network (VNet), Subnet, and Network Interface (NIC), you need to follow the steps below. Each command needs to be executed in the order specified to set up a complete environment for the VM.


# Define variables for the deployment
```plaintext
location="eastus"                   # Set your desired location
resourceGroup="resourcegroup123"     # Resource group name
vmName="MyWindowsVM"                 # Virtual machine name
vnetName="MyVNet"                    # Virtual network name
subnetName="MySubnet"                # Subnet name
nsgName="MyNSG"                      # Network Security Group name
nicName="MyNIC"                      # Network Interface name
image="Win2019Datacenter"            # VM image
adminUser="azureuser"                # VM admin username
adminPassword="YourPassword123!"     # VM admin password
vmSize="Standard_D2s_v3"             # VM size
osDiskSku="Standard_LRS"             # OS Disk SKU
```


# Create the resource group
```plaintext
az group create --name $resourceGroup --location $location
```


# Create a virtual network
```plaintext
az network vnet create \
  --resource-group $resourceGroup \
  --name $vnetName \
  --address-prefix 10.0.0.0/16 \
  --subnet-name $subnetName \
  --subnet-prefix 10.0.0.0/24
```


# Create a Network Security Group (NSG)
```plaintext
az network nsg create \
  --resource-group $resourceGroup \
  --name $nsgName \
  --location $location
```

Step 5: Create Security Rules for the NSG
You will need to add rules to allow traffic on RDP (port 3389) and HTTP (port 80).


```plaintext
# Allow RDP (Port 3389) traffic
az network nsg rule create \
  --resource-group $resourceGroup \
  --nsg-name $nsgName \
  --name AllowRDP \
  --priority 1000 \
  --protocol Tcp \
  --direction Inbound \
  --source-address-prefixes '*' \
  --source-port-ranges '*' \
  --destination-address-prefixes '*' \
  --destination-port-ranges 3389 \
  --access Allow
```


```plaintext
# Allow HTTP (Port 80) traffic
az network nsg rule create \
  --resource-group $resourceGroup \
  --nsg-name $nsgName \
  --name AllowHTTP \
  --priority 1001 \
  --protocol Tcp \
  --direction Inbound \
  --source-address-prefixes '*' \
  --source-port-ranges '*' \
  --destination-address-prefixes '*' \
  --destination-port-ranges 80 \
  --access Allow
```


# Create a public IP address for the VM
```plaintext
az network public-ip create \
  --resource-group $resourceGroup \
  --name MyPublicIP \
  --allocation-method Static
```


# Create a NIC and associate it with the subnet and NSG
```plaintext
az network nic create \
  --resource-group $resourceGroup \
  --name $nicName \
  --vnet-name $vnetName \
  --subnet $subnetName \
  --network-security-group $nsgName \
  --public-ip-address MyPublicIP
```


# Create the VM using the NIC, and specify Standard HDD for the OS disk
```plaintext
az vm create \
  --resource-group $resourceGroup \
  --name $vmName \
  --image $image \
  --size $vmSize \
  --admin-username $adminUser \
  --admin-password $adminPassword \
  --nics $nicName \
  --location $location \
  --storage-sku $osDiskSku
```


# Open port 80 to allow HTTP traffic (If you didn't create NSG rules for HTTP)
```plaintext
az vm open-port \
  --resource-group $resourceGroup \
  --name $vmName \
  --port 80
```


# Open port 3389 to allow RDP traffic (If you didn't create NSG rules for RDP)
```plaintext
az vm open-port \
  --resource-group $resourceGroup \
  --name $vmName \
  --port 3389
```

Summary of Command Execution Order:
1. Create Resource Group
2. Create VNet and Subnet
3. Create NSG
4. Add NSG Rules for RDP and HTTP
5. Create Public IP
6. Create NIC and associate it with VNet, Subnet, NSG, and Public IP
7. Create Virtual Machine
8. Open Ports (if needed)

This setup ensures all required components for the VM are created and properly associated. After completing these steps, you can connect to the VM via RDP and access the HTTP server.