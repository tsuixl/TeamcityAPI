# TeamCity API C# 封装库

这是一个用于 TeamCity REST API 的 C# 封装库，提供了简单易用的 API 来与 TeamCity 服务器进行交互。

## 功能特性

✅ **多种认证方式**
- Access Token 认证（Bearer Token）
- 基本认证（用户名/密码）

✅ **完整的功能覆盖**
- 项目查询和管理
- 构建配置查询
- 触发构建
- 取消构建
- 构建历史查询
- 构建搜索（支持多种条件）
- 构建代理查询

✅ **开发友好**
- 异步 API（async/await）
- 可配置的超时时间
- 完善的异常处理
- 分页支持（默认 20 条/页）
- 强类型模型

## 项目结构

```
TeamcityAPI/
├── Authentication/          # 认证模块
│   ├── AuthConfig.cs
│   ├── TokenAuthConfig.cs
│   ├── BasicAuthConfig.cs
│   └── ...
├── Services/               # 服务层
│   ├── BuildService.cs    # 构建操作
│   ├── QueryService.cs    # 查询功能
│   └── ProjectService.cs  # 项目管理
├── Models/                # 数据模型
│   ├── Build.cs
│   ├── Project.cs
│   ├── BuildType.cs
│   └── ...
├── Exceptions/            # 异常类
│   └── TeamCityApiException.cs
└── TeamCityClient.cs      # 主客户端
```

## 快速开始

### 1. Token 认证方式

```csharp
using TeamcityAPI;
using TeamcityAPI.Authentication;

// 创建客户端
var config = new TokenAuthConfig 
{ 
    ServerUrl = "https://teamcity.example.com",
    AccessToken = "your-access-token",
    Timeout = TimeSpan.FromSeconds(60)  // 可选，默认 30 秒
};

using var client = new TeamCityClient(config);

// 测试连接
var isConnected = await client.TestConnectionAsync();
Console.WriteLine($"连接状态: {isConnected}");
```

### 2. 用户名密码认证

```csharp
var config = new BasicAuthConfig 
{ 
    ServerUrl = "https://teamcity.example.com",
    Username = "admin",
    Password = "password"
};

using var client = new TeamCityClient(config);
```

## 主要功能示例

### 查询项目（分页）

```csharp
// 获取第一页项目（每页 20 条）
var projects = await client.Query.GetProjectsAsync(page: 1, pageSize: 20);

Console.WriteLine($"找到 {projects.Count} 个项目");
Console.WriteLine($"是否有更多数据: {projects.HasMore}");

foreach (var project in projects.Items)
{
    Console.WriteLine($"[{project.Id}] {project.Name}");
}
```

### 触发构建

```csharp
var options = new TriggerOptions
{
    BranchName = "main",
    Comment = "从 API 触发",
    Parameters = new Dictionary<string, string>
    {
        { "env.BUILD_VERSION", "1.0.0" },
        { "system.debug", "true" }
    }
};

var build = await client.Builds.TriggerBuildAsync("MyBuildConfig", options);
Console.WriteLine($"构建 ID: {build.Id}, 状态: {build.Status}");
```

### 取消构建

```csharp
var cancelled = await client.Builds.CancelBuildAsync(buildId, "用户手动取消");
Console.WriteLine($"取消结果: {(cancelled ? "成功" : "失败")}");
```

### 查询构建历史

```csharp
var history = await client.Builds.GetBuildHistoryAsync(
    buildTypeId: "MyBuildConfig", 
    page: 1, 
    pageSize: 20
);

foreach (var build in history.Items)
{
    Console.WriteLine($"#{build.Number} - {build.Status}");
    Console.WriteLine($"  开始时间: {build.StartDate}");
}
```

### 搜索构建

```csharp
var criteria = new BuildSearchCriteria
{
    BuildTypeId = "MyBuildConfig",
    Status = BuildStatus.Success,
    Branch = "main",
    Count = 10,
    SinceDate = DateTime.Now.AddDays(-7)  // 最近 7 天
};

var builds = await client.Query.SearchBuildsAsync(criteria);
Console.WriteLine($"找到 {builds.Count} 个符合条件的构建");
```

