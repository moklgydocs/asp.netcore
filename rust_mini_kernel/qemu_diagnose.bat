@echo off
echo === QEMU Diagnostics for Rust Mini Kernel ===
echo.

REM Check if QEMU is accessible
echo [1] Checking QEMU installation...
where qemu-system-x86_64 >nul 2>&1
if %errorlevel% equ 0 (
    echo ✓ QEMU found in PATH
    qemu-system-x86_64 --version
) else (
    echo ✗ QEMU not found in PATH
    echo Checking default installation path...
    if exist "C:\Program Files\qemu\qemu-system-x86_64.exe" (
        echo ✓ QEMU found at C:\Program Files\qemu
    ) else (
        echo ✗ QEMU not found at C:\Program Files\qemu
        echo.
        echo Installing QEMU options:
        echo 1. Chocolatey: choco install qemu
        echo 2. MSYS2: pacman -S mingw-w64-x86_64-qemu
        echo 3. Official installer: https://www.qemu.org/download/
        echo.
    )
)

echo.
REM Check bootimage
echo [2] Checking bootimage...
if exist "target\x86_64-unknown-none\release\bootimage-rust_mini_kernel.bin" (
    echo ✓ Bootimage found
    dir "target\x86_64-unknown-none\release\bootimage-rust_mini_kernel.bin"
) else (
    echo ✗ Bootimage not found
    echo Please run: cargo bootimage --release
)

echo.
REM Check directory structure
echo [3] Checking project structure...
if exist "Cargo.toml" (
    echo ✓ Cargo.toml found
) else (
    echo ✗ Cargo.toml not found
)

if exist "src\main.rs" (
    echo ✓ src\main.rs found
) else (
    echo ✗ src\main.rs not found
)

echo.
REM Check virtualization support
echo [4] Checking virtualization support...
echo This requires checking in Task Manager > Performance > CPU
echo Look for "Virtualization: Enabled"
echo.
echo You can also check with:
echo - Command: core_isolation_conf.exe
echo - PowerShell: Get-ComputerInfo | Select-Object CsVirtualizationCapability
echo.

REM Test QEMU with minimal parameters
echo [5] Testing QEMU basic functionality...
qemu-system-x86_64 -help >nul 2>&1
if %errorlevel% equ 0 (
    echo ✓ QEMU help command works
) else (
    echo ✗ QEMU help command failed
    echo This suggests QEMU installation issue
)

echo.
REM Check BIOS directory
echo [6] Checking QEMU BIOS directory...
if exist "C:\Program Files\qemu" (
    echo ✓ QEMU directory exists
    dir "C:\Program Files\qemu" | findstr -i "bios\|rom"
    if %errorlevel% equ 0 (
        echo BIOS/ROM files found
    ) else (
        echo No BIOS/ROM files found in QEMU directory
    )
) else (
    echo ✗ QEMU directory not found
)

echo.
echo === Summary ===
echo If QEMU is installed but still failing:
echo 1. Try running as Administrator
echo 2. Disable antivirus temporarily
echo 3. Check Windows Defender Firewall settings
echo 4. Try different QEMU parameters (see run_qemu.bat)
echo.
echo Next steps:
echo 1. Run: run_qemu.bat
echo 2. Or try PowerShell version: powershell -ExecutionPolicy Bypass -File qemu_runner.ps1
echo.

pause
