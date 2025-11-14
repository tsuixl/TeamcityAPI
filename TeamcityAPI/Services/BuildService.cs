using System.Text.Json;
using Microsoft.Extensions.Logging;
using TeamcityAPI.Models;

namespace TeamcityAPI.Services;

/// <summary>
/// 构建服务
/// </summary>
public class BuildService
{
    private readonly TeamCityClient _client;
    private readonly ILogger? _logger;

    internal BuildService(TeamCityClient client, ILogger? logger = null)
    {
        _client = client;
        _logger = logger;
    }

    /// <summary>
    /// 获取单个构建信息
    /// </summary>
    /// <param name="buildId">构建 ID</param>
    public async Task<Build?> GetBuildAsync(string buildId)
    {
        if (string.IsNullOrEmpty(buildId))
            throw new ArgumentException("构建 ID 不能为空", nameof(buildId));
            
        return await _client.GetAsync<Build>($"/app/rest/builds/id:{buildId}");
    }

    /// <summary>
    /// 获取构建状态
    /// </summary>
    /// <param name="buildId">构建 ID</param>
    public async Task<BuildStatus> GetBuildStatusAsync(string buildId)
    {
        var build = await GetBuildAsync(buildId);
        return build?.Status ?? BuildStatus.Unknown;
    }

    /// <summary>
    /// 触发构建
    /// </summary>
    /// <param name="buildTypeId">构建配置 ID</param>
    /// <param name="options">触发选项（可选）</param>
    public async Task<Build?> TriggerBuildAsync(string buildTypeId, TriggerOptions? options = null)
    {
        if (string.IsNullOrEmpty(buildTypeId))
            throw new ArgumentException("构建配置 ID 不能为空", nameof(buildTypeId));

        options ??= new TriggerOptions();

        // 构造请求体
        var triggerRequest = new
        {
            buildType = new { id = buildTypeId },
            branchName = options.BranchName,
            comment = new { text = options.Comment ?? string.Empty },
            personal = options.Personal,
            properties = options.Parameters.Count > 0 
                ? new
                {
                    property = options.Parameters.Select(p => new
                    {
                        name = p.Key,
                        value = p.Value
                    }).ToList()
                }
                : null
        };

        return await _client.PostAsync<object, Build>("/app/rest/buildQueue", triggerRequest);
    }

    public async Task<bool> CancelBuildAsync(long buildId, string comment = "")
    {
        return await CancelBuildAsync(buildId.ToString(), comment);
    }
    
    /// <summary>
    /// 取消构建
    /// </summary>
    /// <param name="buildId">构建 ID</param>
    /// <param name="comment">取消原因</param>
    public async Task<bool> CancelBuildAsync(string buildId, string comment = "")
    {
        if (string.IsNullOrEmpty(buildId))
            throw new ArgumentException("构建 ID 不能为空", nameof(buildId));

        try
        {
            var cancelRequest = new
            {
                comment = comment,
                readdIntoQueue = false
            };

            await _client.PostAsync($"/app/rest/builds/id:{buildId}", cancelRequest);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取构建历史（分页）
    /// </summary>
    /// <param name="buildTypeId">构建配置 ID</param>
    /// <param name="page">页码（从 1 开始）</param>
    /// <param name="pageSize">每页数量（默认 20）</param>
    public async Task<PagedResponse<Build>> GetBuildHistoryAsync(string buildTypeId, int page = 1, int pageSize = 20)
    {
        if (string.IsNullOrEmpty(buildTypeId))
            throw new ArgumentException("构建配置 ID 不能为空", nameof(buildTypeId));

        var start = (page - 1) * pageSize;
        var locator = $"buildType:{buildTypeId},count:{pageSize},start:{start}";
        var endpoint = $"/app/rest/builds?locator={locator}";
        
        var response = await _client.GetAsync<TeamCityListResponse<Build>>(endpoint);
        
        return new PagedResponse<Build>
        {
            Items = response?.Build ?? new List<Build>(),
            Count = response?.Count ?? 0
        };
    }

    /// <summary>
    /// 获取运行中的构建
    /// </summary>
    /// <param name="buildTypeId">构建配置 ID(可选)</param>
    /// <param name="pageSize">返回数量(默认 20)</param>
    public async Task<PagedResponse<Build>> GetRunningBuildsAsync(string? buildTypeId = null, int pageSize = 20)
    {
        return await GetBuildsAsync("builds", buildTypeId, "running:true", pageSize);
    }

    /// <summary>
    /// 获取排队中的构建
    /// </summary>
    /// <param name="buildTypeId">构建配置 ID(可选)</param>
    /// <param name="pageSize">返回数量(默认 20)</param>
    public async Task<PagedResponse<Build>> GetQueuedBuildsAsync(string? buildTypeId = null, int pageSize = 20)
    {
        return await GetBuildsAsync("buildQueue", buildTypeId, null, pageSize);
    }
    
    /// <summary>
    /// 获取运行中和排队构建信息
    /// </summary>
    /// <param name="buildTypeId">构建配置 ID（可选）</param>
    /// <param name="pageSize">每种状态返回数量（默认 20）</param>
    /// <returns>包含运行中和排队中的所有构建</returns>
    public async Task<PagedResponse<Build>> GetActiveBuildsAsync(string? buildTypeId = null, int pageSize = 20)
    {
        // 并行获取运行中和排队中的构建，提高性能
        var runningTask = GetRunningBuildsAsync(buildTypeId, pageSize);
        var queuedTask = GetQueuedBuildsAsync(buildTypeId, pageSize);
    
        await Task.WhenAll(runningTask, queuedTask);
    
        var runningBuilds = await runningTask;
        var queuedBuilds = await queuedTask;
        
        var allBuilds = new List<Build>();
        allBuilds.AddRange(runningBuilds.Items);
        allBuilds.AddRange(queuedBuilds.Items);
    
        return new PagedResponse<Build>
        {
            Items = allBuilds,
            Count = runningBuilds.Count + queuedBuilds.Count
        };
    }

    /// <summary>
    /// 构建信息获取
    /// </summary>
    /// <param name="endpoint">端点名称(builds 或 buildQueue)</param>
    /// <param name="buildTypeId">构建配置 ID(可选)</param>
    /// <param name="additionalLocator">额外的定位器条件(可选)</param>
    /// <param name="pageSize">返回数量</param>
    private async Task<PagedResponse<Build>> GetBuildsAsync(
        string endpoint, 
        string? buildTypeId, 
        string? additionalLocator, 
        int pageSize)
    {
        var locatorParts = new List<string> { $"count:{pageSize}" };
    
        if (!string.IsNullOrEmpty(buildTypeId))
            locatorParts.Insert(0, $"buildType:{buildTypeId}");
    
        if (!string.IsNullOrEmpty(additionalLocator))
            locatorParts.Insert(string.IsNullOrEmpty(buildTypeId) ? 0 : 1, additionalLocator);
    
        var locator = string.Join(",", locatorParts);
    
        var fields = "count,build(id,number,status,state,statusText,buildTypeId,branchName,agent,webUrl," +
                     "properties(property(name,value))," +
                     "tags(tag(name)))";

        var url = $"/app/rest/{endpoint}?locator={locator}&fields={fields}";
        var response = await _client.GetAsync<TeamCityListResponse<Build>>(url);

        return new PagedResponse<Build>
        {
            Items = response?.Build ?? new List<Build>(),
            Count = response?.Count ?? 0
        };
    }
}

