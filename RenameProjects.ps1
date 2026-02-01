# ============================================================================
# Script: RenameProjects.ps1
# Description: ??i tên Solution và Projects t? DoMinhGiaBao_SE1856_A01 sang PRN232.LAB
# Author: GitHub Copilot
# ============================================================================

param(
    [string]$SourcePath = "D:\DoMinhGiaBao_ SE1856_A01_BE",
    [string]$TargetPath = "D:\PRN232.LAB.Solution"
)

# Mapping c? -> m?i
$projectMappings = @{
    "DoMinhGiaBao_ SE1856_A01_BE" = "PRN232.LAB.API"
    "DoMinhGiaBao_SE1856_A01_Repository" = "PRN232.LAB.Repo"
    "DoMinhGiaBao_SE1856_A01_Service" = "PRN232.LAB.Services"
}

$namespaceMappings = @{
    "DoMinhGiaBao_ SE1856_A01_BE" = "PRN232.LAB.API"
    "DoMinhGiaBao_SE1856_A01_BE" = "PRN232.LAB.API"
    "DoMinhGiaBao_SE1856_A01_Repository" = "PRN232.LAB.Repo"
    "DoMinhGiaBao_SE1856_A01_Service" = "PRN232.LAB.Services"
}

$solutionNameOld = "DoMinhGiaBao_SE1856_A01_BE"
$solutionNameNew = "PRN232.LAB.Solution"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RENAME SOLUTION & PROJECTS SCRIPT    " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Step 0: Ki?m tra source path
if (-not (Test-Path $SourcePath)) {
    Write-Host "ERROR: Source path not found: $SourcePath" -ForegroundColor Red
    exit 1
}

Write-Host "[INFO] Source: $SourcePath" -ForegroundColor Yellow
Write-Host "[INFO] Target: $TargetPath" -ForegroundColor Yellow
Write-Host ""

# Step 1: Copy toàn b? folder sang folder m?i
Write-Host "[STEP 1] Copying solution to new location..." -ForegroundColor Green
if (Test-Path $TargetPath) {
    Write-Host "[WARN] Target path exists. Removing..." -ForegroundColor Yellow
    Remove-Item -Path $TargetPath -Recurse -Force
}
Copy-Item -Path $SourcePath -Destination $TargetPath -Recurse
Write-Host "[OK] Copied successfully!" -ForegroundColor Green
Write-Host ""

# Step 2: ??i tên các th? m?c project
Write-Host "[STEP 2] Renaming project folders..." -ForegroundColor Green
foreach ($mapping in $projectMappings.GetEnumerator()) {
    $oldFolder = Join-Path $TargetPath $mapping.Key
    $newFolder = Join-Path $TargetPath $mapping.Value
    
    if (Test-Path $oldFolder) {
        Rename-Item -Path $oldFolder -NewName $mapping.Value
        Write-Host "  Renamed: $($mapping.Key) -> $($mapping.Value)" -ForegroundColor Gray
    }
}
Write-Host "[OK] Folders renamed!" -ForegroundColor Green
Write-Host ""

# Step 3: ??i tên các file .csproj
Write-Host "[STEP 3] Renaming .csproj files..." -ForegroundColor Green
foreach ($mapping in $projectMappings.GetEnumerator()) {
    $projectFolder = Join-Path $TargetPath $mapping.Value
    $oldCsproj = Join-Path $projectFolder "$($mapping.Key).csproj"
    $newCsproj = "$($mapping.Value).csproj"
    
    if (Test-Path $oldCsproj) {
        Rename-Item -Path $oldCsproj -NewName $newCsproj
        Write-Host "  Renamed: $($mapping.Key).csproj -> $newCsproj" -ForegroundColor Gray
    }
}
Write-Host "[OK] .csproj files renamed!" -ForegroundColor Green
Write-Host ""

# Step 4: ??i tên file .sln
Write-Host "[STEP 4] Renaming .sln file..." -ForegroundColor Green
$oldSln = Join-Path $TargetPath "$solutionNameOld.sln"
$newSln = Join-Path $TargetPath "$solutionNameNew.sln"
if (Test-Path $oldSln) {
    Rename-Item -Path $oldSln -NewName "$solutionNameNew.sln"
    Write-Host "  Renamed: $solutionNameOld.sln -> $solutionNameNew.sln" -ForegroundColor Gray
}
Write-Host "[OK] .sln file renamed!" -ForegroundColor Green
Write-Host ""

