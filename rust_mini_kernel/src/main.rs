#![no_std]
#![no_main]

use core::panic::PanicInfo;

mod serial;
mod vga_buffer;

/// 内核入口点
#[no_mangle]
pub extern "C" fn _start() -> ! {
    println!("====================================");
    println!("欢迎使用迷你Linux内核!");
    println!("Welcome to Mini Linux Kernel!");
    println!("====================================");
    println!();
    println!("内核版本: 0.1.0");
    println!("语言: Rust");
    println!();
    println!("系统启动成功 - System Boot Success!");
    println!();
    println!("主要功能模块:");
    println!("  [√] VGA文本输出");
    println!("  [√] 串口通信");
    println!("  [√] Panic处理");
    println!();
    println!("内核正在运行...");

    // 内核主循环
    loop {
        x86_64::instructions::hlt();
    }
}

/// Panic处理器 - 当内核panic时调用
#[panic_handler]
fn panic(info: &PanicInfo) -> ! {
    println!("[内核Panic!]");
    println!("{}", info);
    loop {
        x86_64::instructions::hlt();
    }
}
