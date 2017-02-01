#
# DeployDocDbCollection.ps1
#
# Create the Document DB database and collection
Param(
    [string] [Parameter(Mandatory=$true)] $ResourceGroupName
)

#get the most recent deployment for the resource group
$lastRgDeployment = Get-AzureRmResourceGroupDeployment -ResourceGroupName $ResourceGroupName |
	Sort Timestamp -Descending |
		Select -First 1        

if(!$lastRgDeployment)
{
	throw "Resource Group Deployment could not be found for '$ResourceGroupName'."
}

$deploymentOutputParameters = $lastRgDeployment.Outputs

if(!$deploymentOutputParameters)
{
	throw "No output parameters could be found for the last deployment of '$ResourceGroupName'."
}

$azDocDbServerParam = $deploymentOutputParameters.Item("docDbName")
$azDocDbServer = $azDocDbServerParam.Value
	
$azDocDbKeyParam = $deploymentOutputParameters.Item("docDbKey")
$azDocDbKey = $azDocDbKeyParam.Value

$azStorageAccountNameParam = $deploymentOutputParameters.Item("storageName")
$azStorageAccountName = $azStorageAccountNameParam.Value

$PSScriptRoot
$docDbPSModulePath = join-path $PSScriptRoot -childpath "..\..\..\AzureKit.PowerShell.dll"

if ([System.IO.File]::Exists($docDbPSModulePath))
{
    $docDbPSModulePath = resolve-path (join-path $PSScriptRoot -childpath "..\..\..\AzureKit.PowerShell.dll")
}
else
{
    $docDbPSModulePath = join-path $PSScriptRoot -childpath "..\AzureKit.PowerShell.dll"
    if ([System.IO.File]::Exists($docDbPSModulePath))
    {
        $docDbPSModulePath = resolve-path (join-path $PSScriptRoot -childpath "..\AzureKit.PowerShell.dll")
    }
    else
    {
	    Write-Host "Can't find module!"
	    exit
    }
}

$docDbPSModulePath
import-module $docDbPSModulePath

New-DocDbDatabaseAndCollection -ServerName $azDocDBServer -PrimaryKey $azDocDbKey -DatabaseName "AzureKit" -CollectionName "SiteContent"

# Create the Azure Storage Blob Container and Set CORS rules
$azStorageAccountContext = (Get-AzureRmStorageAccount -ResourceGroupName $ResourceGroupName -Name $azStorageAccountName).Context

# try to get the container first before creating
$existingStorageContainer = (Get-AzureStorageContainer -Name images -Context $azStorageAccountContext -ErrorAction SilentlyContinue)

if($existingStorageContainer -eq $null) {
	New-AzureStorageContainer -Name images -Permission Container -Context $azStorageAccountContext
}
$CorsRules = (@{AllowedHeaders=@("*"); `
	AllowedOrigins=@("*"); `
	MaxAgeInSeconds=1200; `
	AllowedMethods=@("DELETE","GET","NONE","POST","PUT")})

Set-AzureStorageCORSRule -ServiceType Blob -CorsRules $CorsRules -Context $azStorageAccountContext