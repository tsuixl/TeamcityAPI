using CommandLine;
using Microsoft.Extensions.Logging;
using TeamcityAPI;
using TeamcityAPI.Authentication;
using TeamcityAPI.CLI;
using TeamcityAPI.Examples;

// ============================================
// TeamCity API 命令行工具
// ============================================

// 如果没有参数，运行示例代码
if (args.Length == 0)
{
    await ExampleUsage.RunAllExamplesAsync();
    return 0;
}

// 使用 CommandLineParser 解析参数
var exitCode = await Parser.Default.ParseArguments<
    SvnTaskOptions,
    // TestOptions,
    // ProjectsOptions,
    // BuildsOptions,
    // TriggerBuildOptions,
    // CancelBuildOptions,
    // GetBuildOptions,
    // HistoryOptions,
    // AgentsOptions,
    ExampleOptions>(args)
    .MapResult(
        (SvnTaskOptions opts) => SvnTaskExecutor.ExecuteProjectsAsync(opts),
        // (TestOptions opts) => ExecuteTestAsync(opts),
        // (ProjectsOptions opts) => ExecuteProjectsAsync(opts),
        // (BuildsOptions opts) => ExecuteBuildsAsync(opts),
        // (TriggerBuildOptions opts) => ExecuteTriggerAsync(opts),
        // (CancelBuildOptions opts) => ExecuteCancelAsync(opts),
        // (GetBuildOptions opts) => ExecuteGetBuildAsync(opts),
        // (HistoryOptions opts) => ExecuteHistoryAsync(opts),
        // (AgentsOptions opts) => ExecuteAgentsAsync(opts),
        // (ExampleOptions opts) => ExecuteExampleAsync(),
        errs => Task.FromResult(1)
    );

return exitCode;

// ============================================
// 命令执行函数
// ============================================

// static async Task<int> ExecuteTestAsync(TestOptions opts)
// {
//     var config = CreateAuthConfig(opts);
//     if (config == null) return 1;
//
//     using var loggerFactory = CreateLoggerFactory(opts.LogLevel);
//     var logger = loggerFactory.CreateLogger<TeamCityClient>();
//     using var handler = new CommandHandler(config, logger);
//     return await handler.TestConnectionAsync();
// }

// static async Task<int> ExecuteProjectsAsync(ProjectsOptions opts)
// {
//     var config = CreateAuthConfig(opts);
//     if (config == null) return 1;
//
//     using var loggerFactory = CreateLoggerFactory(opts.LogLevel);
//     var logger = loggerFactory.CreateLogger<TeamCityClient>();
//     using var handler = new CommandHandler(config, logger);
//     return await handler.ListProjectsAsync(opts.Page, opts.PageSize);
// }
//
// static async Task<int> ExecuteBuildsAsync(BuildsOptions opts)
// {
//     var config = CreateAuthConfig(opts);
//     if (config == null) return 1;
//
//     using var loggerFactory = CreateLoggerFactory(opts.LogLevel);
//     var logger = loggerFactory.CreateLogger<TeamCityClient>();
//     using var handler = new CommandHandler(config, logger);
//     return await handler.ListBuildTypesAsync(opts.ProjectId, opts.Page, opts.PageSize);
// }
//
// static async Task<int> ExecuteTriggerAsync(TriggerBuildOptions opts)
// {
//     var config = CreateAuthConfig(opts);
//     if (config == null) return 1;
//
//     // 解析参数
//     var parameters = new Dictionary<string, string>();
//     if (opts.Parameters != null)
//     {
//         foreach (var param in opts.Parameters)
//         {
//             var parts = param.Split('=', 2);
//             if (parts.Length == 2)
//             {
//                 parameters[parts[0]] = parts[1];
//             }
//         }
//     }
//
//     using var loggerFactory = CreateLoggerFactory(opts.LogLevel);
//     var logger = loggerFactory.CreateLogger<TeamCityClient>();
//     using var handler = new CommandHandler(config, logger);
//     return await handler.TriggerBuildAsync(opts.BuildTypeId, opts.Branch, opts.Comment, parameters);
// }
//
// static async Task<int> ExecuteCancelAsync(CancelBuildOptions opts)
// {
//     var config = CreateAuthConfig(opts);
//     if (config == null) return 1;
//
//     using var loggerFactory = CreateLoggerFactory(opts.LogLevel);
//     var logger = loggerFactory.CreateLogger<TeamCityClient>();
//     using var handler = new CommandHandler(config, logger);
//     return await handler.CancelBuildAsync(opts.BuildId, opts.Comment);
// }
//
// static async Task<int> ExecuteGetBuildAsync(GetBuildOptions opts)
// {
//     var config = CreateAuthConfig(opts);
//     if (config == null) return 1;
//
//     using var loggerFactory = CreateLoggerFactory(opts.LogLevel);
//     var logger = loggerFactory.CreateLogger<TeamCityClient>();
//     using var handler = new CommandHandler(config, logger);
//     return await handler.GetBuildAsync(opts.BuildId);
// }

