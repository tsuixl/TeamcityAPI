using Microsoft.Extensions.Logging;
using TeamcityAPI.Models;

namespace TeamcityAPI.Services;

/// <summary>
/// 查询服务
/// </summary>
public class QueryService
{
    private readonly TeamCityClient _client;
    private readonly ILogger? _logger;

    internal QueryService(TeamCityClient client, ILogger? logger = null)
    {
        _client = client;
        _logger = logger;
    }

    /// <summary>
    /// 获取所有项目（分页）
    /// </summary>
    /// <param name="page">页码（从 1 开始）</param>
    /// <param name="pageSize">每页数量（默认 20）</param>
    public async Task<PagedResponse<Project>> GetProjectsAsync(int page = 1, int pageSize = 20)
    {
        var start = (page - 1) * pageSize;
        var locator = $"count:{pageSize},start:{start}";
        var endpoint = $"/app/rest/projects?locator={locator}";
        
        var response = await _client.GetAsync<TeamCityListResponse<Project>>(endpoint);
        
        return new PagedResponse<Project>
        {
            Items = response?.Project ?? new List<Project>(),
            Count = response?.Count ?? 0
        };
    }

    /// <summary>
    /// 获取单个项目
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    public async Task<Project?> GetProjectAsync(string projectId)
    {
        if (string.IsNullOrEmpty(projectId))
            throw new ArgumentException("项目 ID 不能为空", nameof(projectId));
            
        return await _client.GetAsync<Project>($"/app/rest/projects/id:{projectId}");
    }

    /// <summary>
    /// 获取构建配置列表
    /// </summary>
    /// <param name="projectId">项目 ID（可选）</param>
    /// <param name="page">页码（从 1 开始）</param>
    /// <param name="pageSize">每页数量（默认 20）</param>
    public async Task<PagedResponse<BuildType>> GetBuildTypesAsync(string? projectId = null, int page = 1, int pageSize = 20)
    {
        var start = (page - 1) * pageSize;
        var locator = string.IsNullOrEmpty(projectId) 
            ? $"count:{pageSize},start:{start}" 
            : $"project:{projectId},count:{pageSize},start:{start}";
        var endpoint = $"/app/rest/buildTypes?locator={locator}";
        
        var response = await _client.GetAsync<TeamCityListResponse<BuildType>>(endpoint);
        
        return new PagedResponse<BuildType>
        {
            Items = response?.BuildType ?? new List<BuildType>(),
            Count = response?.Count ?? 0
        };
    }

    /// <summary>
    /// 搜索构建
    /// </summary>
    /// <param name="criteria">搜索条件</param>
    public async Task<PagedResponse<Build>> SearchBuildsAsync(BuildSearchCriteria criteria)
    {
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));

        var locatorParts = new List<string> { $"count:{criteria.Count}" };
        
        if (!string.IsNullOrEmpty(criteria.ProjectId))
            locatorParts.Add($"project:{criteria.ProjectId}");
            
        if (!string.IsNullOrEmpty(criteria.BuildTypeId))
            locatorParts.Add($"buildType:{criteria.BuildTypeId}");
            
        if (criteria.Status.HasValue)
            locatorParts.Add($"status:{criteria.Status.Value.ToString().ToUpper()}");
            
        if (!string.IsNullOrEmpty(criteria.Branch))
            locatorParts.Add($"branch:{criteria.Branch}");
            
        if (criteria.SinceDate.HasValue)
            locatorParts.Add($"sinceDate:{criteria.SinceDate.Value:yyyyMMdd'T'HHmmsszzz}");
            
        if (!string.IsNullOrEmpty(criteria.Tags))
            locatorParts.Add($"tags:{criteria.Tags}");

        var locator = string.Join(",", locatorParts);
        var endpoint = $"/app/rest/builds?locator={locator}";
        
        var response = await _client.GetAsync<TeamCityListResponse<Build>>(endpoint);
        
        return new PagedResponse<Build>
        {
            Items = response?.Build ?? new List<Build>(),
            Count = response?.Count ?? 0
        };
    }

    /// <summary>
    /// 获取构建代理列表
    /// </summary>
    /// <param name="page">页码（从 1 开始）</param>
    /// <param name="pageSize">每页数量（默认 20）</param>
    public async Task<PagedResponse<BuildAgent>> GetAgentsAsync(int page = 1, int pageSize = 20)
    {
        var start = (page - 1) * pageSize;
        var locator = $"count:{pageSize},start:{start}";
        var fields = "agent(id,name,connected,enabled,ip)";
        var endpoint = $"/app/rest/agents?locator={locator}&fields={fields}";

        var response = await _client.GetAsync<TeamCityListResponse<BuildAgent>>(endpoint);
        
        return new PagedResponse<BuildAgent>
        {
            Items = response?.Agent ?? new List<BuildAgent>(),
        };
    }
}

