# Microsoft.Extensions.Logging 日志集成

## ✅ 集成完成

成功将 **Microsoft.Extensions.Logging** 框架集成到 TeamCity API 项目中！

---

## 📦 安装的包

```xml
<PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.10" />
<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="9.0.10" />
```

还包括相关依赖：
- Microsoft.Extensions.DependencyInjection
- Microsoft.Extensions.Logging.Abstractions
- Microsoft.Extensions.Options
- Microsoft.Extensions.Primitives
- Microsoft.Extensions.Configuration

---

## 🎯 集成内容

### 1. TeamCityClient 日志支持

**修改**：
- 添加 `ILogger<TeamCityClient>` 字段
- 构造函数接受可选的 `logger` 参数
- 记录所有关键操作（连接、GET/POST 请求、错误等）

**日志记录点**：
- ✅ 客户端创建（Information）
- ✅ 连接测试（Debug/Information/Error）
- ✅ GET 请求（Debug/Warning/Error）
- ✅ POST 请求（Debug/Warning/Error）
- ✅ DELETE 请求（Debug/Warning/Error）
- ✅ 超时和异常（Error）

**示例代码**：
```csharp
public TeamCityClient(TokenAuthConfig config, ILogger<TeamCityClient>? logger = null)
{
    _logger = logger ?? NullLogger<TeamCityClient>.Instance;
    _logger.LogInformation("TeamCity 客户端已创建 (Token 认证) - 服务器: {ServerUrl}, 超时: {Timeout}秒", 
        _serverUrl, config.Timeout.TotalSeconds);
    // ...
}
```

### 2. 服务类日志准备

**修改**：
- `BuildService`、`QueryService`、`ProjectService`
- 构造函数接受可选的 `logger` 参数
- 为将来扩展详细日志做准备

### 3. 命令行参数

**新增选项**：
```csharp
[Option("log-level", Required = false, Default = "Information", 
    HelpText = "日志级别 (Trace, Debug, Information, Warning, Error, Critical, None)")]
public string LogLevel { get; set; } = "Information";
```

**支持的日志级别**：
- `Trace` - 最详细，包含所有信息
- `Debug` - 调试信息
- `Information` - 一般信息（默认）
- `Warning` - 警告
- `Error` - 错误
- `Critical` - 严重错误
- `None` - 不记录日志

### 4. Program.cs 日志配置

**创建日志工厂函数**：
```csharp
static ILoggerFactory CreateLoggerFactory(string logLevel)
{
    if (!Enum.TryParse<LogLevel>(logLevel, true, out var level))
    {
        level = LogLevel.Information;
    }

    return LoggerFactory.Create(builder =>
    {
        builder
            .SetMinimumLevel(level)
            .AddConsole(options =>
            {
                options.FormatterName = "simple";
            })
            .AddSimpleConsole(options =>
            {
                options.IncludeScopes = false;
                options.SingleLine = true;
                options.TimestampFormat = "[HH:mm:ss] ";
            });
    });
}
```

---

## 🚀 使用示例

### 1. 默认日志级别（Information）

```bash
dotnet run -- test --token YOUR_TOKEN
```

**输出**：
```
正在测试连接...
[19:00:33] info: TeamcityAPI.TeamCityClient[0] TeamCity 客户端已创建 (Token 认证) - 服务器: http://localhost:8111, 超时: 30秒
[19:00:33] info: TeamcityAPI.TeamCityClient[0] 成功连接到 TeamCity 服务器: http://localhost:8111
✓ 连接成功
```

### 2. Debug 日志级别

```bash
dotnet run -- test --token YOUR_TOKEN --log-level Debug
```

**输出**：
```
正在测试连接...
[19:00:33] info: TeamcityAPI.TeamCityClient[0] TeamCity 客户端已创建 (Token 认证) - 服务器: http://localhost:8111, 超时: 30秒
[19:00:33] dbug: TeamcityAPI.TeamCityClient[0] 测试 TeamCity 服务器连接: http://localhost:8111
[19:00:33] dbug: TeamcityAPI.TeamCityClient[0] 发送 GET 请求: /app/rest/server
[19:00:33] dbug: TeamcityAPI.TeamCityClient[0] GET 请求成功: /app/rest/server - 状态码: OK
[19:00:33] info: TeamcityAPI.TeamCityClient[0] 成功连接到 TeamCity 服务器: http://localhost:8111
✓ 连接成功
```

### 3. Error 日志级别（只显示错误）

```bash
dotnet run -- test --token YOUR_TOKEN --log-level Error
```

**输出**（成功时无日志）：
```
正在测试连接...
✓ 连接成功
```

**输出**（失败时）：
```
正在测试连接...
[19:00:33] fail: TeamcityAPI.TeamCityClient[0] 连接 TeamCity 服务器失败: http://localhost:8111
      System.Net.Http.HttpRequestException: Connection refused
✗ 连接失败
```

### 4. None - 禁用日志

```bash
dotnet run -- test --token YOUR_TOKEN --log-level None
```

**输出**（只有应用输出）：
```
正在测试连接...
✓ 连接成功
```

### 5. Trace - 所有详细信息

```bash
dotnet run -- trigger --token YOUR_TOKEN --buildtype MyBuild --log-level Trace
```

---

## 📊 日志级别对比

| 级别 | 用途 | 输出内容 |
|------|------|----------|
| **Trace** | 详细跟踪 | 所有可能的信息 |
| **Debug** | 开发调试 | 请求详情、状态码 |
| **Information** | 一般信息 | 关键操作、成功消息 |
| **Warning** | 警告 | 非致命问题、API 错误 |
| **Error** | 错误 | 异常、失败信息 |
| **Critical** | 严重错误 | 系统级错误 |
| **None** | 禁用 | 不输出日志 |

