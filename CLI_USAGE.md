# TeamCity API 命令行工具使用指南

## 概述

TeamCity API 命令行工具提供了两种使用模式：

1. **示例模式**：不带任何参数运行时，自动执行完整的示例演示
2. **命令行模式**：提供参数时，执行特定的 TeamCity API 操作

---

## 快速开始

### 1. 无参数运行（示例模式）

```bash
# 直接运行
dotnet run

# 或编译后运行
TeamcityAPI.exe
```

这将运行所有示例代码，展示各种 API 的使用方法。

### 2. 命令行模式

```bash
# 使用 dotnet run
dotnet run -- test --token your-token

# 使用编译后的程序
TeamcityAPI.exe test --token your-token
```

---

## 认证方式

### Token 认证（推荐）

```bash
TeamcityAPI <命令> --token YOUR_TOKEN [其他选项]
```

### 用户名密码认证

```bash
TeamcityAPI <命令> --username admin --password password [其他选项]
```

### 连接选项

| 参数 | 简写 | 说明 | 默认值 |
|------|------|------|--------|
| `--server` | `-s` | TeamCity 服务器地址 | `http://localhost:8111` |
| `--token` | `-t` | Access Token | - |
| `--username` | `-u` | 用户名（基本认证） | - |
| `--password` | `-p` | 密码（基本认证） | - |
| `--timeout` | - | 超时时间（秒） | `30` |

---

## 可用命令

### 1. 测试连接

```bash
# 测试与 TeamCity 服务器的连接
dotnet run -- test --token YOUR_TOKEN

# 测试远程服务器
dotnet run -- test --server https://teamcity.example.com --token YOUR_TOKEN
```

**别名**: `test`, `test-connection`

---

### 2. 查询项目

```bash
# 列出所有项目（第一页，默认 20 条）
dotnet run -- projects --token YOUR_TOKEN

# 查询第 2 页，每页 10 条
dotnet run -- projects --token YOUR_TOKEN --page 2 --pagesize 10
```

**别名**: `projects`, `list-projects`

**选项**:
- `--page`: 页码（默认 1）
- `--pagesize`: 每页数量（默认 20）

---

### 3. 查询构建配置

```bash
# 列出所有构建配置
dotnet run -- builds --token YOUR_TOKEN

# 查询指定项目的构建配置
dotnet run -- builds --token YOUR_TOKEN --project MyProject

# 分页查询
dotnet run -- builds --token YOUR_TOKEN --page 2 --pagesize 15
```

**别名**: `builds`, `list-builds`

**选项**:
- `--project`: 项目 ID（可选）
- `--page`: 页码（默认 1）
- `--pagesize`: 每页数量（默认 20）

---

### 4. 触发构建

```bash
# 基本触发
dotnet run -- trigger --token YOUR_TOKEN --buildtype MyBuildConfig

# 指定分支触发
dotnet run -- trigger --token YOUR_TOKEN --buildtype MyBuildConfig --branch develop

# 带注释触发
dotnet run -- trigger --token YOUR_TOKEN --buildtype MyBuildConfig --comment "测试构建"

# 带参数触发
dotnet run -- trigger --token YOUR_TOKEN --buildtype MyBuildConfig \
  --param env.VERSION=1.0.0 \
  --param system.debug=true
```

**别名**: `trigger`, `trigger-build`

**必需选项**:
- `--buildtype`: 构建配置 ID

**可选选项**:
- `--branch`: 分支名称
- `--comment`: 构建注释
- `--param`: 构建参数（格式：key=value，可多次使用）

---

### 5. 取消构建

```bash
# 取消构建
dotnet run -- cancel --token YOUR_TOKEN --buildid 12345

# 带注释取消
dotnet run -- cancel --token YOUR_TOKEN --buildid 12345 --comment "取消原因"
```

**别名**: `cancel`, `cancel-build`

**必需选项**:
- `--buildid`: 构建 ID

**可选选项**:
- `--comment`: 取消原因

---

### 6. 查询构建信息

```bash
# 获取指定构建的详细信息
dotnet run -- get-build --token YOUR_TOKEN --buildid 12345
```

**必需选项**:
- `--buildid`: 构建 ID

---

### 7. 查询构建历史

```bash
# 查询构建配置的历史记录
dotnet run -- history --token YOUR_TOKEN --buildtype MyBuildConfig

# 分页查询
dotnet run -- history --token YOUR_TOKEN --buildtype MyBuildConfig --page 1 --pagesize 10
```

**别名**: `history`, `build-history`

**必需选项**:
- `--buildtype`: 构建配置 ID

**可选选项**:
- `--page`: 页码（默认 1）
- `--pagesize`: 每页数量（默认 20）

---

### 8. 查询构建代理

```bash
# 列出所有构建代理
dotnet run -- agents --token YOUR_TOKEN

# 分页查询
dotnet run -- agents --token YOUR_TOKEN --page 1 --pagesize 10
```

**别名**: `agents`, `list-agents`

