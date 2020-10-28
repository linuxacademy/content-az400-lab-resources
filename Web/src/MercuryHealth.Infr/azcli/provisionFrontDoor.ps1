# This IaC script provisions and configures the front door instance needed by 
# The Urlist.
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
    $azureSubscriptionName,

    [Parameter(Mandatory = $True)]
    [string]
    $frontDoorName,

    [Parameter(Mandatory = $True)]
    [string]
    $resourceGroupName,

    [Parameter(Mandatory = $True)]
    [string]
    $agentReleaseDirectory,

    [Parameter(Mandatory = $True)]
    [string]
    $webAppName,

    [Parameter(Mandatory = $True)]
    [string]
    $backendFunctionName,

    [Parameter(Mandatory = $True)]
    [string]
    $dnsName,

    [Parameter(Mandatory = $True)]
    [string]
    $friendlyDnsName
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

# This sets the subscription to the subscription I need all my apps to
# run in
#
Write-Output "Setting default azure subscription..."
az account set `
    --subscription $azureSubscriptionName
Write-Output "Done"
Write-Output ""
#endregion
# this defines my time 1 up function which will deploy and configure the infrastructure 
# for Front Door using an ARM template
#
function 1_Up {
    # this addes the front door extension to the azure cli. It's currently in preview
    # hopefully i can remove this soon
    #
    az extension add `
        --name front-door

    # this creates front door from an arm template. Ironically, there are some stuff in front door
    # that can't be configured by the Azure CLI at this moment. 
    # 
    Write-Output "creating Front Door: $frontDoorName..."
    $fqdnWebApp = $webAppName + ".azurewebsites.net"
    Write-Output "fqdn of webapp: $fqdnWebApp"
    az network front-door create `
        --backend-address $fqdnWebApp `
        --name $frontDoorName `
        --resource-group $resourceGroupName
    Write-Output "Done creating Front Door"
    Write-Output ""
}

Install-Module -Name VersionInfrastructure -Force -Scope CurrentUser
Update-InfrastructureVersion `
    -infraToolsFunctionName $Env:INFRATOOLS_FUNCTIONNAME `
    -infraToolsTableName $Env:INFRATOOLS_TABLENAME `
    -deploymentStage $Env:INFRATOOLS_DEPLOYMENTSTAGE