# Rust Mini Kernel - 简易版类Linux内核

这是一个使用Rust编写的简易类Linux内核项目，用于学习操作系统内核开发的基本概念。

## 项目特性

- ✅ **裸机编程**: 不依赖任何操作系统，直接在硬件上运行
- ✅ **VGA文本输出**: 实现了基本的屏幕文字输出功能
- ✅ **内核panic处理**: 自定义的panic处理机制
- ✅ **串口通信**: 支持通过串口输出调试信息
- ✅ **中文支持**: 内核启动信息包含中英文双语

## 项目结构

```
rust_mini_kernel/
├── src/
│   ├── main.rs          # 内核入口点
│   ├── vga_buffer.rs    # VGA文本缓冲区实现
│   └── serial.rs        # 串口通信实现
├── .cargo/
│   └── config.toml      # Cargo配置
├── Cargo.toml           # 项目依赖配置
├── Makefile             # 构建脚本
├── .gitignore           # Git忽略文件
└── README.md            # 本文件
```

## 技术架构

### 核心组件

1. **VGA文本缓冲区** (`vga_buffer.rs`)
   - 实现了80x25字符的VGA文本模式
   - 支持16种颜色
   - 自动滚屏功能
   - 线程安全的全局写入器

2. **串口通信** (`serial.rs`)
   - 用于调试输出
   - 支持UART 16550串口

3. **内核入口** (`main.rs`)
   - `_start` 函数作为内核入口点
   - panic处理器
   - 简洁的启动界面

## 构建要求

### 必需工具

- Rust (nightly toolchain)
- cargo
- bootimage工具 (用于创建可启动镜像)

### 安装步骤

1. 安装Rust nightly工具链：
```bash
rustup toolchain install nightly
```

2. 安装必要组件：
```bash
rustup component add rust-src --toolchain nightly
rustup component add llvm-tools-preview --toolchain nightly
```

3. 添加x86_64-unknown-none目标：
```bash
rustup target add x86_64-unknown-none
```

4. 安装bootimage工具：
```bash
cargo install bootimage
```

## 构建和运行

### 使用Makefile构建

```bash
cd rust_mini_kernel

# 查看所有可用命令
make help

# 构建内核
make build

# 创建可启动镜像
make bootimage

# 在QEMU中运行（需要先安装QEMU）
make run

# 清理构建产物
make clean
```

### 手动构建

```bash
# 构建内核
cargo build

# 构建发布版本
cargo build --release
```

### 创建可启动镜像

```bash
cargo bootimage
```

### 在QEMU中运行

首先安装QEMU:

```bash
# Ubuntu/Debian
sudo apt-get install qemu-system-x86

# macOS
brew install qemu

# Arch Linux
sudo pacman -S qemu
```

然后运行内核:

```bash
cargo run
```

或者直接使用QEMU:

```bash
qemu-system-x86_64 -drive format=raw,file=target/x86_64-unknown-none/debug/bootimage-rust_mini_kernel.bin
```

## 内核功能演示

当内核启动时，你会在屏幕上看到:

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

## 代码示例

### 使用VGA缓冲区输出

```rust
use crate::println;

pub extern "C" fn _start() -> ! {
    println!("Hello, Kernel World!");
    println!("你好，内核世界！");
    loop {}
}
```

### 自定义颜色输出

```rust
use crate::vga_buffer::{WRITER, Color, ColorCode};

let mut writer = WRITER.lock();
writer.color_code = ColorCode::new(Color::LightGreen, Color::Black);
writer.write_string("绿色文本!");
```

## 学习资源

这个项目受以下优秀资源启发:

- [Writing an OS in Rust](https://os.phil-opp.com/) - Philipp Oppermann
- [The Rust Programming Language](https://doc.rust-lang.org/book/)
- [OSDev Wiki](https://wiki.osdev.org/)
- [Rust嵌入式书](https://rust-embedded.github.io/book/)

## 技术细节

### 内存布局

- VGA文本缓冲区位于物理地址 `0xb8000`
- 每个字符占用2个字节：1字节ASCII码 + 1字节颜色码

### 编译目标

项目使用 `x86_64-unknown-none` 作为编译目标，特点:
- 裸机环境（no_std）
- 不依赖操作系统
- 直接在x86_64硬件上运行

### 依赖项

- `bootloader`: 提供BIOS启动支持（0.9版本）
- `volatile`: 防止编译器优化VGA缓冲区访问
- `spin`: 提供自旋锁用于并发控制
- `uart_16550`: 串口通信支持
- `x86_64`: x86_64架构特定功能
- `lazy_static`: 静态变量延迟初始化

### 为什么使用Rust?

1. **内存安全**: Rust的所有权系统在编译期防止内存错误
2. **零成本抽象**: 高级特性不影响运行时性能
3. **现代语言特性**: 模式匹配、trait、宏等强大功能
4. **优秀的工具链**: Cargo包管理器和构建系统
5. **活跃的社区**: 丰富的文档和学习资源

## 未来改进方向

- [ ] 实现中断描述符表(IDT)
- [ ] 添加键盘输入支持
- [ ] 实现内存管理（分页、堆分配）
- [ ] 添加进程调度器
- [ ] 实现简单的文件系统
- [ ] 支持多核CPU
- [ ] 添加网络协议栈
- [ ] 实现系统调用接口

## 问题排查

### 构建失败

如果遇到构建问题，请确保:
1. 使用nightly工具链: `rustup default nightly`
2. 已安装rust-src: `rustup component add rust-src`
3. 已添加目标: `rustup target add x86_64-unknown-none`

### QEMU运行失败

**Windows 用户解决方案**

如果在 Windows 上运行 QEMU 出现问题，请使用以下解决方案：

1. **运行诊断脚本**：
```bash
qemu_diagnose.bat
```

2. **使用多配置启动脚本**：
```bash
run_qemu.bat
```

3. **安装 QEMU（推荐方式）**：
```bash
choco install qemu
```

4. **使用 PowerShell 脚本**：
```bash
powershell -ExecutionPolicy Bypass -File qemu_runner.ps1
```

5. **WSL 替代方案**：
```bash
bash run_kernel.sh
```

**常见解决方案**

- 确保以管理员身份运行脚本
- 检查系统虚拟化是否启用（任务管理器 > 性能 > CPU）
- 尝试不同的 QEMU 配置参数
- 检查 Windows 防火墙和杀毒软件设置

详细故障排除指南请参考：`QEMU_TROUBLESHOOTING.md`

## 许可证

本项目用于教育目的，欢迎学习和改进。

## 贡献

欢迎提交问题和改进建议！如果你想贡献代码：

1. Fork本仓库
2. 创建特性分支 (`git checkout -b feature/amazing-feature`)
3. 提交更改 (`git commit -m 'Add some amazing feature'`)
4. 推送到分支 (`git push origin feature/amazing-feature`)
5. 开启Pull Request

## 致谢

感谢Rust社区和所有为操作系统开发教育做出贡献的开发者！

---

**注意**: 这是一个教育性质的简化内核，不适合用于生产环境。它的目的是帮助理解操作系统的基本概念和Rust在系统编程中的应用。
