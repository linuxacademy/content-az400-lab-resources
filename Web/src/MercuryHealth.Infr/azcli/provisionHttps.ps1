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
    $azureSubscriptionName,

    [Parameter(Mandatory = $True)]
    [string]
    $resourceGroupName,

    [Parameter(Mandatory = $True)]
    [string]
    $resourceGroupNameRegion,

    [Parameter(Mandatory = $True)]  
    [string]
    $webAppName,

    [Parameter(Mandatory = $True)]  
    [string]
    $dnsName,

    [Parameter(Mandatory = $True)]  
    [string]
    $certificateThumbprint,

    [Parameter(Mandatory = $True)]  
    [string]
    $pfx,

    [Parameter(Mandatory = $True)]  
    [string]
    $pfxPassword
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



# this defines my time 1 up function which will configure https for my apps front door
#
function 1_Up {
    
    Write-Output "getting dev certificate..."
    $kvSecretBytes = [System.Convert]::FromBase64String($pfx)
    $certCollection = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2Collection
    $certCollection.Import($kvSecretBytes,$null,[System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::Exportable)
    Write-Output "done getting dev certificate"
    Write-Output ""

    Write-Output "saving pfx to disk"
    $password = $pfxPassword
    $protectedCertificateBytes = $certCollection.Export([System.Security.Cryptography.X509Certificates.X509ContentType]::Pkcs12, $password)
    $pfxPath = [Environment]::GetFolderPath("Desktop") + "\MyCert.pfx"
    [System.IO.File]::WriteAllBytes($pfxPath, $protectedCertificateBytes)
    Write-Output "done saving pfx to disk"
    Write-Output ""

    Write-Output "uploading certificate, getting thumbprint"
    $thumbprint=$(az webapp config ssl upload `
    --name $webAppName `
    --resource-group $resourceGroupName `
    --certificate-file $pfxPath `
    --certificate-password $pfxPassword `
    --query thumbprint `
    --output tsv)
    Write-Output "done uploading certificate, thumbprint: $thumbprint"
    Write-Output ""


    Write-Output " adding custom domain and adding certificate "
    az webapp config hostname add `
        --webapp-name $webAppName `
        --resource-group $resourceGroupName `
        --hostname $dnsName
    
    az webapp config ssl bind `
        --name $webAppName `
        --resource-group $resourceGroupName `
        --certificate-thumbprint $thumbprint `
        --ssl-type SNI

    Write-Output "Done with function 1_Up"
    Write-Output ""
}



Install-Module -Name VersionInfrastructure -Force -Scope CurrentUser
Update-InfrastructureVersion `
    -infraToolsFunctionName $Env:INFRATOOLS_FUNCTIONNAME `
    -infraToolsTableName $Env:INFRATOOLS_TABLENAME `
    -deploymentStage $Env:INFRATOOLS_DEPLOYMENTSTAGE