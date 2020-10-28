# This IaC script provisions and configures the web stite hosted in azure
# storage, the back end function and the cosmos db
#
[CmdletBinding()]
param(
    [Parameter(Mandatory = $True)]
    [string]
    $servicePrincipal,

    [Parameter(Mandatory = $True)]
    [string]
    $servicePrincipalSecret,

    [Parameter(Mandatory = $True)]
    [string]
    $servicePrincipalTenantId,

    [Parameter(Mandatory = $True)]
    [string]
    $resourceGroupName,

    [Parameter(Mandatory = $True)]
    [string]
    $resourceGroupNameRegion,

    [Parameter(Mandatory = $True)]  
    [string]
    $serverName,

    [Parameter(Mandatory = $True)]  
    [string]
    $dbLocation,

    [Parameter(Mandatory = $True)]  
    [string]
    $adminLogin,

    [Parameter(Mandatory = $True)]  
    [string]
    $adminPassword,
    
    [Parameter(Mandatory = $True)]  
    [string]
    $startip,

    [Parameter(Mandatory = $True)]  
    [string]
    $endip,

    [Parameter(Mandatory = $True)]  
    [string]
    $dbName,

    [Parameter(Mandatory = $True)]
    [string]
    $dbBackupName
)


#region Login
# This logs in a service principal
#
Write-Output "Logging in to Azure with a service principal..."
az login `
    --service-principal `
    --username $servicePrincipal `
    --password $servicePrincipalSecret `
    --tenant $servicePrincipalTenantId
Write-Output "Done"
Write-Output ""



# this defines my time 1 up function which will deploy and configure the infrastructure 
# for my sql server
function 1_Up {
    Write-Output "In function 1_Up"

    #region Create Resource Group
    # # This creates the resource group used to house all of Mercury Health
    # #
    Write-Output "Creating resource group $resourceGroupName in region $resourceGroupNameRegion..."
    az group create `
        --name $resourceGroupName `
        --location $resourceGroupNameRegion
    Write-Output "Done creating resource group"
    Write-Output ""
    #endregion


    #region Create Sql Server
    # Create a logical sql server in the resource group
    # 
    Write-Output "Creating sql server..."
    az sql server create `
        --name $serverName `
        --resource-group $resourceGroupName `
        --location $dbLocation  `
        --admin-user $adminLogin `
        --admin-password $adminPassword
    Write-Output "Done creating sql server"
    Write-Output ""

    # Configure a firewall rule for the server
    #
    Write-Output "Creating firewall rule for sql server..."
    az sql server firewall-rule create `
        --resource-group $resourceGroupName `
        --server $serverName `
        -n AllowYourIp `
        --start-ip-address $startip `
        --end-ip-address $endip 
    Write-Output "Done creating firewall rule for sql server"
    Write-Output ""
    #endregion
        
    Write-Output "Done with function 1_Up"
    Write-Output ""
}



# this defines my time 2 up fuction which will set up and restore database from backups
function 2_UP {
    Write-Output "In function 2_Up"
    Write-Output ""

    Write-Output "Downloading bacpac..."
    Write-Output ""
    $source = "https://mhlongtermstorage.blob.core.windows.net/backups/$dbBackupName"
    $filename = [System.IO.Path]::GetFileName($source)
    $dest = "./$filename"
    Write-Output "    destination filename: $dest"
    $wc = New-Object System.Net.WebClient
    $wc.DownloadFile($source, $dest)
    Write-Output "Done downloading backpac"
    Write-Output ""

    Write-Output "Restoring DB from bacpac.."
    Write-Output ""
    & "c:\Program Files\Microsoft SQL Server\150\DAC\bin\SqlPackage.exe" `
        /a:import `
        /tcs:"Data Source=$serverName.database.windows.net;Initial Catalog=$dbName;User Id=$adminLogin;Password=$adminPassword" `
        /sf:$filename `
        /p:DatabaseEdition=Premium `
        /p:DatabaseServiceObjective=P6
    Write-Output "Done restoring DB from backpac"
    Write-Output ""

    Write-Output "Done with function 2_Up"
    Write-Output ""
}



Install-Module -Name VersionInfrastructure -Force -Scope CurrentUser
Update-InfrastructureVersion `
    -infraToolsFunctionName $Env:INFRATOOLS_FUNCTIONNAME `
    -infraToolsTableName $Env:INFRATOOLS_TABLENAME `
    -deploymentStage $Env:INFRATOOLS_DEPLOYMENTSTAGE