// static async Task<int> ExecuteHistoryAsync(HistoryOptions opts)
// {
//     var config = CreateAuthConfig(opts);
//     if (config == null) return 1;
//
//     using var loggerFactory = CreateLoggerFactory(opts.LogLevel);
//     var logger = loggerFactory.CreateLogger<TeamCityClient>();
//     using var handler = new CommandHandler(config, logger);
//     return await handler.GetBuildHistoryAsync(opts.BuildTypeId, opts.Page, opts.PageSize);
// }

// static async Task<int> ExecuteAgentsAsync(AgentsOptions opts)
// {
//     var config = CreateAuthConfig(opts);
//     if (config == null) return 1;
//
//     using var loggerFactory = CreateLoggerFactory(opts.LogLevel);
//     var logger = loggerFactory.CreateLogger<TeamCityClient>();
//     using var handler = new CommandHandler(config, logger);
//     return await handler.ListAgentsAsync(opts.Page, opts.PageSize);
// }

// ============================================
// 辅助函数
// ============================================

// static async Task<int> ExecuteExampleAsync()
// {
//     await ExampleUsage.RunAllExamplesAsync();
//     return 0;
// }
//
// static AuthConfig? CreateAuthConfig(GlobalOptions opts)
// {
//     // 验证认证参数
//     if (string.IsNullOrEmpty(opts.Token) && 
//         (string.IsNullOrEmpty(opts.Username) || string.IsNullOrEmpty(opts.Password)))
//     {
//         Console.WriteLine("错误: 必须提供 --token 或 --username 和 --password");
//         Console.WriteLine("使用 --help 查看帮助");
//         return null;
//     }
//
//     // 创建认证配置
//     if (!string.IsNullOrEmpty(opts.Token))
//     {
//         return new TokenAuthConfig
//         {
//             ServerUrl = opts.Server,
//             AccessToken = opts.Token,
//             Timeout = TimeSpan.FromSeconds(opts.Timeout)
//         };
//     }
//     else
//     {
//         return new BasicAuthConfig
//         {
//             ServerUrl = opts.Server,
//             Username = opts.Username!,
//             Password = opts.Password!,
//             Timeout = TimeSpan.FromSeconds(opts.Timeout)
//         };
//     }
// }
//
// static ILoggerFactory CreateLoggerFactory(string logLevel)
// {
//     // 解析日志级别
//     if (!Enum.TryParse<LogLevel>(logLevel, true, out var level))
//     {
//         level = LogLevel.Information;
//     }
//
//     return LoggerFactory.Create(builder =>
//     {
//         builder
//             .SetMinimumLevel(level)
//             .AddConsole(options =>
//             {
//                 options.FormatterName = "simple";
//             })
//             .AddSimpleConsole(options =>
//             {
//                 options.IncludeScopes = false;
//                 options.SingleLine = true;
//                 options.TimestampFormat = "[HH:mm:ss] ";
//             });
//     });
// }
