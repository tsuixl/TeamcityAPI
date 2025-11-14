using CommandLine;

namespace TeamcityAPI.CLI;

/// <summary>
/// 全局连接选项
/// </summary>
public class GlobalOptions
{
    [Option('s', "server", Required = false, Default = "http://localhost:8111", HelpText = "TeamCity 服务器地址")]
    public string Server { get; set; } = "http://localhost:8111";

    [Option('t', "token", Required = false, HelpText = "Access Token（Token 或 用户名/密码 二选一）")]
    public string? Token { get; set; }

    [Option('u', "username", Required = false, HelpText = "用户名（基本认证）")]
    public string? Username { get; set; }

    [Option('p', "password", Required = false, HelpText = "密码（基本认证）")]
    public string? Password { get; set; }

    [Option("timeout", Required = false, Default = 30, HelpText = "超时时间（秒）")]
    public int Timeout { get; set; } = 30;

    [Option("log-level", Required = false, Default = "Debug", HelpText = "日志级别 (Trace, Debug, Information, Warning, Error, Critical, None)")]
    public string LogLevel { get; set; } = "Information";
}

// /// <summary>
// /// 测试连接命令
// /// </summary>
// [Verb("test", HelpText = "测试与 TeamCity 服务器的连接")]
// public class TestOptions : GlobalOptions
// {
// }
//
// /// <summary>
// /// 查询项目命令
// /// </summary>
// [Verb("projects", HelpText = "查询项目列表")]
// public class ProjectsOptions : GlobalOptions
// {
//     [Option("page", Required = false, Default = 1, 
//         HelpText = "页码（从 1 开始）")]
//     public int Page { get; set; } = 1;
//
//     [Option("pagesize", Required = false, Default = 20, 
//         HelpText = "每页数量")]
//     public int PageSize { get; set; } = 20;
// }
//
// /// <summary>
// /// 查询构建配置命令
// /// </summary>
// [Verb("builds", HelpText = "查询构建配置列表")]
// public class BuildsOptions : GlobalOptions
// {
//     [Option("project", Required = false, 
//         HelpText = "项目 ID（可选，不指定则查询所有）")]
//     public string? ProjectId { get; set; }
//
//     [Option("page", Required = false, Default = 1, 
//         HelpText = "页码")]
//     public int Page { get; set; } = 1;
//
//     [Option("pagesize", Required = false, Default = 20, 
//         HelpText = "每页数量")]
//     public int PageSize { get; set; } = 20;
// }
//
// /// <summary>
// /// 触发构建命令
// /// </summary>
// [Verb("trigger", HelpText = "触发构建")]
// public class TriggerBuildOptions : GlobalOptions
// {
//     [Option("buildtype", Required = true, 
//         HelpText = "构建配置 ID（必需）")]
//     public string BuildTypeId { get; set; } = string.Empty;
//
//     [Option("branch", Required = false, 
//         HelpText = "分支名称")]
//     public string? Branch { get; set; }
//
//     [Option("comment", Required = false, 
//         HelpText = "构建注释")]
//     public string? Comment { get; set; }
//
//     [Option("param", Required = false, Separator = ',',
//         HelpText = "构建参数，格式：key=value,key2=value2")]
//     public IEnumerable<string>? Parameters { get; set; }
// }
//
// /// <summary>
// /// 取消构建命令
// /// </summary>
// [Verb("cancel", HelpText = "取消构建")]
// public class CancelBuildOptions : GlobalOptions
// {
//     [Option("buildid", Required = true, 
//         HelpText = "构建 ID（必需）")]
//     public string BuildId { get; set; } = string.Empty;
//
//     [Option("comment", Required = false, 
//         HelpText = "取消原因")]
//     public string? Comment { get; set; }
// }
//
// /// <summary>
// /// 获取构建信息命令
// /// </summary>
// [Verb("get-build", HelpText = "获取构建详细信息")]
// public class GetBuildOptions : GlobalOptions
// {
//     [Option("buildid", Required = true, 
//         HelpText = "构建 ID（必需）")]
//     public string BuildId { get; set; } = string.Empty;
// }
//
// /// <summary>
// /// 查询构建历史命令
// /// </summary>
// [Verb("history", HelpText = "查询构建历史")]
// public class HistoryOptions : GlobalOptions
// {
//     [Option("buildtype", Required = true, 
//         HelpText = "构建配置 ID（必需）")]
//     public string BuildTypeId { get; set; } = string.Empty;
//
//     [Option("page", Required = false, Default = 1, 
//         HelpText = "页码")]
//     public int Page { get; set; } = 1;
//
//     [Option("pagesize", Required = false, Default = 20, 
//         HelpText = "每页数量")]
//     public int PageSize { get; set; } = 20;
// }
//
// /// <summary>
// /// 查询构建代理命令
// /// </summary>
// [Verb("agents", HelpText = "查询构建代理列表")]
// public class AgentsOptions : GlobalOptions
// {
//     [Option("page", Required = false, Default = 1, 
//         HelpText = "页码")]
//     public int Page { get; set; } = 1;
//
//     [Option("pagesize", Required = false, Default = 20, 
//         HelpText = "每页数量")]
//     public int PageSize { get; set; } = 20;
// }

/// <summary>
/// 运行示例命令
/// </summary>
[Verb("example", HelpText = "运行使用示例")]
public class ExampleOptions
{
}

