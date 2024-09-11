#### Here is a step-by-step guide for creating an Ubuntu virtual machine using Azure CLI with the Standard_D2s_v3 SKU, opening port 80, and enabling HTTP server access:



#### 1. Define variables for the deployment
```plaintext
location="eastus"  # Set your desired location
resourceGroup="resourcegroup123"
vmName="MyUbuntuVM"
image="UbuntuLTS"
adminUser="azureuser"
vmSize="Standard_D2s_v3"
osDiskSku="Standard_LRS"  # Specify Standard HDD for the OS disk
```


#### 2. Create the resource group if it doesn't exist
```plaintext
az group create --name $resourceGroup --location $location
```


#### 3. Create the virtual machine
```plaintext
az vm create \
  --resource-group $resourceGroup \
  --name $vmName \
  --image $image \
  --size $vmSize \
  --admin-username $adminUser \
  --generate-ssh-keys \
  --location $location \
  --storage-sku $osDiskSku
```


#### 4. Open port 80 to allow HTTP traffic and 3389 for RDP access
```plaintext
# Open port 80 to allow HTTP traffic
az vm open-port \
  --port 80 \
  --resource-group $resourceGroup \
  --name $vmName
  
# Open port 3389 to allow RDP traffic
az vm open-port \
  --port 3389 \
  --resource-group $resourceGroup \
  --name $vmName
```


#### 5. SSH into the virtual machine
```plaintext
vmPublicIP=$(az vm show -d --resource-group $resourceGroup --name $vmName --query publicIps -o tsv)
ssh $adminUser@$vmPublicIP
```


#### 6. Once logged in, install the Apache HTTP server
```plaintext
sudo apt update
sudo apt install apache2 -y
```


#### 7. Enable the Apache service
```plaintext
sudo systemctl enable apache2
sudo systemctl start apache2
```

#### 8.Test the HTTP Server
After completing the steps, you can access the HTTP server in your browser by
navigating to
```plaintext
http://<VM_PUBLIC_IP> where <VM_PUBLIC_IP>
```
 is the public IP of your virtual machine ($vmPublicIP).