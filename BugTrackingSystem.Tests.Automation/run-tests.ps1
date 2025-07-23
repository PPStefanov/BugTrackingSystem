# Bug Tracking System Automation Test Runner
param(
    [string]$Browser = "chromium",
    [string]$TestFilter = "",
    [switch]$Headless = $false,
    [switch]$InstallBrowsers = $false
)

Write-Host "Bug Tracking System - Automation Test Runner" -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Green

# Install browsers if requested
if ($InstallBrowsers) {
    Write-Host "Installing Playwright browsers..." -ForegroundColor Yellow
    dotnet build
    pwsh bin/Debug/net8.0/playwright.ps1 install
    Write-Host "Browsers installed successfully!" -ForegroundColor Green
    return
}

# Build the project
Write-Host "Building test project..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Prepare test command
$testCommand = "dotnet test"

if ($TestFilter) {
    $testCommand += " --filter `"$TestFilter`""
}

$testCommand += " --logger:html --logger:junit"

# Set environment variables for Playwright
$env:BROWSER = $Browser
if ($Headless) {
    $env:HEADLESS = "true"
}

Write-Host "Running tests with browser: $Browser" -ForegroundColor Yellow
if ($TestFilter) {
    Write-Host "Test filter: $TestFilter" -ForegroundColor Yellow
}

# Run tests
Write-Host "Executing tests..." -ForegroundColor Yellow
Invoke-Expression $testCommand

if ($LASTEXITCODE -eq 0) {
    Write-Host "All tests passed!" -ForegroundColor Green
} else {
    Write-Host "Some tests failed. Check the test results for details." -ForegroundColor Red
}

Write-Host "Test results available in: test-results/" -ForegroundColor Cyan