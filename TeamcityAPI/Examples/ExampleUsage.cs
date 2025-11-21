using TeamcityAPI;
using TeamcityAPI.Authentication;
using TeamcityAPI.Exceptions;
using TeamcityAPI.Models;

namespace TeamcityAPI.Examples;

/// <summary>
/// TeamCity API 使用示例
/// </summary>
public static class ExampleUsage
{

    public static class ExampleDefine
    {
        public const string k_TestServerUrl = "http://10.11.4.168:8111";
        public const string k_TestAccessToken = "eyJ0eXAiOiAiVENWMiJ9.VWllUkVtOXU4am1zSVViREQ0SVFDOGdmYTRF.NWY4YjU1NDItMGIzYy00OTg4LWIyMjItZWM0N2M5MDEwZGVh";
        public const string k_TestUsername = "odin";
        public const string k_TestPassword = "123456";
    }
    
    /// <summary>
    /// 运行所有示例
    /// </summary>
    public static async Task RunAllExamplesAsync()
    {
        Console.WriteLine("=== TeamCity API C# 封装 - 使用示例 ===\n");
        
        // ============================================
        // 示例 1: 使用 Access Token 认证
        // ============================================
        Console.WriteLine("--- 示例 1: Token 认证 ---");
        try
        {
            var tokenConfig = new TokenAuthConfig
            {
                ServerUrl = ExampleDefine.k_TestServerUrl,
                AccessToken = ExampleDefine.k_TestAccessToken,
                Timeout = TimeSpan.FromSeconds(60)
            };

            using var client = new TeamCityClient(tokenConfig);
            
            // 测试连接
            Console.WriteLine("正在测试连接...");
            var isConnected = await client.TestConnectionAsync();
            Console.WriteLine($"连接状态: {(isConnected ? "成功" : "失败")}\n");
        }
        catch (TeamCityApiException ex)
        {
            Console.WriteLine($"API 错误: {ex.Message}");
            Console.WriteLine($"状态码: {ex.StatusCode}");
            if (!string.IsNullOrEmpty(ex.ResponseContent))
                Console.WriteLine($"响应内容: {ex.ResponseContent}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"异常: {ex.Message}");
        }

        // ============================================
        // 示例 2: 使用用户名密码认证
        // ============================================
        Console.WriteLine("\n--- 示例 2: 基本认证（用户名/密码） ---");
        try
        {
            var basicConfig = new BasicAuthConfig
            {
                ServerUrl = ExampleDefine.k_TestServerUrl,
                Username = ExampleDefine.k_TestUsername,
                Password = ExampleDefine.k_TestPassword,
                Timeout = TimeSpan.FromSeconds(30)
            };

            using var client = new TeamCityClient(basicConfig);
            
            Console.WriteLine("正在使用用户名/密码认证...");
            var isConnected = await client.TestConnectionAsync();
            Console.WriteLine($"连接状态: {(isConnected ? "成功" : "失败")}\n");
        }
        catch (TeamCityApiException ex)
        {
            Console.WriteLine($"API 错误: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"异常: {ex.Message}");
        }
        
        Console.WriteLine("\n--- 示例 查询Agent（分页） ---");
        await DemoQueryAgents();

        // // ============================================
        // // 示例 3: 查询项目
        // // ============================================
        // Console.WriteLine("\n--- 示例 3: 查询项目（分页） ---");
        // await DemoQueryProjects();
        //
        // // ============================================
        // // 示例 4: 查询构建配置
        // // ============================================
        // Console.WriteLine("\n--- 示例 4: 查询构建配置 ---");
        // await DemoQueryBuildTypes();
        //
        // // ============================================
        // // 示例 5: 触发构建
        // // ============================================
        // Console.WriteLine("\n--- 示例 5: 触发构建 ---");
        // await DemoTriggerBuild();
        //
        // // ============================================
        // // 示例 6: 查询构建历史
        // // ============================================
        // Console.WriteLine("\n--- 示例 6: 查询构建历史 ---");
        // await DemoQueryBuildHistory();
        //
        // // ============================================
        // // 示例 7: 搜索构建
        // // ============================================
        // Console.WriteLine("\n--- 示例 7: 按条件搜索构建 ---");
        // await DemoSearchBuilds();
        //
        // // ============================================
        // // 示例 8: 取消构建
        // // ============================================
        // Console.WriteLine("\n--- 示例 8: 取消构建 ---");
        // await DemoCancelBuild();
        //
        // // ============================================
        // // 示例 9: 查询构建代理
        // // ============================================
        // Console.WriteLine("\n--- 示例 9: 查询构建代理 ---");
        // await DemoQueryAgents();
        //
        // // ============================================
        // // 示例 10: 错误处理和超时
        // // ============================================
        // Console.WriteLine("\n--- 示例 10: 错误处理和超时 ---");
        // await DemoErrorHandling();
        //
        // Console.WriteLine("\n=== 所有示例完成 ===");
    }

    // ============================================
    // 演示函数
    // ============================================

    private static async Task DemoQueryProjects()
    {
        try
        {
            var config = new TokenAuthConfig
            {
                ServerUrl = ExampleDefine.k_TestServerUrl,
                AccessToken = ExampleDefine.k_TestAccessToken
            };

            using var client = new TeamCityClient(config);
            
            // 获取第一页项目（每页 20 条）
            var projects = await client.Query.GetProjectsAsync(page: 1, pageSize: 20);
            
            Console.WriteLine($"找到 {projects.Count} 个项目，当前返回 {projects.ReturnedCount} 个");
            Console.WriteLine($"是否有更多数据: {projects.HasMore}");
            
            foreach (var project in projects.Items.Take(5))
            {
                Console.WriteLine($"  - [{project.Id}] {project.Name}");
                if (!string.IsNullOrEmpty(project.Description))
                    Console.WriteLine($"    描述: {project.Description}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"查询项目失败: {ex.Message}");
        }
    }

    private static async Task DemoQueryBuildTypes()
    {
        try
        {
            var config = new TokenAuthConfig
            {
                ServerUrl = ExampleDefine.k_TestServerUrl,
                AccessToken = ExampleDefine.k_TestAccessToken
            };

            using var client = new TeamCityClient(config);
            
            // 获取所有构建配置
            var buildTypes = await client.Query.GetBuildTypesAsync();
            
            Console.WriteLine($"找到 {buildTypes.Count} 个构建配置");
            
            foreach (var buildType in buildTypes.Items.Take(5))
            {
                Console.WriteLine($"  - [{buildType.Id}] {buildType.Name}");
                Console.WriteLine($"    项目: {buildType.ProjectName ?? buildType.ProjectId}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"查询构建配置失败: {ex.Message}");
        }
    }

    private static async Task DemoTriggerBuild()
    {
        try
        {
            var config = new TokenAuthConfig
            {
                ServerUrl = ExampleDefine.k_TestServerUrl,
                AccessToken = ExampleDefine.k_TestAccessToken
            };

            using var client = new TeamCityClient(config);
            
            // 触发构建
            var options = new TriggerOptions
            {
                BranchName = "main",
                Comment = "从 API 触发的构建",
                Parameters = new Dictionary<string, string>
                {
                    { "env.BUILD_VERSION", "1.0.0" },
                    { "system.debug", "true" }
                }
            };
            
            var build = await client.Builds.TriggerBuildAsync("MyBuildConfig", options);
            
            if (build != null)
            {
                Console.WriteLine($"构建已触发:");
                Console.WriteLine($"  - 构建 ID: {build.Id}");
                Console.WriteLine($"  - 构建编号: {build.Number}");
                Console.WriteLine($"  - 状态: {build.Status}");
                Console.WriteLine($"  - 分支: {build.BranchName ?? "默认"}");
                if (!string.IsNullOrEmpty(build.WebUrl))
                    Console.WriteLine($"  - 链接: {build.WebUrl}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"触发构建失败: {ex.Message}");
        }
    }

    private static async Task DemoQueryBuildHistory()
    {
        try
        {
            var config = new TokenAuthConfig
            {
                ServerUrl = ExampleDefine.k_TestServerUrl,
                AccessToken = ExampleDefine.k_TestAccessToken
            };

            using var client = new TeamCityClient(config);
            
            // 获取构建历史
            var history = await client.Builds.GetBuildHistoryAsync("MyBuildConfig", page: 1, pageSize: 10);
            
            Console.WriteLine($"构建历史（共 {history.Count} 条）:");
            
            foreach (var build in history.Items.Take(5))
            {
                Console.WriteLine($"  - #{build.Number} ({build.Status})");
                // Console.WriteLine($"    开始: {build.StartDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "未知"}");
                // if (build.FinishDate.HasValue)
                //     Console.WriteLine($"    结束: {build.FinishDate:yyyy-MM-dd HH:mm:ss}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"查询构建历史失败: {ex.Message}");
        }
    }

    private static async Task DemoSearchBuilds()
    {
        try
        {
            var config = new TokenAuthConfig
            {
                ServerUrl = ExampleDefine.k_TestServerUrl,
                AccessToken = ExampleDefine.k_TestAccessToken
            };

            using var client = new TeamCityClient(config);
            
            // 搜索成功的构建
            var criteria = new BuildSearchCriteria
            {
                BuildTypeId = "MyBuildConfig",
                Status = BuildStatus.Success,
                Branch = "main",
                Count = 10,
                SinceDate = DateTime.Now.AddDays(-7) // 最近 7 天
            };
            
            var builds = await client.Query.SearchBuildsAsync(criteria);
            
            Console.WriteLine($"搜索结果（共 {builds.Count} 条）:");
            
            foreach (var build in builds.Items)
            {
                Console.WriteLine($"  - #{build.Number} - {build.Status}");
                Console.WriteLine($"    分支: {build.BranchName ?? "默认"}");
                // Console.WriteLine($"    时间: {build.StartDate?.ToString("yyyy-MM-dd HH:mm:ss") ?? "未知"}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"搜索构建失败: {ex.Message}");
        }
    }

    private static async Task DemoCancelBuild()
    {
        try
        {
            var config = new TokenAuthConfig
            {
                ServerUrl = ExampleDefine.k_TestServerUrl,
                AccessToken = ExampleDefine.k_TestAccessToken
            };

            using var client = new TeamCityClient(config);
            
            // 假设有一个运行中的构建 ID
            var buildId = "12345";
            
            // 取消构建
            var cancelled = await client.Builds.CancelBuildAsync(buildId, "用户手动取消");
            
            Console.WriteLine($"取消构建结果: {(cancelled ? "成功" : "失败")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"取消构建失败: {ex.Message}");
        }
    }

    private static async Task DemoQueryAgents()
    {
        try
        {
            var config = new TokenAuthConfig
            {
                ServerUrl = ExampleDefine.k_TestServerUrl,
                AccessToken = ExampleDefine.k_TestAccessToken
            };

            using var client = new TeamCityClient(config);
            
            // 获取构建代理
            var agents = await client.Query.GetAgentsAsync(page: 1, pageSize: 20);
            
            Console.WriteLine($"获取到 {agents.ReturnedCount} 个构建代理:");
            
            foreach (var agent in agents.Items.Take(5))
            {
                Console.WriteLine(agent);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"查询代理失败: {ex.Message}");
        }
    }

    private static async Task DemoErrorHandling()
    {
        try
        {
            // 故意使用错误的配置来演示错误处理
            var config = new TokenAuthConfig
            {
                ServerUrl = ExampleDefine.k_TestServerUrl,
                AccessToken = ExampleDefine.k_TestAccessToken,
                Timeout = TimeSpan.FromSeconds(5) // 5 秒超时
            };

            using var client = new TeamCityClient(config);
            
            Console.WriteLine("尝试连接到无效的服务器...");
            var isConnected = await client.TestConnectionAsync();
            Console.WriteLine($"连接状态: {isConnected}");
        }
        catch (TeamCityApiException ex)
        {
            Console.WriteLine($"✓ 捕获 TeamCityApiException:");
            Console.WriteLine($"  - 消息: {ex.Message}");
            Console.WriteLine($"  - 状态码: {ex.StatusCode}");
            if (!string.IsNullOrEmpty(ex.ResponseContent))
                Console.WriteLine($"  - 响应: {ex.ResponseContent.Substring(0, Math.Min(100, ex.ResponseContent.Length))}...");
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("✓ 捕获超时异常: 请求已超时");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✓ 捕获其他异常: {ex.GetType().Name} - {ex.Message}");
        }
    }
}

