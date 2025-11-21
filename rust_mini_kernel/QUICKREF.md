# 快速参考指南

## 常用命令

### 构建相关
```bash
# 构建项目（开发模式）
cargo build

# 构建项目（发布模式，优化）
cargo build --release

# 清理构建产物
cargo clean

# 检查代码（不生成可执行文件）
cargo check
```

### 代码质量
```bash
# 格式化代码
cargo fmt

# 检查代码风格
cargo fmt --check

# 运行Clippy代码检查
cargo clippy
```

### 运行和测试
```bash
# 创建可启动镜像
cargo bootimage

# 在QEMU中运行
cargo run

# 或使用Makefile
make build
make run
```

## 项目结构速查

```
rust_mini_kernel/
├── src/
│   ├── main.rs           # 内核入口，_start函数，panic处理
│   ├── vga_buffer.rs     # VGA文本输出，println!宏
│   └── serial.rs         # 串口通信，serial_println!宏
├── .cargo/
│   └── config.toml       # 指定编译目标和构建选项
├── Cargo.toml            # 项目配置和依赖
├── Makefile              # 方便的构建命令
├── README.md             # 完整文档
├── EXAMPLES.md           # 扩展功能示例
└── QUICKREF.md           # 本文件
```

## 核心API速查

### VGA输出
```rust
// 使用宏输出（推荐）
println!("Hello, {}!", "World");
print!("No newline");

// 直接使用Writer
use crate::vga_buffer::{WRITER, Color, ColorCode};

let mut writer = WRITER.lock();
writer.color_code = ColorCode::new(Color::Yellow, Color::Black);
writer.write_string("自定义颜色!\n");
```

### 串口输出（调试用）
```rust
serial_println!("Debug: value = {}", 42);
serial_print!("No newline");
```

### CPU控制
```rust
use x86_64::instructions;

// 暂停CPU直到下一个中断
instructions::hlt();

// 禁用中断
instructions::interrupts::disable();

// 启用中断
instructions::interrupts::enable();

// 在禁用中断的情况下执行代码
instructions::interrupts::without_interrupts(|| {
    // 临界区代码
});
```

### 端口IO
```rust
use x86_64::instructions::port::Port;

unsafe {
    // 创建端口
    let mut port: Port<u8> = Port::new(0x3F8);
    
    // 写入
    port.write(42);
    
    // 读取
    let value = port.read();
}
```

## 颜色代码

```rust
pub enum Color {
    Black = 0,        // 黑色
    Blue = 1,         // 蓝色
    Green = 2,        // 绿色
    Cyan = 3,         // 青色
    Red = 4,          // 红色
    Magenta = 5,      // 品红
    Brown = 6,        // 棕色
    LightGray = 7,    // 浅灰
    DarkGray = 8,     // 深灰
    LightBlue = 9,    // 浅蓝
    LightGreen = 10,  // 浅绿
    LightCyan = 11,   // 浅青
    LightRed = 12,    // 浅红
    Pink = 13,        // 粉色
    Yellow = 14,      // 黄色
    White = 15,       // 白色
}
```

## 常见内存地址

| 地址 | 用途 |
|------|------|
| 0xb8000 | VGA文本缓冲区起始地址 |
| 0x3F8 | COM1串口基地址 |
| 0x3F9 | COM1串口中断使能寄存器 |
| 0x43 | PIT命令端口 |
| 0x40 | PIT通道0数据端口 |

## 依赖库速查

| 库 | 版本 | 用途 |
|----|------|------|
| bootloader | 0.9.23 | BIOS启动加载器 |
| volatile | 0.2.6 | 防止编译器优化volatile内存访问 |
| spin | 0.5.2 | 无锁并发原语（自旋锁） |
| uart_16550 | 0.2.0 | UART串口通信 |
| x86_64 | 0.14.2 | x86_64架构特定功能 |
| lazy_static | 1.0 | 延迟静态变量初始化 |

## 编译目标

- **目标平台**: x86_64-unknown-none
- **特性**: 
  - no_std（无标准库）
  - no_main（无标准main函数）
  - panic_abort（panic时中止）

## 故障排查

### 问题：构建失败，找不到core库
```bash
# 解决方案：安装rust-src
rustup component add rust-src --toolchain nightly
```

### 问题：找不到目标平台
```bash
# 解决方案：添加目标
rustup target add x86_64-unknown-none
```

### 问题：bootimage命令不存在
```bash
# 解决方案：安装bootimage
cargo install bootimage
```

### 问题：需要nightly工具链
```bash
# 解决方案：设置项目使用nightly
cd rust_mini_kernel
rustup override set nightly
```

## 性能提示

1. **使用release模式**: `cargo build --release` 可以显著提升性能
2. **避免浮点运算**: x86_64-unknown-none目标禁用了SSE，使用整数运算
3. **使用volatile访问**: 访问硬件内存时必须使用volatile防止优化
4. **最小化锁竞争**: WRITER是全局锁，尽量减少持锁时间

## 调试技巧

1. **使用串口输出**: `serial_println!` 比 `println!` 更适合调试
2. **QEMU监视器**: 在QEMU中按 `Ctrl+Alt+2` 进入监视器模式
3. **GDB调试**: 
   ```bash
   qemu-system-x86_64 -s -S -drive format=raw,file=<image>
   # 在另一个终端
   gdb
   target remote :1234
   ```

## 有用的链接

- [Rust文档](https://doc.rust-lang.org/)
- [x86_64 crate文档](https://docs.rs/x86_64/)
- [OSDev Wiki](https://wiki.osdev.org/)
- [Intel开发者手册](https://software.intel.com/content/www/us/en/develop/articles/intel-sdm.html)
