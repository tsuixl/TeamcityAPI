using Microsoft.Extensions.Logging;
using TeamcityAPI.Models;

namespace TeamcityAPI.Services;

/// <summary>
/// 项目服务
/// </summary>
public class ProjectService
{
    private readonly TeamCityClient _client;
    private readonly ILogger? _logger;

    internal ProjectService(TeamCityClient client, ILogger? logger = null)
    {
        _client = client;
        _logger = logger;
    }

    /// <summary>
    /// 获取项目详情
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    public async Task<Project?> GetProjectAsync(string projectId)
    {
        if (string.IsNullOrEmpty(projectId))
            throw new ArgumentException("项目 ID 不能为空", nameof(projectId));
            
        return await _client.GetAsync<Project>($"/app/rest/projects/id:{projectId}");
    }

    /// <summary>
    /// 获取子项目
    /// </summary>
    /// <param name="projectId">父项目 ID</param>
    /// <param name="page">页码（从 1 开始）</param>
    /// <param name="pageSize">每页数量（默认 20）</param>
    public async Task<PagedResponse<Project>> GetSubProjectsAsync(string projectId, int page = 1, int pageSize = 20)
    {
        if (string.IsNullOrEmpty(projectId))
            throw new ArgumentException("项目 ID 不能为空", nameof(projectId));

        var start = (page - 1) * pageSize;
        var locator = $"affectedProject:{projectId},count:{pageSize},start:{start}";
        var endpoint = $"/app/rest/projects?locator={locator}";
        
        var response = await _client.GetAsync<TeamCityListResponse<Project>>(endpoint);
        
        return new PagedResponse<Project>
        {
            Items = response?.Project ?? new List<Project>(),
            Count = response?.Count ?? 0
        };
    }

    /// <summary>
    /// 获取项目的构建配置
    /// </summary>
    /// <param name="projectId">项目 ID</param>
    /// <param name="page">页码（从 1 开始）</param>
    /// <param name="pageSize">每页数量（默认 20）</param>
    public async Task<PagedResponse<BuildType>> GetBuildConfigurationsAsync(string projectId, int page = 1, int pageSize = 20)
    {
        if (string.IsNullOrEmpty(projectId))
            throw new ArgumentException("项目 ID 不能为空", nameof(projectId));

        var start = (page - 1) * pageSize;
        var locator = $"project:{projectId},count:{pageSize},start:{start}";
        var endpoint = $"/app/rest/buildTypes?locator={locator}";
        
        var response = await _client.GetAsync<TeamCityListResponse<BuildType>>(endpoint);
        
        return new PagedResponse<BuildType>
        {
            Items = response?.BuildType ?? new List<BuildType>(),
            Count = response?.Count ?? 0
        };
    }
    
    /// <summary>
    /// 根据名称模糊搜索构建配置
    /// </summary>
    /// <param name="buildName">构建名称</param>
    /// <param name="projectId">可选的项目 ID，用于限制搜索范围</param>
    /// <param name="page">页码（从 1 开始）</param>
    /// <param name="pageSize">每页数量（默认 20）</param>
    public async Task<PagedResponse<BuildType>> SearchBuildConfigurationsByNameAsync(
        string? buildName = null, 
        string? projectId = null, 
        int page = 1, 
        int pageSize = 20)
    {
        var start = (page - 1) * pageSize;
        var locatorParts = new List<string>();
    
        // 名称
        if (!string.IsNullOrEmpty(buildName))
        {
            locatorParts.Add($"name:{buildName}");
        }
    
        // 如果提供了项目 ID，则限制搜索范围
        if (!string.IsNullOrEmpty(projectId))
        {
            locatorParts.Add($"project:{projectId}");
        }
    
        // 添加分页参数
        locatorParts.Add($"count:{pageSize}");
        locatorParts.Add($"start:{start}");
    
        var locator = string.Join(",", locatorParts);
        var endpoint = $"/app/rest/buildTypes?locator={locator}";
    
        _logger?.LogDebug("搜索构建配置: 关键词={NamePattern}, 项目={ProjectId}, API={Endpoint}", 
            buildName, projectId ?? "全部", endpoint);
    
        var response = await _client.GetAsync<TeamCityListResponse<BuildType>>(endpoint);
    
        return new PagedResponse<BuildType>
        {
            Items = response?.BuildType ?? new List<BuildType>(),
            Count = response?.Count ?? 0
        };
    }
    
}

