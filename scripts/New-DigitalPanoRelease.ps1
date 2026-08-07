[CmdletBinding()]
param(
    [string]$OutputDirectory = 'artifacts/release',
    [string]$Version = (Get-Date -Format 'yyyy.MM.dd-HHmm'),
    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'
$workspace = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
if ([string]::IsNullOrWhiteSpace($env:DOTNET_CLI_HOME)) {
    $env:DOTNET_CLI_HOME = Join-Path $workspace '.dotnet-home'
}
if ([string]::IsNullOrWhiteSpace($env:NUGET_PACKAGES)) {
    $env:NUGET_PACKAGES = Join-Path $env:DOTNET_CLI_HOME '.nuget\packages'
}
$outputRoot = [System.IO.Path]::GetFullPath((Join-Path $workspace $OutputDirectory))
$requiredPrefix = [System.IO.Path]::GetFullPath((Join-Path $workspace 'artifacts')) + [System.IO.Path]::DirectorySeparatorChar
if (-not $outputRoot.StartsWith($requiredPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw 'Çıktı klasörü çalışma alanındaki artifacts klasörü altında olmalıdır.'
}

if (Test-Path -LiteralPath $outputRoot) {
    Remove-Item -LiteralPath $outputRoot -Recurse -Force
}
$appDirectory = Join-Path $outputRoot 'app'
$databaseDirectory = Join-Path $outputRoot 'database'
New-Item -ItemType Directory -Force -Path $appDirectory, $databaseDirectory | Out-Null

Push-Location $workspace
try {
    if (-not $SkipTests) {
        dotnet test DigitalPano.sln --configuration Release
        if ($LASTEXITCODE -ne 0) { throw 'Test paketi başarısız oldu; yayın durduruldu.' }
    }

    dotnet tool restore
    if ($LASTEXITCODE -ne 0) { throw 'Yerel .NET araçları geri yüklenemedi.' }

    dotnet publish src\DigitalPano.Web\DigitalPano.Web.csproj `
        --configuration Release --self-contained false `
        --output $appDirectory /p:Version=$Version
    if ($LASTEXITCODE -ne 0) { throw 'dotnet publish başarısız oldu.' }

    $previousEnvironment = $env:ASPNETCORE_ENVIRONMENT
    try {
        $env:ASPNETCORE_ENVIRONMENT = 'Development'
        dotnet tool run dotnet-ef migrations script --idempotent `
            --project src\DigitalPano.Web --startup-project src\DigitalPano.Web `
            --output (Join-Path $databaseDirectory 'DigitalPano-migrate-idempotent.sql')
        if ($LASTEXITCODE -ne 0) { throw 'Idempotent migration SQL betiği üretilemedi.' }
    }
    finally {
        $env:ASPNETCORE_ENVIRONMENT = $previousEnvironment
    }

    Copy-Item son_adım.md (Join-Path $outputRoot 'KURULUM.md')
    Copy-Item Dockerfile, render.yaml $outputRoot

    $manifest = Get-ChildItem $outputRoot -Recurse -File | ForEach-Object {
        [pscustomobject]@{
            Path = $_.FullName.Substring($outputRoot.Length + 1).Replace('\', '/')
            Length = $_.Length
            Sha256 = (Get-FileHash $_.FullName -Algorithm SHA256).Hash
        }
    }
    $manifest | ConvertTo-Json | Set-Content (Join-Path $outputRoot 'manifest.json') -Encoding utf8

    $zipPath = Join-Path ([System.IO.Path]::GetDirectoryName($outputRoot)) "DigitalPano-$Version-render.zip"
    if (Test-Path -LiteralPath $zipPath) { Remove-Item -LiteralPath $zipPath -Force }
    Compress-Archive -Path (Join-Path $outputRoot '*') -DestinationPath $zipPath -CompressionLevel Optimal
    Write-Host "Yayın paketi hazır: $zipPath" -ForegroundColor Green
}
finally {
    Pop-Location
}