# Step 5: C?p nh?t n?i dung file .sln
Write-Host "[STEP 5] Updating .sln file content..." -ForegroundColor Green
$slnPath = Join-Path $TargetPath "$solutionNameNew.sln"
$slnContent = Get-Content $slnPath -Raw

# Thay th? project names và paths trong .sln
$slnContent = $slnContent -replace 'DoMinhGiaBao_ SE1856_A01_BE\\DoMinhGiaBao_ SE1856_A01_BE\.csproj', 'PRN232.LAB.API\PRN232.LAB.API.csproj'
$slnContent = $slnContent -replace '"DoMinhGiaBao_ SE1856_A01_BE"', '"PRN232.LAB.API"'
$slnContent = $slnContent -replace 'DoMinhGiaBao_SE1856_A01_Repository\\DoMinhGiaBao_SE1856_A01_Repository\.csproj', 'PRN232.LAB.Repo\PRN232.LAB.Repo.csproj'
$slnContent = $slnContent -replace '"DoMinhGiaBao_SE1856_A01_Repository"', '"PRN232.LAB.Repo"'
$slnContent = $slnContent -replace 'DoMinhGiaBao_SE1856_A01_Service\\DoMinhGiaBao_SE1856_A01_Service\.csproj', 'PRN232.LAB.Services\PRN232.LAB.Services.csproj'
$slnContent = $slnContent -replace '"DoMinhGiaBao_SE1856_A01_Service"', '"PRN232.LAB.Services"'

Set-Content -Path $slnPath -Value $slnContent -NoNewline
Write-Host "[OK] .sln file content updated!" -ForegroundColor Green
Write-Host ""

# Step 6: C?p nh?t ProjectReference trong các file .csproj
Write-Host "[STEP 6] Updating ProjectReferences in .csproj files..." -ForegroundColor Green
$csprojFiles = Get-ChildItem -Path $TargetPath -Filter "*.csproj" -Recurse

foreach ($csproj in $csprojFiles) {
    $content = Get-Content $csproj.FullName -Raw
    $originalContent = $content
    
    # Thay th? các ProjectReference paths
    $content = $content -replace 'DoMinhGiaBao_ SE1856_A01_BE\\DoMinhGiaBao_ SE1856_A01_BE\.csproj', 'PRN232.LAB.API\PRN232.LAB.API.csproj'
    $content = $content -replace 'DoMinhGiaBao_SE1856_A01_Repository\\DoMinhGiaBao_SE1856_A01_Repository\.csproj', 'PRN232.LAB.Repo\PRN232.LAB.Repo.csproj'
    $content = $content -replace 'DoMinhGiaBao_SE1856_A01_Service\\DoMinhGiaBao_SE1856_A01_Service\.csproj', 'PRN232.LAB.Services\PRN232.LAB.Services.csproj'
    
    if ($content -ne $originalContent) {
        Set-Content -Path $csproj.FullName -Value $content -NoNewline
        Write-Host "  Updated: $($csproj.Name)" -ForegroundColor Gray
    }
}
Write-Host "[OK] ProjectReferences updated!" -ForegroundColor Green
Write-Host ""

# Step 7: C?p nh?t namespaces trong t?t c? file .cs
Write-Host "[STEP 7] Updating namespaces in .cs files..." -ForegroundColor Green
$csFiles = Get-ChildItem -Path $TargetPath -Filter "*.cs" -Recurse
$updatedCount = 0

foreach ($csFile in $csFiles) {
    $content = Get-Content $csFile.FullName -Raw
    $originalContent = $content
    
    # Thay th? namespaces - theo th? t? c? th? ?? tránh xung ??t
    $content = $content -replace 'DoMinhGiaBao_ SE1856_A01_BE', 'PRN232.LAB.API'
    $content = $content -replace 'DoMinhGiaBao_SE1856_A01_BE', 'PRN232.LAB.API'
    $content = $content -replace 'DoMinhGiaBao_SE1856_A01_Repository', 'PRN232.LAB.Repo'
    $content = $content -replace 'DoMinhGiaBao_SE1856_A01_Service', 'PRN232.LAB.Services'
    
    if ($content -ne $originalContent) {
        Set-Content -Path $csFile.FullName -Value $content -NoNewline
        $updatedCount++
    }
}
Write-Host "  Updated $updatedCount .cs files" -ForegroundColor Gray
Write-Host "[OK] Namespaces updated!" -ForegroundColor Green
Write-Host ""

