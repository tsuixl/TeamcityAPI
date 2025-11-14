using System.Text.Json.Serialization;

namespace TeamcityAPI.Models;

/// <summary>
/// 项目信息
/// </summary>
public class Project
{
    /// <summary>
    /// 项目 ID
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// 项目名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 父项目 ID
    /// </summary>
    [JsonPropertyName("parentProjectId")]
    public string? ParentProjectId { get; set; }
    
    /// <summary>
    /// 描述
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    /// <summary>
    /// 项目网页链接
    /// </summary>
    [JsonPropertyName("webUrl")]
    public string? WebUrl { get; set; }
    
    /// <summary>
    /// 是否归档
    /// </summary>
    [JsonPropertyName("archived")]
    public bool Archived { get; set; }
}