### 查询构建配置

```csharp
// 获取所有构建配置
var buildTypes = await client.Query.GetBuildTypesAsync();

// 获取指定项目的构建配置
var projectBuildTypes = await client.Projects.GetBuildConfigurationsAsync("ProjectId");
```

### 查询构建代理

```csharp
var agents = await client.Query.GetAgentsAsync(page: 1, pageSize: 20);

foreach (var agent in agents.Items)
{
    Console.WriteLine($"[{agent.Id}] {agent.Name}");
    Console.WriteLine($"  连接: {agent.Connected}, 启用: {agent.Enabled}");
}
```

## 错误处理

```csharp
try 
{
    var build = await client.Builds.TriggerBuildAsync("BuildConfig");
}
catch (TeamCityApiException ex)
{
    // TeamCity API 错误
    Console.WriteLine($"错误: {ex.Message}");
    Console.WriteLine($"状态码: {ex.StatusCode}");
    Console.WriteLine($"响应: {ex.ResponseContent}");
}
catch (TaskCanceledException)
{
    // 请求超时
    Console.WriteLine("请求超时");
}
catch (Exception ex)
{
    // 其他异常
    Console.WriteLine($"异常: {ex.Message}");
}
```

## API 服务说明

### BuildService（构建服务）

| 方法 | 描述 |
|------|------|
| `GetBuildAsync(buildId)` | 获取单个构建信息 |
| `GetBuildStatusAsync(buildId)` | 获取构建状态 |
| `TriggerBuildAsync(buildTypeId, options)` | 触发构建 |
| `CancelBuildAsync(buildId, comment)` | 取消构建 |
| `GetBuildHistoryAsync(buildTypeId, page, pageSize)` | 获取构建历史（分页） |
| `GetRunningBuildsAsync(buildTypeId, pageSize)` | 获取运行中的构建 |
| `GetQueuedBuildsAsync(pageSize)` | 获取排队中的构建 |

### QueryService（查询服务）

| 方法 | 描述 |
|------|------|
| `GetProjectsAsync(page, pageSize)` | 获取所有项目（分页） |
| `GetProjectAsync(projectId)` | 获取单个项目 |
| `GetBuildTypesAsync(projectId, page, pageSize)` | 获取构建配置列表 |
| `SearchBuildsAsync(criteria)` | 按条件搜索构建 |
| `GetAgentsAsync(page, pageSize)` | 获取构建代理列表 |

### ProjectService（项目服务）

| 方法 | 描述 |
|------|------|
| `GetProjectAsync(projectId)` | 获取项目详情 |
| `GetSubProjectsAsync(projectId, page, pageSize)` | 获取子项目 |
| `GetBuildConfigurationsAsync(projectId, page, pageSize)` | 获取项目的构建配置 |

## 数据模型

### Build（构建）

```csharp
public class Build
{
    public long Id { get; set; }              // 构建 ID
    public string Number { get; set; }        // 构建编号
    public BuildStatus Status { get; set; }   // 构建状态
    public string BuildTypeId { get; set; }   // 构建配置 ID
    public string BranchName { get; set; }    // 分支名称
    public DateTime? StartDate { get; set; }  // 开始时间
    public DateTime? FinishDate { get; set; } // 结束时间
    public BuildAgent Agent { get; set; }     // 构建代理
    public string WebUrl { get; set; }        // 网页链接
}
```

### BuildStatus（构建状态）

```csharp
public enum BuildStatus
{
    Unknown,    // 未知
    Queued,     // 排队中
    Running,    // 运行中
    Success,    // 成功
    Failure,    // 失败
    Cancelled   // 已取消
}
```

### PagedResponse（分页响应）

```csharp
public class PagedResponse<T>
{
    public List<T> Items { get; set; }     // 数据列表
    public int Count { get; set; }          // 总数量
    public int ReturnedCount { get; }       // 当前返回数量
    public bool HasMore { get; }            // 是否有更多数据
}
```

## 配置选项

### AuthConfig 配置

