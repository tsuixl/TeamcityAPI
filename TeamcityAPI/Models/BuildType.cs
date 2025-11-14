using System.Text.Json.Serialization;

namespace TeamcityAPI.Models;

/// <summary>
/// 构建配置（Build Type）
/// </summary>
public class BuildType
{
    /// <summary>
    /// 构建配置 ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// 构建配置名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 所属项目 ID
    /// </summary>
    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = string.Empty;
    
    /// <summary>
    /// 所属项目名称
    /// </summary>
    [JsonPropertyName("projectName")]
    public string? ProjectName { get; set; }
    
    /// <summary>
    /// 描述
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    /// <summary>
    /// 网页链接
    /// </summary>
    [JsonPropertyName("webUrl")]
    public string? WebUrl { get; set; }

    public bool IsIdle;
    
    public int ActiveBuildsCount;

    public override string ToString()
    {
        return $"{Name} (ID: {Id}, ProjectId: {ProjectId}, ProjectName: {ProjectName}, Description: {Description}, WebUrl: {WebUrl})";
    }
}