# Step 8: C?p nh?t Dockerfile n?u có
Write-Host "[STEP 8] Updating Dockerfile..." -ForegroundColor Green
$dockerfiles = Get-ChildItem -Path $TargetPath -Filter "Dockerfile" -Recurse

foreach ($dockerfile in $dockerfiles) {
    $content = Get-Content $dockerfile.FullName -Raw
    $originalContent = $content
    
    $content = $content -replace 'DoMinhGiaBao_ SE1856_A01_BE', 'PRN232.LAB.API'
    $content = $content -replace 'DoMinhGiaBao_SE1856_A01_BE', 'PRN232.LAB.API'
    $content = $content -replace 'DoMinhGiaBao_SE1856_A01_Repository', 'PRN232.LAB.Repo'
    $content = $content -replace 'DoMinhGiaBao_SE1856_A01_Service', 'PRN232.LAB.Services'
    
    if ($content -ne $originalContent) {
        Set-Content -Path $dockerfile.FullName -Value $content -NoNewline
        Write-Host "  Updated: $($dockerfile.FullName)" -ForegroundColor Gray
    }
}
Write-Host "[OK] Dockerfile updated!" -ForegroundColor Green
Write-Host ""

# Step 9: C?p nh?t launchSettings.json n?u có
Write-Host "[STEP 9] Updating launchSettings.json..." -ForegroundColor Green
$launchSettings = Get-ChildItem -Path $TargetPath -Filter "launchSettings.json" -Recurse

foreach ($settings in $launchSettings) {
    $content = Get-Content $settings.FullName -Raw
    $originalContent = $content
    
    $content = $content -replace 'DoMinhGiaBao_ SE1856_A01_BE', 'PRN232.LAB.API'
    $content = $content -replace 'DoMinhGiaBao_SE1856_A01_BE', 'PRN232.LAB.API'
    
    if ($content -ne $originalContent) {
        Set-Content -Path $settings.FullName -Value $content -NoNewline
        Write-Host "  Updated: $($settings.FullName)" -ForegroundColor Gray
    }
}
Write-Host "[OK] launchSettings.json updated!" -ForegroundColor Green
Write-Host ""

# Step 10: Xóa th? m?c bin và obj ?? clean build
Write-Host "[STEP 10] Cleaning bin and obj folders..." -ForegroundColor Green
Get-ChildItem -Path $TargetPath -Include "bin","obj" -Recurse -Directory | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "[OK] Cleaned!" -ForegroundColor Green
Write-Host ""

# Step 11: Test build
Write-Host "[STEP 11] Testing build..." -ForegroundColor Green
Push-Location $TargetPath
try {
    $buildResult = dotnet build "$solutionNameNew.sln" 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "[OK] Build successful!" -ForegroundColor Green
    } else {
        Write-Host "[WARN] Build has warnings/errors. Please check manually." -ForegroundColor Yellow
        Write-Host $buildResult -ForegroundColor Gray
    }
} finally {
    Pop-Location
}
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RENAME COMPLETED SUCCESSFULLY!       " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "New solution location: $TargetPath" -ForegroundColor Yellow
Write-Host ""
Write-Host "Project Mappings:" -ForegroundColor Yellow
Write-Host "  DoMinhGiaBao_ SE1856_A01_BE       -> PRN232.LAB.API" -ForegroundColor Gray
Write-Host "  DoMinhGiaBao_SE1856_A01_Repository -> PRN232.LAB.Repo" -ForegroundColor Gray
Write-Host "  DoMinhGiaBao_SE1856_A01_Service    -> PRN232.LAB.Services" -ForegroundColor Gray
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Close Visual Studio" -ForegroundColor Gray
Write-Host "  2. Open: $TargetPath\$solutionNameNew.sln" -ForegroundColor Gray
Write-Host "  3. Update Git remote if needed" -ForegroundColor Gray
Write-Host ""