```csharp
// Token 认证
var tokenConfig = new TokenAuthConfig
{
    ServerUrl = "https://teamcity.example.com",  // 必填
    AccessToken = "your-token",                   // 必填
    Timeout = TimeSpan.FromSeconds(30)           // 可选，默认 30 秒
};

// 基本认证
var basicConfig = new BasicAuthConfig
{
    ServerUrl = "https://teamcity.example.com",  // 必填
    Username = "admin",                           // 必填
    Password = "password",                        // 必填
    Timeout = TimeSpan.FromSeconds(30)           // 可选，默认 30 秒
};
```

## 技术要求

- .NET 9.0
- 支持异步编程（async/await）
- 使用 System.Text.Json 进行 JSON 序列化
- 使用 CommandLineParser 2.9.1 进行命令行参数解析

## 依赖包

```xml
<PackageReference Include="CommandLineParser" Version="2.9.1" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.10" />
<PackageReference Include="Microsoft.Extensions.Logging.Console" Version="9.0.10" />
```

## 使用方式

本项目提供两种使用模式：

### 1. 示例模式（无参数）

```bash
cd TeamcityAPI
dotnet run
```

无参数运行时，将自动执行所有示例代码，演示各种 API 的使用方法。

### 2. 命令行模式（带参数）

**特性**：
- ✅ 基于 **CommandLineParser** 库，自动参数验证
- ✅ 自动生成专业的帮助文档
- ✅ 类型安全的参数解析
- ✅ 清晰的错误提示
- ✅ 集成 **Microsoft.Extensions.Logging**，支持详细日志

```bash
# 显示帮助（自动生成）
dotnet run -- --help
dotnet run -- trigger --help

# 测试连接
dotnet run -- test --token YOUR_TOKEN

# 查询项目
dotnet run -- projects --token YOUR_TOKEN --page 1 --pagesize 20

# 触发构建
dotnet run -- trigger --token YOUR_TOKEN --buildtype MyBuild --branch main

# 取消构建
dotnet run -- cancel --token YOUR_TOKEN --buildid 12345

# 查询构建信息
dotnet run -- get-build --token YOUR_TOKEN --buildid 12345

# 查询构建历史
dotnet run -- history --token YOUR_TOKEN --buildtype MyBuild --pagesize 10

# 查询构建代理
dotnet run -- agents --token YOUR_TOKEN

# 使用日志（Debug 级别）
dotnet run -- test --token YOUR_TOKEN --log-level Debug
```

**自动参数验证示例**：
```bash
$ dotnet run -- trigger
ERROR(S):
  Required option 'buildtype' is missing.
```

**日志输出示例**（Debug 级别）：
```bash
$ dotnet run -- test --token YOUR_TOKEN --log-level Debug
正在测试连接...
[19:00:33] info: TeamcityAPI.TeamCityClient[0] TeamCity 客户端已创建 (Token 认证)
[19:00:33] dbug: TeamcityAPI.TeamCityClient[0] 测试 TeamCity 服务器连接
[19:00:33] dbug: TeamcityAPI.TeamCityClient[0] 发送 GET 请求: /app/rest/server
[19:00:33] dbug: TeamcityAPI.TeamCityClient[0] GET 请求成功: /app/rest/server
[19:00:33] info: TeamcityAPI.TeamCityClient[0] 成功连接到 TeamCity 服务器
✓ 连接成功
```

**支持的日志级别**：`Trace`, `Debug`, `Information`（默认）, `Warning`, `Error`, `Critical`, `None`

详细的命令行使用说明请参考：
- [CLI_USAGE.md](CLI_USAGE.md) - 命令行使用指南
- [COMMANDLINEPARSER_INTEGRATION.md](COMMANDLINEPARSER_INTEGRATION.md) - CommandLineParser 集成说明
- [LOGGING_INTEGRATION.md](LOGGING_INTEGRATION.md) - 日志集成说明

## 许可证

本项目仅供学习和参考使用。

## 贡献

欢迎提交 Issue 和 Pull Request！

---

**注意**: 使用前请确保您的 TeamCity 服务器版本支持 REST API v2018.1+

