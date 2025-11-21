#!/bin/bash

# Rust Mini Kernel Runner for WSL/Linux
# Make sure you're in the project directory: /mnt/e/项目/asp.net core/rust_mini_kernel

echo "=== Rust Mini Kernel Runner ==="
echo "Building kernel..."

# Build the kernel
cargo bootimage --release

if [ $? -eq 0 ]; then
    echo "Build successful!"
    echo "Running kernel in QEMU..."
    
    # Check if bootimage exists
    if [ -f "target/x86_64-unknown-none/release/bootimage-rust_mini_kernel.bin" ]; then
        echo "Starting QEMU..."
        echo "Press Ctrl+A, then X to exit QEMU"
        echo ""
        
        # Run the kernel
        qemu-system-x86_64 \
            -drive format=raw,file=target/x86_64-unknown-none/release/bootimage-rust_mini_kernel.bin \
            -nographic \
            -serial stdio
    else
        echo "Error: bootimage file not found!"
        echo "Expected path: target/x86_64-unknown-none/release/bootimage-rust_mini_kernel.bin"
        exit 1
    fi
else
    echo "Build failed!"
    exit 1
fi
