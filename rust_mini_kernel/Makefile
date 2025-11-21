.PHONY: build run clean test help install-deps bootimage

# 默认目标
all: build

# 构建内核
build:
	@echo "构建内核..."
	cargo build

# 构建发布版本
build-release:
	@echo "构建发布版内核..."
	cargo build --release

# 创建可启动镜像
bootimage:
	@echo "创建可启动镜像..."
	cargo bootimage

# 在QEMU中运行
run:
	@echo "在QEMU中运行内核..."
	cargo run

# 清理构建产物
clean:
	@echo "清理构建产物..."
	cargo clean

# 安装必需的依赖
install-deps:
	@echo "安装Rust组件..."
	rustup toolchain install nightly
	rustup component add rust-src --toolchain nightly
	rustup component add llvm-tools-preview --toolchain nightly
	rustup target add x86_64-unknown-none
	@echo "安装bootimage工具..."
	cargo install bootimage

# 检查代码格式
fmt:
	@echo "格式化代码..."
	cargo fmt

# 代码检查
clippy:
	@echo "运行clippy检查..."
	cargo clippy

# 显示帮助信息
help:
	@echo "可用的Make目标:"
	@echo "  make build         - 构建内核"
	@echo "  make build-release - 构建发布版内核"
	@echo "  make bootimage     - 创建可启动镜像"
	@echo "  make run           - 在QEMU中运行内核"
	@echo "  make clean         - 清理构建产物"
	@echo "  make install-deps  - 安装必需的依赖"
	@echo "  make fmt           - 格式化代码"
	@echo "  make clippy        - 运行clippy检查"
	@echo "  make help          - 显示此帮助信息"
