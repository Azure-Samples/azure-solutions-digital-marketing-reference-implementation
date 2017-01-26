#
# DeployDocDbCollection.ps1
#
# Create the Document DB database and collection
Param(
    [string] [Parameter(Mandatory=$true)] $ResourceGroupName,
    [string] $azDocDbServer,
    [string] $azDocDbKey,
    [string] $azStorageAccountName
)

#$docDbPSModulePath = resolve-path (join-path $PSScriptRoot -childpath "..\..\..\..\..\..\AzureKit.PowerShell\bin\Debug\AzureKit.PowerShell.dll")
$docDbPSModulePath = resolve-path (join-path $PSScriptRoot -childpath "..\..\..\AzureKit.PowerShell.dll")
import-module $docDbPSModulePath

# $azDocDbServer = $deploymentOutputs.Outputs["docDbName"].Value
# $azDocDbKey =  $deploymentOutputs.Outputs["docDbKey"].Value

New-DocDbDatabaseAndCollection -ServerName $azDocDBServer -PrimaryKey $azDocDbKey -DatabaseName "AzureKit" -CollectionName "SiteContent"

# Create the Azure Storage Blob Container and Set CORS rules
# $azStorageAccountName = $deploymentOutputs.Outputs["storageName"].Value

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