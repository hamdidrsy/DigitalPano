[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string]$WebConfigPath,
    [Parameter(Mandatory = $true)] [string]$SqlServer,
    [string]$DatabaseName = 'DigitalPano',
    [Parameter(Mandatory = $true)] [string]$HostName,
    [Parameter(Mandatory = $true)] [string]$MediaPath
)

$ErrorActionPreference = 'Stop'
$resolvedConfig = (Resolve-Path $WebConfigPath).Path
if ([System.IO.Path]::GetFileName($resolvedConfig) -ne 'web.config') {
    throw 'Yalnız web.config dosyası yapılandırılabilir.'
}
if (-not [System.IO.Path]::IsPathRooted($MediaPath)) {
    throw 'MediaPath mutlak bir yol olmalıdır.'
}

[xml]$config = Get-Content $resolvedConfig
$aspNetCore = $config.configuration.location.'system.webServer'.aspNetCore
if ($null -eq $aspNetCore) { throw 'Yayın web.config dosyasında aspNetCore öğesi bulunamadı.' }

$environmentVariables = $aspNetCore.environmentVariables
if ($null -eq $environmentVariables) {
    $environmentVariables = $config.CreateElement('environmentVariables')
    [void]$aspNetCore.AppendChild($environmentVariables)
}

function Set-EnvironmentVariableNode([string]$Name, [string]$Value) {
    $node = @($environmentVariables.environmentVariable) | Where-Object { $_.name -eq $Name } | Select-Object -First 1
    if ($null -eq $node) {
        $node = $config.CreateElement('environmentVariable')
        [void]$environmentVariables.AppendChild($node)
    }
    $node.SetAttribute('name', $Name)
    $node.SetAttribute('value', $Value)
}

$connectionString = "Server=$SqlServer;Database=$DatabaseName;Integrated Security=True;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=True"
Set-EnvironmentVariableNode 'ASPNETCORE_ENVIRONMENT' 'Production'
Set-EnvironmentVariableNode 'ConnectionStrings__DefaultConnection' $connectionString
Set-EnvironmentVariableNode 'MediaStorage__RootPath' ([System.IO.Path]::GetFullPath($MediaPath))
Set-EnvironmentVariableNode 'AllowedHosts' $HostName

$settings = [System.Xml.XmlWriterSettings]::new()
$settings.Indent = $true
$settings.Encoding = [System.Text.UTF8Encoding]::new($false)
$writer = [System.Xml.XmlWriter]::Create($resolvedConfig, $settings)
try { $config.Save($writer) } finally { $writer.Dispose() }
Write-Host "web.config Production değerleriyle güncellendi: $resolvedConfig" -ForegroundColor Green
