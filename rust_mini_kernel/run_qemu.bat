@echo off
echo === Rust Mini Kernel QEMU Runner ===
echo.

REM Check if bootimage exists
if not exist "target\x86_64-unknown-none\release\bootimage-rust_mini_kernel.bin" (
    echo Error: Bootimage file not found!
    echo Expected path: target\x86_64-unknown-none\release\bootimage-rust_mini_kernel.bin
    echo Please run 'cargo bootimage --release' first.
    pause
    exit /b 1
)

echo Bootimage found: target\x86_64-unknown-none\release\bootimage-rust_mini_kernel.bin
echo.

echo === Trying different QEMU configurations ===
echo.

REM Configuration 1: Your original command
echo [1] Trying your original command...
qemu-system-x86_64 -L "C:\Program Files\qemu" -drive format=raw,file=target\x86_64-unknown-none\release\bootimage-rust_mini_kernel.bin
if %errorlevel% equ 0 goto success

echo.
echo [2] Trying with modern CPU...
qemu-system-x86_64 -L "C:\Program Files\qemu" -drive format=raw,file=target\x86_64-unknown-none\release\bootimage-rust_mini_kernel.bin -cpu max -machine q35
if %errorlevel% equ 0 goto success

echo.
echo [3] Trying debug mode with serial output...
qemu-system-x86_64 -L "C:\Program Files\qemu" -drive format=raw,file=target\x86_64-unknown-none\release\bootimage-rust_mini_kernel.bin -nographic -serial stdio -monitor none
if %errorlevel% equ 0 goto success

echo.
echo [4] Trying legacy mode...
qemu-system-x86_64 -L "C:\Program Files\qemu" -drive format=raw,file=target\x86_64-unknown-none\release\bootimage-rust_mini_kernel.bin -cpu 486 -machine pc -no-acpi
if %errorlevel% equ 0 goto success

echo.
echo [5] Trying minimal configuration...
qemu-system-x86_64 -L "C:\Program Files\qemu" -drive format=raw,file=target\x86_64-unknown-none\release\bootimage-rust_mini_kernel.bin -display none -nodefaults
if %errorlevel% equ 0 goto success

echo.
echo All QEMU configurations failed!
echo.
echo Possible solutions:
echo 1. Make sure QEMU is installed at "C:\Program Files\qemu"
echo 2. Try running QEMU from a different location
echo 3. Check if your system supports virtualization
echo 4. Run this script as administrator
echo.
echo You can also try installing QEMU via:
echo - Chocolatey: choco install qemu
echo - MSYS2: pacman -S mingw-w64-x86_64-qemu
echo.
pause
exit /b 1

:success
echo.
echo Kernel started successfully!
echo Press Ctrl+C to stop QEMU
pause
