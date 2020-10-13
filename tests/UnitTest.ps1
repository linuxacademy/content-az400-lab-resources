param (
    $TemplatePath
)

$template = Get-Content $TemplatePath | convertfrom-json
$nsg = $template.resources | Where-Object type -eq 'Microsoft.Network/networkSecurityGroups'

Describe 'NSG Validation' {
	Context 'Check NSG Rules' {
        It "Verify SSH is not open to the internet" {
            $nsg.properties.securityRules.properties.destinationPortRange | Should -Not -Be 22            
        }
        It "Verify RDP is not open to the internet" {
          $nsg.properties.securityRules.properties.destinationPortRange | Should -Not -Be 3389          
      }        
    }
}
