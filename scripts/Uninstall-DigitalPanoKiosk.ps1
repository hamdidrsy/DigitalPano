[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Medium')]
param()

$ErrorActionPreference = 'Stop'
$taskName = 'DigitalPano-Kiosk'
$installRoot = Join-Path $env:LOCALAPPDATA 'DigitalPanoKiosk'
$stopFile = Join-Path $installRoot 'bakim-modu.stop'

if ($PSCmdlet.ShouldProcess($taskName, 'DigitalPano kiosk otomatik başlangıcını kaldır')) {
    New-Item -ItemType Directory -Force -Path $installRoot | Out-Null
    New-Item -ItemType File -Force -Path $stopFile | Out-Null
    Stop-ScheduledTask -TaskName $taskName -ErrorAction SilentlyContinue
    Unregister-ScheduledTask -TaskName $taskName -Confirm:$false -ErrorAction SilentlyContinue

    Get-CimInstance Win32_Process -Filter "Name = 'msedge.exe'" |
        Where-Object { $_.CommandLine -like "*$installRoot*" } |
        ForEach-Object { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue }

    Write-Host 'DigitalPano kiosk otomatik başlangıcı kaldırıldı.'
    Write-Host "Profil verileri gerektiğinde elle silinebilir: $installRoot"
}
