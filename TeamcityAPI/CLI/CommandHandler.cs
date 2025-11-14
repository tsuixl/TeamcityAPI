// using Microsoft.Extensions.Logging;
// using TeamcityAPI.Authentication;
// using TeamcityAPI.Exceptions;
// using TeamcityAPI.Models;
//
// namespace TeamcityAPI.CLI;
//
// /// <summary>
// /// 命令行处理器
// /// </summary>
// public class CommandHandler : IDisposable
// {
//     private readonly TeamCityClient _client;
//
//     public CommandHandler(AuthConfig config, ILogger<TeamCityClient>? logger = null)
//     {
//         _client = config is TokenAuthConfig tokenConfig 
//             ? new TeamCityClient(tokenConfig, logger) 
//             : new TeamCityClient((BasicAuthConfig)config, logger);
//     }
//
//     /// <summary>
//     /// 测试连接
//     /// </summary>
//     public async Task<int> TestConnectionAsync()
//     {
//         try
//         {
//             Console.WriteLine("正在测试连接...");
//             var isConnected = await _client.TestConnectionAsync();
//             
//             if (isConnected)
//             {
//                 Console.WriteLine("✓ 连接成功");
//                 return 0;
//             }
//             
//             Console.WriteLine("✗ 连接失败");
//             return 1;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"✗ 连接错误: {ex.Message}");
//             return 1;
//         }
//     }
//
//     /// <summary>
//     /// 查询项目列表
//     /// </summary>
//     public async Task<int> ListProjectsAsync(int page = 1, int pageSize = 20)
//     {
//         try
//         {
//             Console.WriteLine($"正在查询项目（第 {page} 页，每页 {pageSize} 条）...\n");
//             var projects = await _client.Query.GetProjectsAsync(page, pageSize);
//             
//             Console.WriteLine($"找到 {projects.Count} 个项目，当前返回 {projects.ReturnedCount} 个\n");
//             
//             foreach (var project in projects.Items)
//             {
//                 Console.WriteLine($"[{project.Id}] {project.Name}");
//                 if (!string.IsNullOrEmpty(project.Description))
//                     Console.WriteLine($"  描述: {project.Description}");
//                 if (!string.IsNullOrEmpty(project.WebUrl))
//                     Console.WriteLine($"  链接: {project.WebUrl}");
//                 Console.WriteLine();
//             }
//             
//             if (projects.HasMore)
//                 Console.WriteLine($"还有更多数据，使用 --page {page + 1} 查看下一页");
//             
//             return 0;
//         }
//         catch (TeamCityApiException ex)
//         {
//             Console.WriteLine($"✗ API 错误: {ex.Message} (状态码: {ex.StatusCode})");
//             return 1;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"✗ 错误: {ex.Message}");
//             return 1;
//         }
//     }
//
//     /// <summary>
//     /// 查询构建配置
//     /// </summary>
//     public async Task<int> ListBuildTypesAsync(string? projectId = null, int page = 1, int pageSize = 20)
//     {
//         try
//         {
//             var title = string.IsNullOrEmpty(projectId) 
//                 ? "所有构建配置" 
//                 : $"项目 [{projectId}] 的构建配置";
//             
//             Console.WriteLine($"正在查询{title}（第 {page} 页，每页 {pageSize} 条）...\n");
//             
//             var buildTypes = await _client.Query.GetBuildTypesAsync(projectId, page, pageSize);
//             
//             Console.WriteLine($"找到 {buildTypes.Count} 个构建配置，当前返回 {buildTypes.ReturnedCount} 个\n");
//             
//             foreach (var buildType in buildTypes.Items)
//             {
//                 Console.WriteLine($"[{buildType.Id}] {buildType.Name}");
//                 Console.WriteLine($"  项目: {buildType.ProjectName ?? buildType.ProjectId}");
//                 if (!string.IsNullOrEmpty(buildType.Description))
//                     Console.WriteLine($"  描述: {buildType.Description}");
//                 if (!string.IsNullOrEmpty(buildType.WebUrl))
//                     Console.WriteLine($"  链接: {buildType.WebUrl}");
//                 Console.WriteLine();
//             }
//             
//             if (buildTypes.HasMore)
//                 Console.WriteLine($"还有更多数据，使用 --page {page + 1} 查看下一页");
//             
//             return 0;
//         }
//         catch (TeamCityApiException ex)
//         {
//             Console.WriteLine($"✗ API 错误: {ex.Message} (状态码: {ex.StatusCode})");
//             return 1;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"✗ 错误: {ex.Message}");
//             return 1;
//         }
//     }
//
//     /// <summary>
//     /// 触发构建
//     /// </summary>
//     public async Task<int> TriggerBuildAsync(string buildTypeId, string? branch = null, 
//         string? comment = null, Dictionary<string, string>? parameters = null)
//     {
//         try
//         {
//             Console.WriteLine($"正在触发构建 [{buildTypeId}]...");
//             
//             var options = new TriggerOptions
//             {
//                 BranchName = branch,
//                 Comment = comment ?? "从命令行触发",
//                 Parameters = parameters ?? new Dictionary<string, string>()
//             };
//             
//             var build = await _client.Builds.TriggerBuildAsync(buildTypeId, options);
//             
//             if (build != null)
//             {
//                 Console.WriteLine($"\n✓ 构建已触发:");
//                 Console.WriteLine($"  构建 ID: {build.Id}");
//                 Console.WriteLine($"  构建编号: {build.Number}");
//                 Console.WriteLine($"  状态: {build.Status}");
//                 Console.WriteLine($"  分支: {build.BranchName ?? "默认"}");
//                 if (!string.IsNullOrEmpty(build.WebUrl))
//                     Console.WriteLine($"  链接: {build.WebUrl}");
//                 
//                 return 0;
//             }
//             
//             Console.WriteLine("✗ 触发构建失败：未返回构建信息");
//             return 1;
//         }
//         catch (TeamCityApiException ex)
//         {
//             Console.WriteLine($"✗ API 错误: {ex.Message} (状态码: {ex.StatusCode})");
//             return 1;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"✗ 错误: {ex.Message}");
//             return 1;
//         }
//     }
//
//     /// <summary>
//     /// 取消构建
//     /// </summary>
//     public async Task<int> CancelBuildAsync(string buildId, string? comment = null)
//     {
//         try
//         {
//             Console.WriteLine($"正在取消构建 [{buildId}]...");
//             
//             var cancelled = await _client.Builds.CancelBuildAsync(buildId, comment ?? "从命令行取消");
//             
//             if (cancelled)
//             {
//                 Console.WriteLine("✓ 构建已取消");
//                 return 0;
//             }
//             
//             Console.WriteLine("✗ 取消构建失败");
//             return 1;
//         }
//         catch (TeamCityApiException ex)
//         {
//             Console.WriteLine($"✗ API 错误: {ex.Message} (状态码: {ex.StatusCode})");
//             return 1;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"✗ 错误: {ex.Message}");
//             return 1;
//         }
//     }
//
//     /// <summary>
//     /// 获取构建信息
//     /// </summary>
//     public async Task<int> GetBuildAsync(string buildId)
//     {
//         try
//         {
//             Console.WriteLine($"正在查询构建 [{buildId}]...\n");
//             
//             var build = await _client.Builds.GetBuildAsync(buildId);
//             
//             if (build != null)
//             {
//                 Console.WriteLine($"构建 ID: {build.Id}");
//                 Console.WriteLine($"构建编号: {build.Number}");
//                 Console.WriteLine($"构建配置: {build.BuildTypeId}");
//                 Console.WriteLine($"状态: {build.Status}");
//                 Console.WriteLine($"分支: {build.BranchName ?? "默认"}");
//                 
//                 // if (build.StartDate.HasValue)
//                 //     Console.WriteLine($"开始时间: {build.StartDate:yyyy-MM-dd HH:mm:ss}");
//                 //
//                 // if (build.FinishDate.HasValue)
//                 //     Console.WriteLine($"结束时间: {build.FinishDate:yyyy-MM-dd HH:mm:ss}");
//                 
//                 if (build.Agent != null)
//                     Console.WriteLine($"构建代理: {build.Agent.Name}");
//                 
//                 if (!string.IsNullOrEmpty(build.WebUrl))
//                     Console.WriteLine($"链接: {build.WebUrl}");
//                 
//                 return 0;
//             }
//             
//             Console.WriteLine("✗ 未找到构建信息");
//             return 1;
//         }
//         catch (TeamCityApiException ex)
//         {
//             Console.WriteLine($"✗ API 错误: {ex.Message} (状态码: {ex.StatusCode})");
//             return 1;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"✗ 错误: {ex.Message}");
//             return 1;
//         }
//     }
//
//     /// <summary>
//     /// 查询构建历史
//     /// </summary>
//     public async Task<int> GetBuildHistoryAsync(string buildTypeId, int page = 1, int pageSize = 20)
//     {
//         try
//         {
//             Console.WriteLine($"正在查询构建历史 [{buildTypeId}]（第 {page} 页，每页 {pageSize} 条）...\n");
//             
//             var history = await _client.Builds.GetBuildHistoryAsync(buildTypeId, page, pageSize);
//             
//             Console.WriteLine($"找到 {history.Count} 条构建记录，当前返回 {history.ReturnedCount} 条\n");
//             
//             foreach (var build in history.Items)
//             {
//                 Console.WriteLine($"#{build.Number} - {build.Status}");
//                 Console.WriteLine($"  构建 ID: {build.Id}");
//                 Console.WriteLine($"  分支: {build.BranchName ?? "默认"}");
//                 
//                 // if (build.StartDate.HasValue)
//                 //     Console.WriteLine($"  开始: {build.StartDate:yyyy-MM-dd HH:mm:ss}");
//                 //
//                 // if (build.FinishDate.HasValue)
//                 //     Console.WriteLine($"  结束: {build.FinishDate:yyyy-MM-dd HH:mm:ss}");
//                 
//                 Console.WriteLine();
//             }
//             
//             if (history.HasMore)
//                 Console.WriteLine($"还有更多数据，使用 --page {page + 1} 查看下一页");
//             
//             return 0;
//         }
//         catch (TeamCityApiException ex)
//         {
//             Console.WriteLine($"✗ API 错误: {ex.Message} (状态码: {ex.StatusCode})");
//             return 1;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"✗ 错误: {ex.Message}");
//             return 1;
//         }
//     }
//
//     /// <summary>
//     /// 查询构建代理
//     /// </summary>
//     public async Task<int> ListAgentsAsync(int page = 1, int pageSize = 20)
//     {
//         try
//         {
//             Console.WriteLine($"正在查询构建代理（第 {page} 页，每页 {pageSize} 条）...\n");
//             
//             var agents = await _client.Query.GetAgentsAsync(page, pageSize);
//             
//             Console.WriteLine($"找到 {agents.Count} 个构建代理，当前返回 {agents.ReturnedCount} 个\n");
//             
//             foreach (var agent in agents.Items)
//             {
//                 Console.WriteLine($"[{agent.Id}] {agent.Name}");
//                 Console.WriteLine($"  连接: {(agent.Connected ? "已连接 ✓" : "未连接 ✗")}");
//                 Console.WriteLine($"  启用: {(agent.Enabled ? "是" : "否")}");
//                 Console.WriteLine();
//             }
//             
//             if (agents.HasMore)
//                 Console.WriteLine($"还有更多数据，使用 --page {page + 1} 查看下一页");
//             
//             return 0;
//         }
//         catch (TeamCityApiException ex)
//         {
//             Console.WriteLine($"✗ API 错误: {ex.Message} (状态码: {ex.StatusCode})");
//             return 1;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"✗ 错误: {ex.Message}");
//             return 1;
//         }
//     }
//
//     public void Dispose()
//     {
//         _client?.Dispose();
//     }
// }
//
