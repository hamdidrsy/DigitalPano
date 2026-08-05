[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^https?://')]
    [string]$PanoUrl,

    [ValidateRange(0.001, 168)]
    [double]$DurationHours = 72,

    [ValidateRange(1, 3600)]
    [int]$IntervalSeconds = 60,

    [ValidateRange(0, 100000)]
    [int]$MaxSamples = 0,

    [string]$OutputDirectory = 'artifacts/endurance'
)

$ErrorActionPreference = 'Stop'
$resolvedOutput = [System.IO.Path]::GetFullPath((Join-Path (Get-Location) $OutputDirectory))
New-Item -ItemType Directory -Force -Path $resolvedOutput | Out-Null

$runId = Get-Date -Format 'yyyyMMdd-HHmmss'
$csvPath = Join-Path $resolvedOutput "pano-72h-$runId.csv"
$summaryPath = Join-Path $resolvedOutput "pano-72h-$runId-summary.json"
$startedAt = Get-Date
$finishAt = $startedAt.AddHours($DurationHours)
$samples = [System.Collections.Generic.List[object]]::new()
$consecutiveFailures = 0
$maximumConsecutiveFailures = 0

Write-Host "DigitalPano dayanıklılık testi başladı."
Write-Host "Adres: $PanoUrl"
Write-Host "Bitiş: $($finishAt.ToString('yyyy-MM-dd HH:mm:ss'))"
Write-Host "Kayıt: $csvPath"

while ((Get-Date) -lt $finishAt -and ($MaxSamples -eq 0 -or $samples.Count -lt $MaxSamples)) {
    $sampleTime = Get-Date
    $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
    $success = $false
    $statusCode = $null
    $hasPanoRoot = $false
    $errorMessage = $null

    try {
        $response = Invoke-WebRequest -Uri $PanoUrl -Method Get -TimeoutSec 15 -MaximumRedirection 5 -UseBasicParsing
        $statusCode = [int]$response.StatusCode
        $hasPanoRoot = $response.Content -match 'data-pano-root'
        $success = $statusCode -eq 200 -and $hasPanoRoot
        if (-not $hasPanoRoot) { $errorMessage = 'Yanıt pano kök öğesini içermiyor.' }
    }
    catch {
        if ($_.Exception.Response -and $_.Exception.Response.StatusCode) {
            $statusCode = [int]$_.Exception.Response.StatusCode
        }
        $errorMessage = $_.Exception.Message
    }
    finally {
        $stopwatch.Stop()
    }

    if ($success) {
        $consecutiveFailures = 0
    }
    else {
        $consecutiveFailures++
        $maximumConsecutiveFailures = [Math]::Max($maximumConsecutiveFailures, $consecutiveFailures)
    }

    $sample = [pscustomobject]@{
        Timestamp = $sampleTime.ToString('o')
        Success = $success
        StatusCode = $statusCode
        HasPanoRoot = $hasPanoRoot
        ResponseMilliseconds = $stopwatch.ElapsedMilliseconds
        ConsecutiveFailures = $consecutiveFailures
        Error = $errorMessage
    }
    $samples.Add($sample)
    $sample | Export-Csv -Path $csvPath -NoTypeInformation -Encoding utf8 -Append

    $state = if ($success) { 'OK' } else { 'HATA' }
    Write-Host "[$($sampleTime.ToString('yyyy-MM-dd HH:mm:ss'))] $state - $($stopwatch.ElapsedMilliseconds) ms"

    if ((Get-Date) -lt $finishAt -and ($MaxSamples -eq 0 -or $samples.Count -lt $MaxSamples)) {
        Start-Sleep -Seconds $IntervalSeconds
    }
}

$completedAt = Get-Date
$successfulSamples = @($samples | Where-Object Success)
$latencies = @($successfulSamples | ForEach-Object ResponseMilliseconds | Sort-Object)
$successRate = if ($samples.Count -gt 0) { 100 * $successfulSamples.Count / $samples.Count } else { 0 }
$p95Index = if ($latencies.Count -gt 0) { [Math]::Min($latencies.Count - 1, [Math]::Ceiling($latencies.Count * 0.95) - 1) } else { 0 }
$p95 = if ($latencies.Count -gt 0) { $latencies[$p95Index] } else { $null }
$maximumFailureMinutes = $maximumConsecutiveFailures * $IntervalSeconds / 60
$isFullDuration = $MaxSamples -eq 0 -and $completedAt -ge $finishAt
$passed = $isFullDuration -and $successRate -ge 99.5 -and $maximumFailureMinutes -le 5 -and $null -ne $p95 -and $p95 -le 2000

$summary = [ordered]@{
    RunId = $runId
    PanoUrl = $PanoUrl
    StartedAt = $startedAt.ToString('o')
    CompletedAt = $completedAt.ToString('o')
    RequestedDurationHours = $DurationHours
    FullDurationCompleted = $isFullDuration
    TotalSamples = $samples.Count
    SuccessfulSamples = $successfulSamples.Count
    FailedSamples = $samples.Count - $successfulSamples.Count
    SuccessRatePercent = [Math]::Round($successRate, 3)
    P95ResponseMilliseconds = $p95
    MaximumConsecutiveFailures = $maximumConsecutiveFailures
    MaximumFailureMinutes = $maximumFailureMinutes
    AcceptancePassed = $passed
    CsvPath = $csvPath
}
$summary | ConvertTo-Json | Set-Content -Path $summaryPath -Encoding utf8

Write-Host "Test sona erdi. Başarı oranı: $([Math]::Round($successRate, 3))%"
Write-Host "Özet: $summaryPath"
if (-not $isFullDuration) { Write-Warning 'Bu çalışma kısa doğrulama modunda tamamlandı; 72 saat kabul testi sayılmaz.' }
if ($isFullDuration -and -not $passed) { Write-Warning '72 saat tamamlandı ancak kabul ölçütlerinden en az biri sağlanmadı.' }
if ($passed) { Write-Host '72 saat otomatik erişim ölçütleri başarıyla karşılandı.' -ForegroundColor Green }
