# Running Rust Mini Kernel in WSL/Linux

## Prerequisites

### Install QEMU in WSL/Linux
```bash
# Ubuntu/Debian
sudo apt update
sudo apt install qemu-system-x86

# CentOS/RHEL/Rocky Linux
sudo yum install qemu-kvm qemu-system-x86
# or on newer versions:
sudo dnf install qemu-kvm qemu-system-x86

# Arch Linux
sudo pacman -S qemu-base
```

### Install additional tools (optional)
```bash
sudo apt install build-essential
```

## Building the Kernel

From your Windows directory (accessed from WSL):
```bash
# Navigate to your project directory
cd /mnt/e/项目/asp.net\ core/rust_mini_kernel

# Build the kernel
cargo bootimage --release
```

## Running the Kernel

### Basic QEMU command:
```bash
qemu-system-x86_64 \
  -drive format=raw,file=target/x86_64-unknown-none/release/bootimage-rust_mini_kernel.bin \
  -nographic \
  -serial stdio
```

### Alternative with debug output:
```bash
qemu-system-x86_64 \
  -drive format=raw,file=target/x86_64-unknown-none/release/bootimage-rust_mini_kernel.bin \
  -nographic \
  -serial mon:stdio \
  -monitor telnet:127.0.0.1:1234,server,nowait
```

### For debugging with GDB:
```bash
qemu-system-x86_64 \
  -drive format=raw,file=target/x86_64-unknown-none/release/bootimage-rust_mini_kernel.bin \
  -nographic \
  -s -S &
gdb target/x86_64-unknown-none/release/rust_mini_kernel
```

## Common Issues and Solutions

### 1. File path issues in WSL
If you get file not found errors, make sure you're in the correct directory:
```bash
pwd
ls -la target/x86_64-unknown-none/release/
```

### 2. Permission issues
```bash
chmod +x target/x86_64-unknown-none/release/bootimage-rust_mini_kernel.bin
```

### 3. Alternative file path format
```bash
qemu-system-x86_64 -drive format=raw,file=./target/x86_64-unknown-none/release/bootimage-rust_mini_kernel.bin -nographic
```

## Expected Output

You should see:
```
====================================
欢迎使用迷你Linux内核!
Welcome to Mini Linux Kernel!
====================================

内核版本: 0.1.0
语言: Rust

系统启动成功 - System Boot Success!

主要功能模块:
  [√] VGA文本输出
  [√] 串口通信
  [√] Panic处理

内核正在运行...
```

## Kernel Information

- **Architecture**: x86_64
- **Stack Size**: 512 bytes
- **Features**: VGA output, serial communication, panic handling
- **Build Type**: Release (optimized)

## Troubleshooting

If you still encounter issues:

1. **Check QEMU version**: `qemu-system-x86_64 --version`
2. **Verify bootimage exists**: `ls -la target/x86_64-unknown-none/release/bootimage-rust_mini_kernel.bin`
3. **Try debug version**: Use the debug bootimage instead of release
4. **Check file permissions**: Ensure the bootimage file is readable