**选项**:
- `--page`: 页码（默认 1）
- `--pagesize`: 每页数量（默认 20）

---

## 完整示例

### 使用本地 Token

```bash
# 设置环境变量（推荐）
$env:TC_TOKEN="eyJ0eXAiOiAiVENWMiJ9..."
$env:TC_SERVER="http://localhost:8111"

# 测试连接
dotnet run -- test --server $env:TC_SERVER --token $env:TC_TOKEN

# 查询项目
dotnet run -- projects --server $env:TC_SERVER --token $env:TC_TOKEN

# 触发构建
dotnet run -- trigger --server $env:TC_SERVER --token $env:TC_TOKEN \
  --buildtype MyBuild --branch main --comment "从命令行触发"
```

### 使用远程服务器

```bash
# 连接到远程 TeamCity 服务器
dotnet run -- projects \
  --server https://teamcity.mycompany.com \
  --token YOUR_REMOTE_TOKEN \
  --timeout 60
```

### 批量操作（使用脚本）

**PowerShell 示例**:

```powershell
# 查询所有项目
$projects = dotnet run -- projects --token $token | Out-String

# 触发多个构建
$buildConfigs = @("Build1", "Build2", "Build3")
foreach ($config in $buildConfigs) {
    dotnet run -- trigger --token $token --buildtype $config --branch develop
    Start-Sleep -Seconds 5
}
```

**Bash 示例**:

```bash
#!/bin/bash
TOKEN="your-token"
SERVER="http://localhost:8111"

# 查询项目
dotnet run -- projects --server $SERVER --token $TOKEN

# 触发构建
dotnet run -- trigger --server $SERVER --token $TOKEN \
  --buildtype MyBuild --branch main

# 等待构建完成后查询历史
sleep 60
dotnet run -- history --server $SERVER --token $TOKEN \
  --buildtype MyBuild --pagesize 5
```

---

## 帮助信息

```bash
# 显示帮助
dotnet run -- --help
dotnet run -- -h
dotnet run -- help
```

---

## 返回值

命令执行成功返回 `0`，失败返回 `1`。

```bash
# 在脚本中使用返回值
dotnet run -- test --token $token
if ($LASTEXITCODE -eq 0) {
    Write-Host "连接成功"
} else {
    Write-Host "连接失败"
}
```

---

## 错误处理

### API 错误

```
✗ API 错误: 请求失败: Unauthorized (状态码: 401)
```

### 超时错误

```
✗ 错误: 请求超时
```

### 参数错误

```
错误: 必须指定 --buildtype
```

---

## 高级用法

### 1. 持续集成脚本

```powershell
# 触发构建并等待完成
$result = dotnet run -- trigger --token $token --buildtype MyBuild --branch main
if ($LASTEXITCODE -eq 0) {
    # 解析构建 ID（从输出中提取）
    Write-Host "构建已触发，等待完成..."
    Start-Sleep -Seconds 300
    
    # 查询构建历史
    dotnet run -- history --token $token --buildtype MyBuild --pagesize 1
}
```

### 2. 监控脚本

```bash
#!/bin/bash
# 定期查询构建状态
while true; do
    echo "==== $(date) ===="
    dotnet run -- agents --token $TOKEN --pagesize 5
    echo ""
    sleep 60
done
```

### 3. 自动化部署

```powershell
# 1. 触发构建
Write-Host "触发构建..."
dotnet run -- trigger --token $token --buildtype Release --branch main

# 2. 等待
Start-Sleep -Seconds 600

# 3. 检查结果
Write-Host "检查构建结果..."
dotnet run -- history --token $token --buildtype Release --pagesize 1
```

---

## 项目结构

```
TeamcityAPI/
├── CLI/
│   └── CommandHandler.cs      # 命令处理器（所有 CLI 操作）
├── Examples/
│   └── ExampleUsage.cs        # 示例代码（无参数时运行）
├── Program.cs                 # 主入口（命令行参数解析）
└── ...
```

---

## 从代码调用

如果您想在自己的代码中使用 TeamCity API（而不是命令行），请参考 `Examples/ExampleUsage.cs` 中的示例。

---

## 常见问题

### Q: 如何获取 Access Token？

A: 在 TeamCity Web UI 中：
1. 点击右上角用户名
2. 选择 "Access Tokens"
3. 创建新的 Token

### Q: 如何找到构建配置 ID？

A: 使用命令查询：
```bash
dotnet run -- builds --token YOUR_TOKEN
```

### Q: 如何在 CI/CD 中使用？

A: 将 Token 存储在环境变量或密钥管理系统中：
```bash
# GitHub Actions
- name: Trigger Build
  run: dotnet run -- trigger --token ${{ secrets.TC_TOKEN }} --buildtype MyBuild

# Azure DevOps
- script: dotnet run -- trigger --token $(TC_TOKEN) --buildtype MyBuild
```

---

## 联系与贡献

欢迎提交 Issue 和 Pull Request！

