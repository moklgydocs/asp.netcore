#!/usr/bin/env pwsh

# PowerShell Script for Running Rust Mini Kernel
# This script provides better error handling and multiple QEMU configurations

Write-Host "=== Rust Mini Kernel Runner for Windows ===" -ForegroundColor Green
Write-Host "PowerShell Edition" -ForegroundColor Yellow
Write-Host ""

# Check if we're in the right directory
$projectDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $projectDir

# Check if bootimage exists
$bootimagePath = "target\x86_64-unknown-none\release\bootimage-rust_mini_kernel.bin"

if (-not (Test-Path $bootimagePath)) {
    Write-Host "Error: Bootimage file not found!" -ForegroundColor Red
    Write-Host "Expected path: $bootimagePath" -ForegroundColor Yellow
    Write-Host "Please run 'cargo bootimage --release' first." -ForegroundColor Yellow
    exit 1
}

Write-Host "Bootimage found: $bootimagePath" -ForegroundColor Green
Write-Host ""

# Function to run QEMU with different configurations
function Run-Qemu {
    param(
        [string]$configName,
        [string[]]$args
    )
    
    Write-Host "Trying QEMU configuration: $configName" -ForegroundColor Cyan
    
    try {
        & qemu-system-x86_64 @args
        return $true
    }
    catch {
        Write-Host "QEMU failed with configuration '$configName': $_" -ForegroundColor Red
        return $false
    }
}

# Configuration 1: Basic configuration (your original command)
Write-Host "=== Configuration 1: Basic ===" -ForegroundColor Yellow
$basicArgs = @(
    "-L", "C:\Program Files\qemu",
    "-drive", "format=raw,file=$bootimagePath"
)

if (Run-Qemu -configName "Basic" -args $basicArgs) {
    Write-Host "Kernel started successfully with Basic configuration!" -ForegroundColor Green
    exit 0
}

# Configuration 2: With modern CPU and features
Write-Host "`n=== Configuration 2: Modern CPU ===" -ForegroundColor Yellow
$modernArgs = @(
    "-L", "C:\Program Files\qemu",
    "-drive", "format=raw,file=$bootimagePath",
    "-cpu", "max",
    "-machine", "q35",
    "-accel", "tcg"
)

if (Run-Qemu -configName "Modern CPU" -args $modernArgs) {
    Write-Host "Kernel started successfully with Modern CPU configuration!" -ForegroundColor Green
    exit 0
}

# Configuration 3: Debug mode with serial output
Write-Host "`n=== Configuration 3: Debug Mode ===" -ForegroundColor Yellow
$debugArgs = @(
    "-L", "C:\Program Files\qemu",
    "-drive", "format=raw,file=$bootimagePath",
    "-nographic",
    "-serial", "stdio",
    "-monitor", "none",
    "-cpu", "max"
)

if (Run-Qemu -configName "Debug" -args $debugArgs) {
    Write-Host "Kernel started successfully with Debug configuration!" -ForegroundColor Green
    exit 0
}

# Configuration 4: Legacy mode
Write-Host "`n=== Configuration 4: Legacy Mode ===" -ForegroundColor Yellow
$legacyArgs = @(
    "-L", "C:\Program Files\qemu",
    "-drive", "format=raw,file=$bootimagePath",
    "-cpu", "486",
    "-machine", "pc",
    "-no-acpi"
)

if (Run-Qemu -configName "Legacy" -args $legacyArgs) {
    Write-Host "Kernel started successfully with Legacy configuration!" -ForegroundColor Green
    exit 0
}

# Configuration 5: Minimal configuration
Write-Host "`n=== Configuration 5: Minimal ===" -ForegroundColor Yellow
$minimalArgs = @(
    "-L", "C:\Program Files\qemu",
    "-drive", "format=raw,file=$bootimagePath",
    "-display", "none",
    "-nodefaults"
)

if (Run-Qemu -configName "Minimal" -args $minimalArgs) {
    Write-Host "Kernel started successfully with Minimal configuration!" -ForegroundColor Green
    exit 0
}

Write-Host "`nAll QEMU configurations failed." -ForegroundColor Red
Write-Host "Please check:" -ForegroundColor Yellow
Write-Host "1. QEMU is installed and in PATH" -ForegroundColor Yellow
Write-Host "2. The bootimage file is not corrupted" -ForegroundColor Yellow
Write-Host "3. Your system supports virtualization" -ForegroundColor Yellow
Write-Host "4. Try running QEMU manually with: qemu-system-x86_64 -L 'C:\Program Files\qemu' -drive format=raw,file=$bootimagePath" -ForegroundColor Yellow
