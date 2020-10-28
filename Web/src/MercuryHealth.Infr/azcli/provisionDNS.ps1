# This IaC script provisions and configures DNS to the application
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
    $cloudFlareKey,

    [Parameter(Mandatory = $True)]
    [string]
    $cloudFlareEmail,

    [Parameter(Mandatory = $True)]
    [string]
    $cloudFlareZone,

    [Parameter(Mandatory = $True)]
    [string]
    $dnsName,
    
    [Parameter(Mandatory = $True)]
    [string]
    $frontDoorName,

    [Parameter(Mandatory = $True)]
    [string]
    $nakedDns
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
# for my DNS settings up in cloud flare
#
function 1_Up {
    Write-Output "executing function 1_Up..."

    #region get all dns records from cloudflare
    # this lists all dns records from cloudflare
    #
    Write-Output "getting all dns records from cloudflare..."
    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("X-Auth-Key", $cloudFlareKey)
    $headers.Add("X-Auth-Email", $cloudFlareEmail)
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls -bor [Net.SecurityProtocolType]::Tls11 -bor [Net.SecurityProtocolType]::Tls12
    $listDnsResult=Invoke-RestMethod "https://api.cloudflare.com/client/v4/zones/$cloudFlareZone/dns_records" `
        -Headers $headers
    Write-Output $listDnsResult
    Write-Output "done getting all dns records"
    $numEntries=$listDnsResult.result_info.count
    Write-Output "number of dns entries: $numEntries" 
    Write-Output ""
    #endregion

    #region look at all dns records, see if the our dns name has already 
    # been set. This block looks for our dns name, see if it has been set or not
    #
    Write-Output "looking for correct DNS entry"
    $foundDnsEntry = $false
    $foundDnsEntryId = "x"
    $listDnsResult.result | ForEach-Object {
        $dnsEntryName = $_.name
        Write-Output "dns entry name: $dnsEntryName"
        if ($dnsEntryName -eq $dnsName) {
            Write-Output "found correct dns entry"
            $foundDnsEntry =$true
            $foundDnsEntryId = $_.id
            return
        }
    }
    Write-Output "found dns entry: $foundDnsEntry"
    Write-Output "dns entry id: $foundDnsEntryId"
    Write-Output ""
    #endregion

    #region updates/adds dns entry to cloudflare
    # this either updates or adds a new dns entry to cloudflare
    #
    $frontDoorFQDN=$frontDoorName + ".azurewebsites.net"
    Write-Output "front door fqdn: $frontDoorFQDN"
    if ($foundDnsEntry -eq $true) {
        Write-Output "updating dns entry..."
        $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
        $headers.Add("X-Auth-Key", $cloudFlareKey)
        $headers.Add("X-Auth-Email", $cloudFlareEmail)
        $updateDnsEntry = @{
            type='CNAME'
            name='www'
            content="$frontDoorFQDN"
            proxied=$false
        }
        $json = $updateDnsEntry | ConvertTo-Json
        $updateDnsResponse = $(Invoke-RestMethod "https://api.cloudflare.com/client/v4/zones/$cloudFlareZone/dns_records/$foundDnsEntryId" `
            -Headers $headers `
            -Method Put `
            -Body $json `
            -ContentType 'application/json')

        Write-Output "done updating dns"
        Write-Output "cloudflare response: "
        Write-Output $updateDnsResponse
        Write-Output ""
    }
    else {
        Write-Output "adding new dns entry..."
        $newDnsResponse = $()
        $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
        $headers.Add("X-Auth-Key", $cloudFlareKey)
        $headers.Add("X-Auth-Email", $cloudFlareEmail)
        $newDnsEntry = @{
            type='CNAME'
            name='www'
            content="$frontDoorFQDN"
            proxied=$false
            priority=10
        }
        $json = $newDnsEntry | ConvertTo-Json
        $newDnsResponse = $(Invoke-RestMethod "https://api.cloudflare.com/client/v4/zones/$cloudFlareZone/dns_records" `
        -Headers $headers `
        -Method Post `
        -Body $json `
        -ContentType 'application/json')

        Write-Output "done adding dns"
        Write-Output "cloudflare response: "
        Write-Output $newDnsResponse
        Write-Output ""
    }
    #endregion

    Write-Output "done with function 1_Up"
    Write-Output ""
}

# This brings my infrastructure up to version 2 where it sets up the apex domain url (no www)
# in dns to point and direct to the right place
#
function 2_Up {
    Write-Output "Executing function 2_Up"

    #region gets all dns entries from cloudflare
    # this lists all dns records from cloudflare
    #
    Write-Output "getting all dns records from cloudflare..."
    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("X-Auth-Key", $cloudFlareKey)
    $headers.Add("X-Auth-Email", $cloudFlareEmail)
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls -bor [Net.SecurityProtocolType]::Tls11 -bor [Net.SecurityProtocolType]::Tls12
    $listDnsResult=Invoke-RestMethod "https://api.cloudflare.com/client/v4/zones/$cloudFlareZone/dns_records" `
        -Headers $headers
    Write-Output $listDnsResult
    Write-Output "done getting all dns records"
    $numEntries=$listDnsResult.result_info.count
    Write-Output "number of dns entries: $numEntries" 
    Write-Output ""
    #endregion

    #region look at all dns records, see if our dns name has already been set
    # this looks for our dns name, see if it has been set or not
    #
    $foundDnsEntry = $false
    $foundDnsEntryId = "x"
    $listDnsResult.result | ForEach-Object {
        $dnsEntryName = $_.name
        if ($dnsEntryName -eq $nakedDns) {
            $foundDnsEntry =$true
            $foundDnsEntryId = $_.id
            return
        }
    }
    Write-Output "found dns entry: $foundDnsEntry"
    Write-Output "dns entry id: $foundDnsEntryId"
    Write-Output ""
    #endregion

    #region update/add  dns entry to cloudflare for apex domain
    # this either updates or adds a new dns entry to cloudflare for
    # the apex domain
    #
    $frontDoorFQDN=$frontDoorName + ".azurewebsites.net"
    if ($foundDnsEntry -eq $true) {
        Write-Output "updating dns entry..."
        $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
        $headers.Add("X-Auth-Key", $cloudFlareKey)
        $headers.Add("X-Auth-Email", $cloudFlareEmail)
        $updateDnsEntry = @{
            type='CNAME'
            name='@'
            content="$frontDoorFQDN"
            proxied=$true
        }
        $json = $updateDnsEntry | ConvertTo-Json
        $updateDnsResponse = $(Invoke-RestMethod "https://api.cloudflare.com/client/v4/zones/$cloudFlareZone/dns_records/$foundDnsEntryId" `
            -Headers $headers `
            -Method Put `
            -Body $json `
            -ContentType 'application/json')

        Write-Output "done updating dns"
        Write-Output "cloudflare response: "
        Write-Output $updateDnsResponse
        Write-Output ""

        Write-Output "done updating dns"
        Write-Output ""
    }
    else {
        Write-Output "adding new dns entry..."
        $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
        $headers.Add("X-Auth-Key", $cloudFlareKey)
        $headers.Add("X-Auth-Email", $cloudFlareEmail)
        $newDnsEntry = @{
            type='CNAME'
            name='@'
            content="$frontDoorFQDN"
            proxied=$true
            priority=10
        }
        $json = $newDnsEntry | ConvertTo-Json
        $newDnsResponse = $(Invoke-RestMethod "https://api.cloudflare.com/client/v4/zones/$cloudFlareZone/dns_records" `
        -Headers $headers `
        -Method Post `
        -Body $json `
        -ContentType 'application/json')

        Write-Output "done adding dns"
        Write-Output "cloudflare response: "
        Write-Output $newDnsResponse
        Write-Output ""
        Write-Output "done adding new dns entry"
        Write-Output ""
    }
    #endregion

    #region check page rules
    # this looks to see if we need to add a page rule for apex domain
    # first by looking up all the rules
    #
    Write-Output "getting all rules from cloudflare..."
    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("X-Auth-Key", $cloudFlareKey)
    $headers.Add("X-Auth-Email", $cloudFlareEmail)
    $headers.Add("Content-Type", "application/json")
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls -bor [Net.SecurityProtocolType]::Tls11 -bor [Net.SecurityProtocolType]::Tls12
    $listRulesResult=Invoke-RestMethod "https://api.cloudflare.com/client/v4/zones/$cloudFlareZone/pagerules?status=active&order=status&direction=desc&match=all" `
        -Headers $headers
    Write-Output $listRulesResult
    Write-Output "done getting all dns records"
    $numEntries=$listRulesResult.result_info.count
    Write-Output "number of dns entries: $numEntries" 
    Write-Output ""
    #endregion
   
    #region delete old page rules
    # delete these old rule entries
    #
    Write-Output "deleting all rule entries..."
    $listRulesResult.result | ForEach-Object {
        $ruleId = $_.id
        Write-Output "deleting rule with id: $ruleId"
        $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
        $headers.Add("X-Auth-Key", $cloudFlareKey)
        $headers.Add("X-Auth-Email", $cloudFlareEmail)
        $deleteResult = Invoke-RestMethod "https://api.cloudflare.com/client/v4/zones/$cloudFlareZone/pagerules/$ruleId" `
            -Headers $headers `
            -Method Delete
        Write-Output "delete response: "
        Write-Output $deleteResult
    }
    Write-Output "done deleting all rule entries"    
    Write-Output ""
    #endregion

    #region add new apex domain rules
    # Add in the apex domain rule
    #
    Write-Output "adding apex domain rule..."
    $json = '{"targets":[{"target":"url", "constraint":{"operator":"matches","value":"' + $nakedDns + '/*"}}],"actions":[{"id":"forwarding_url","value": {"url": "https://' + $dnsName + '/$1","status_code": 301}}],"priority":1,"status":"active"}'
    Write-Output "body: $json"
    $headers = New-Object "System.Collections.Generic.Dictionary[[String],[String]]"
    $headers.Add("X-Auth-Key", $cloudFlareKey)
    $headers.Add("X-Auth-Email", $cloudFlareEmail)
    $headers.Add("Content-Type", "application/json")
    $addRuleResponse = Invoke-RestMethod "https://api.cloudflare.com/client/v4/zones/$cloudFlareZone/pagerules" `
        -Headers $headers `
        -Method Post `
        -Body $json `
        -ContentType 'application/json'
    Write-Output $addRuleResponse
    Write-Output "done adding apex domain rule"
    Write-Output ""
    #endregion

    Write-Output "done with 2_Up"
    Write-Output ""
}


Install-Module -Name VersionInfrastructure -Force -Scope CurrentUser
Update-InfrastructureVersion `
    -infraToolsFunctionName $Env:INFRATOOLS_FUNCTIONNAME `
    -infraToolsTableName $Env:INFRATOOLS_TABLENAME `
    -deploymentStage $Env:INFRATOOLS_DEPLOYMENTSTAGE