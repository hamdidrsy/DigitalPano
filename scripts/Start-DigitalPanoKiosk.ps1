[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^https?://')]
    [string]$PanoUrl,

    [ValidateRange(5, 300)]
    [int]$RestartDelaySeconds = 10
)

$ErrorActionPreference = 'Stop'
$edgeCandidates = @(
    (Join-Path ${env:ProgramFiles(x86)} 'Microsoft\Edge\Application\msedge.exe'),
    (Join-Path $env:ProgramFiles 'Microsoft\Edge\Application\msedge.exe')
) | Where-Object { $_ -and (Test-Path -LiteralPath $_) }

if ($edgeCandidates.Count -eq 0) {
    throw 'Microsoft Edge bulunamadı. Edge kurulmalı veya onarılmalıdır.'
}

$edgePath = $edgeCandidates[0]
$kioskRoot = Join-Path $env:LOCALAPPDATA 'DigitalPanoKiosk'
$profilePath = Join-Path $kioskRoot 'EdgeProfile'
$stopFile = Join-Path $kioskRoot 'bakim-modu.stop'
New-Item -ItemType Directory -Force -Path $profilePath | Out-Null

while (-not (Test-Path -LiteralPath $stopFile)) {
    $arguments = @(
        '--kiosk',
        $PanoUrl,
        '--edge-kiosk-type=fullscreen',
        '--no-first-run',
        '--disable-features=msEdgeFirstRunExperience',
        '--disable-session-crashed-bubble',
        "--user-data-dir=$profilePath"
    )

    try {
        $edge = Start-Process -FilePath $edgePath -ArgumentList $arguments -PassThru
        $edge.WaitForExit()
    }
    catch {
        Write-Error $_ -ErrorAction Continue
    }

    if (-not (Test-Path -LiteralPath $stopFile)) {
        Start-Sleep -Seconds $RestartDelaySeconds
    }
}
