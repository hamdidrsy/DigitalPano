[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)] [string]$SitePath,
    [Parameter(Mandatory = $true)] [string]$DataPath,
    [string]$AppPoolName = 'DigitalPano'
)

$ErrorActionPreference = 'Stop'
$resolvedSite = [System.IO.Path]::GetFullPath($SitePath)
$resolvedData = [System.IO.Path]::GetFullPath($DataPath)
if ($resolvedSite -eq [System.IO.Path]::GetPathRoot($resolvedSite) -or
    $resolvedData -eq [System.IO.Path]::GetPathRoot($resolvedData)) {
    throw 'Kök disk klasörüne izin uygulanamaz.'
}

New-Item -ItemType Directory -Force -Path $resolvedSite, $resolvedData, (Join-Path $resolvedData 'media') | Out-Null
$identity = "IIS AppPool\$AppPoolName"

& icacls.exe $resolvedSite /inheritance:r /grant:r "Administrators:(OI)(CI)F" "SYSTEM:(OI)(CI)F" "${identity}:(OI)(CI)RX"
if ($LASTEXITCODE -ne 0) { throw 'Uygulama klasörü izinleri uygulanamadı.' }

& icacls.exe $resolvedData /inheritance:r /grant:r "Administrators:(OI)(CI)F" "SYSTEM:(OI)(CI)F" "${identity}:(OI)(CI)M"
if ($LASTEXITCODE -ne 0) { throw 'Veri klasörü izinleri uygulanamadı.' }

Write-Host 'Klasör izinleri uygulandı.' -ForegroundColor Green