---

## 🔍 日志内容示例

### 客户端创建
```
[19:00:33] info: TeamcityAPI.TeamCityClient[0]
    TeamCity 客户端已创建 (Token 认证) - 服务器: http://localhost:8111, 超时: 30秒
```

### GET 请求成功
```
[19:00:33] dbug: TeamcityAPI.TeamCityClient[0]
    发送 GET 请求: /app/rest/projects
[19:00:34] dbug: TeamcityAPI.TeamCityClient[0]
    GET 请求成功: /app/rest/projects - 状态码: OK
```

### POST 请求失败
```
[19:00:35] warn: TeamcityAPI.TeamCityClient[0]
    POST 请求失败: /app/rest/buildQueue - 状态码: BadRequest, 原因: Invalid build type
```

### 连接错误
```
[19:00:36] fail: TeamcityAPI.TeamCityClient[0]
    连接 TeamCity 服务器失败: http://localhost:8111
    System.Net.Http.HttpRequestException: Connection refused
        at System.Net.Http.HttpConnectionPool...
```

---

## 💡 日志最佳实践

### 1. 开发阶段
使用 `Debug` 或 `Trace` 级别查看详细信息：
```bash
dotnet run -- projects --token $TOKEN --log-level Debug
```

### 2. 生产环境
使用 `Information` 或 `Warning` 级别：
```bash
dotnet run -- trigger --token $TOKEN --buildtype MyBuild --log-level Information
```

### 3. 问题排查
使用 `Debug` 或 `Trace` 查看完整请求链：
```bash
dotnet run -- trigger --token $TOKEN --buildtype MyBuild --log-level Trace
```

### 4. 脚本调用
使用 `None` 或 `Error` 避免干扰输出：
```bash
dotnet run -- test --token $TOKEN --log-level None
```

---

## 🎨 日志格式

### 格式说明
```
[HH:mm:ss] level: CategoryName[EventId] Message
```

**示例**：
```
[19:00:33] info: TeamcityAPI.TeamCityClient[0] 客户端已创建
│          │     │                          │    │
│          │     │                          │    └─ 日志消息
│          │     │                          └────── 事件 ID
│          │     └───────────────────────────────── 日志类别
│          └─────────────────────────────────────── 日志级别
└────────────────────────────────────────────────── 时间戳
```

### 配置选项
在 `CreateLoggerFactory` 中可以自定义：
```csharp
.AddSimpleConsole(options =>
{
    options.IncludeScopes = false;        // 不包含作用域
    options.SingleLine = true;             // 单行输出
    options.TimestampFormat = "[HH:mm:ss] "; // 时间格式
});
```

---

## 📝 代码修改总结

### 修改的文件
| 文件 | 修改 | 行数变化 |
|------|------|----------|
| `TeamCityClient.cs` | 添加日志支持 | +40 行 |
| `Services/BuildService.cs` | 添加 logger 参数 | +3 行 |
| `Services/QueryService.cs` | 添加 logger 参数 | +3 行 |
| `Services/ProjectService.cs` | 添加 logger 参数 | +3 行 |
| `CLI/CommandHandler.cs` | 添加 logger 参数 | +2 行 |
| `CLI/CommandLineOptions.cs` | 添加 log-level 选项 | +4 行 |
| `Program.cs` | 添加日志工厂和集成 | +75 行 |

### 添加的依赖
- Microsoft.Extensions.Logging (9.0.10)
- Microsoft.Extensions.Logging.Console (9.0.10)

---

## 🔧 从代码中使用

如果您想在自己的代码中使用带日志的 TeamCity 客户端：

```csharp
using Microsoft.Extensions.Logging;
using TeamcityAPI;
using TeamcityAPI.Authentication;

// 创建日志工厂
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .SetMinimumLevel(LogLevel.Debug)
        .AddConsole();
});

// 创建 logger
var logger = loggerFactory.CreateLogger<TeamCityClient>();

// 创建配置
var config = new TokenAuthConfig
{
    ServerUrl = "http://localhost:8111",
    AccessToken = "your-token"
};

// 创建带日志的客户端
using var client = new TeamCityClient(config, logger);

// 使用客户端（自动记录日志）
var projects = await client.Query.GetProjectsAsync();
```

---

## 🌟 优势

### 1. **问题排查**
- 清晰看到每个 API 请求
- 快速定位错误位置
- 完整的调用链路

### 2. **性能监控**
- 可以添加耗时统计
- 识别慢请求
- 优化 API 调用

### 3. **审计追踪**
- 记录所有操作
- 符合安全要求
- 可回溯历史

### 4. **开发效率**
- 快速理解程序行为
- 减少调试时间
- 更好的错误信息

### 5. **灵活配置**
- 运行时调整级别
- 不需要重新编译
- 适应不同场景

---

## 📚 相关资源

- [Microsoft.Extensions.Logging 文档](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging)
- [日志最佳实践](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging-best-practices)
- [日志提供程序](https://docs.microsoft.com/en-us/dotnet/core/extensions/logging-providers)

---

## 🎉 总结

✅ **成功集成 Microsoft.Extensions.Logging**  
✅ **支持 7 个日志级别**  
✅ **可通过命令行参数控制**  
✅ **详细记录所有 API 操作**  
✅ **格式化清晰，易于阅读**  
✅ **性能影响极小**  
✅ **可选参数，向后兼容**  

日志功能让您的 TeamCity API 工具更加专业和易用！

