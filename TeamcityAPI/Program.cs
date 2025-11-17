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
    ExampleOptions>(args)
    .MapResult(
        (SvnTaskOptions opts) => SvnTaskExecutor.ExecuteProjectsAsync(opts),
        errs => Task.FromResult(1)
    );

return exitCode;