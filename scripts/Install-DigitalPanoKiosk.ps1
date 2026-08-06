[CmdletBinding(SupportsShouldProcess = $true, ConfirmImpact = 'Medium')]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^https?://')]
    [string]$PanoUrl,

    [switch]$DisableSleep
)

$ErrorActionPreference = 'Stop'
$taskName = 'DigitalPano-Kiosk'
$sourceLauncher = Join-Path $PSScriptRoot 'Start-DigitalPanoKiosk.ps1'
$installRoot = Join-Path $env:LOCALAPPDATA 'DigitalPanoKiosk'
$installedLauncher = Join-Path $installRoot 'Start-DigitalPanoKiosk.ps1'
$stopFile = Join-Path $installRoot 'bakim-modu.stop'

if (-not (Test-Path -LiteralPath $sourceLauncher)) {
    throw "Kiosk başlatıcısı bulunamadı: $sourceLauncher"
}

if ($PanoUrl -notmatch '[?&]key=') {
    Write-Warning 'Adreste cihaz anahtarı (key) görünmüyor. Yönetim panelindeki Ekranlar > Kopyala adresini kullanın.'
}

if ($PSCmdlet.ShouldProcess($installRoot, 'DigitalPano kiosk dosyalarını kur ve oturum açma görevini oluştur')) {
    New-Item -ItemType Directory -Force -Path $installRoot | Out-Null
    Copy-Item -LiteralPath $sourceLauncher -Destination $installedLauncher -Force
    Remove-Item -LiteralPath $stopFile -Force -ErrorAction SilentlyContinue

    $encodedUrl = $PanoUrl.Replace('"', '\"')
    $taskArguments = "-NoLogo -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File `"$installedLauncher`" -PanoUrl `"$encodedUrl`""
    $action = New-ScheduledTaskAction -Execute 'powershell.exe' -Argument $taskArguments
    $trigger = New-ScheduledTaskTrigger -AtLogOn -User "$env:USERDOMAIN\$env:USERNAME"
    $principal = New-ScheduledTaskPrincipal -UserId "$env:USERDOMAIN\$env:USERNAME" -LogonType Interactive -RunLevel Limited
    $settings = New-ScheduledTaskSettingsSet -ExecutionTimeLimit ([TimeSpan]::Zero) -RestartCount 10 -RestartInterval (New-TimeSpan -Minutes 1) -AllowStartIfOnBatteries -DontStopIfGoingOnBatteries

    Register-ScheduledTask -TaskName $taskName -Action $action -Trigger $trigger -Principal $principal -Settings $settings -Description 'DigitalPano Edge kiosk otomatik başlangıcı' -Force | Out-Null

    if ($DisableSleep) {
        powercfg.exe /change monitor-timeout-ac 0
        powercfg.exe /change standby-timeout-ac 0
    }

    Start-ScheduledTask -TaskName $taskName
    Write-Host 'DigitalPano kiosk kuruldu ve başlatıldı.'
    Write-Host "Görev: $taskName"
    Write-Host "Adres: $PanoUrl"
}
