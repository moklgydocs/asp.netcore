# 扩展内核功能示例

本文档展示了如何为Rust Mini Kernel添加新功能。

## 示例1: 添加颜色支持

在`main.rs`中使用不同的颜色：

```rust
use crate::vga_buffer::{WRITER, Color, ColorCode};

#[no_mangle]
pub extern "C" fn _start() -> ! {
    // 使用绿色文本
    {
        let mut writer = WRITER.lock();
        writer.color_code = ColorCode::new(Color::LightGreen, Color::Black);
        writer.write_string("成功启动!\n");
    }
    
    // 使用红色文本显示警告
    {
        let mut writer = WRITER.lock();
        writer.color_code = ColorCode::new(Color::LightRed, Color::Black);
        writer.write_string("警告: 这是测试信息\n");
    }
    
    // 恢复默认黄色
    println!("继续正常输出...");
    
    loop {}
}
```

## 示例2: 添加简单的命令处理

创建一个新文件 `src/command.rs`:

```rust
/// 简单的命令处理器
pub struct CommandProcessor;

impl CommandProcessor {
    pub fn new() -> Self {
        CommandProcessor
    }
    
    pub fn process(&self, command: &str) {
        match command.trim() {
            "help" => self.show_help(),
            "version" => self.show_version(),
            "clear" => self.clear_screen(),
            _ => println!("未知命令: {}", command),
        }
    }
    
    fn show_help(&self) {
        println!("可用命令:");
        println!("  help    - 显示帮助");
        println!("  version - 显示版本");
        println!("  clear   - 清屏");
    }
    
    fn show_version(&self) {
        println!("Rust Mini Kernel v0.1.0");
    }
    
    fn clear_screen(&self) {
        // 这里需要实现清屏功能
        println!("清屏功能待实现...");
    }
}
```

然后在`main.rs`中使用：

```rust
mod command;

use command::CommandProcessor;

#[no_mangle]
pub extern "C" fn _start() -> ! {
    println!("内核启动完成!");
    
    let processor = CommandProcessor::new();
    processor.process("help");
    processor.process("version");
    
    loop {}
}
```

## 示例3: 添加计时器

创建 `src/timer.rs`:

```rust
use x86_64::instructions::port::Port;

/// 简单的PIT (可编程间隔定时器) 封装
pub struct Timer {
    frequency: u32,
}

impl Timer {
    pub fn new() -> Self {
        Timer {
            frequency: 1000, // 1000 Hz
        }
    }
    
    pub fn init(&self) {
        let divisor = 1193180 / self.frequency;
        
        unsafe {
            let mut cmd: Port<u8> = Port::new(0x43);
            let mut data: Port<u8> = Port::new(0x40);
            
            // 设置PIT模式
            cmd.write(0x36);
            
            // 发送频率除数
            data.write((divisor & 0xFF) as u8);
            data.write(((divisor >> 8) & 0xFF) as u8);
        }
    }
    
    pub fn get_frequency(&self) -> u32 {
        self.frequency
    }
}
```

## 示例4: 添加内存信息显示

创建 `src/memory.rs`:

```rust
/// 显示内存信息
pub struct MemoryInfo;

impl MemoryInfo {
    pub fn display() {
        println!("内存布局:");
        println!("  VGA缓冲区: 0xb8000 - 0xb8fa0");
        println!("  串口基址:  0x3F8");
        println!("  内核代码:  [待实现获取地址]");
    }
    
    pub fn display_memory_map() {
        println!("内存映射:");
        println!("  0x00000000 - 0x000003FF : 实模式中断向量表");
        println!("  0x00000400 - 0x000004FF : BIOS数据区");
        println!("  0x00000500 - 0x00007BFF : 可用内存");
        println!("  0x00007C00 - 0x00007DFF : 引导扇区");
        println!("  0x000A0000 - 0x000BFFFF : 视频内存");
        println!("  0x000C0000 - 0x000FFFFF : BIOS ROM");
    }
}
```

使用方式：

```rust
mod memory;

use memory::MemoryInfo;

#[no_mangle]
pub extern "C" fn _start() -> ! {
    println!("系统启动!");
    MemoryInfo::display();
    MemoryInfo::display_memory_map();
    
    loop {}
}
```

## 示例5: 添加数学函数库

创建 `src/math.rs`:

```rust
/// 简单的数学运算库
pub struct Math;

impl Math {
    /// 计算两个数的最大公约数
    pub fn gcd(mut a: u64, mut b: u64) -> u64 {
        while b != 0 {
            let temp = b;
            b = a % b;
            a = temp;
        }
        a
    }
    
    /// 计算阶乘（递归）
    pub fn factorial(n: u64) -> u64 {
        if n <= 1 {
            1
        } else {
            n * Self::factorial(n - 1)
        }
    }
    
    /// 判断是否为质数
    pub fn is_prime(n: u64) -> bool {
        if n < 2 {
            return false;
        }
        if n == 2 {
            return true;
        }
        if n % 2 == 0 {
            return false;
        }
        
        let mut i = 3;
        while i * i <= n {
            if n % i == 0 {
                return false;
            }
            i += 2;
        }
        true
    }
}
```

使用示例：

```rust
mod math;

use math::Math;

#[no_mangle]
pub extern "C" fn _start() -> ! {
    println!("数学运算演示:");
    println!("GCD(48, 18) = {}", Math::gcd(48, 18));
    println!("5! = {}", Math::factorial(5));
    println!("17是质数吗? {}", Math::is_prime(17));
    
    loop {}
}
```

## 编译和测试

添加新功能后，记得：

1. 在`main.rs`中添加模块声明：
```rust
mod command;
mod timer;
mod memory;
mod math;
```

2. 重新构建：
```bash
cargo build
```

3. 运行测试：
```bash
cargo run
```

## 注意事项

1. **no_std环境**: 不能使用标准库，只能使用core库
2. **内存安全**: 使用unsafe时要特别小心
3. **硬件访问**: 直接访问硬件需要使用volatile操作
4. **并发**: 使用自旋锁而不是操作系统互斥锁

## 更多资源

- [Rust Embedded Book](https://rust-embedded.github.io/book/)
- [OSDev Wiki](https://wiki.osdev.org/)
- [Writing an OS in Rust](https://os.phil-opp.com/)
