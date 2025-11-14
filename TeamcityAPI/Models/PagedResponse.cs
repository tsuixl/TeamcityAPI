using System.Text.Json.Serialization;

namespace TeamcityAPI.Models;

/// <summary>
/// 分页响应
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class PagedResponse<T>
{
    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> Items { get; set; } = new();
    
    /// <summary>
    /// 总数量
    /// </summary>
    public int Count { get; set; }
    
    /// <summary>
    /// 当前返回的数量
    /// </summary>
    public int ReturnedCount => Items.Count;
    
    /// <summary>
    /// 是否有更多数据
    /// </summary>
    public bool HasMore => ReturnedCount < Count;
}

/// <summary>
/// TeamCity API 响应包装（用于解析 API 返回）
/// </summary>
internal class TeamCityListResponse<T>
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("build")]
    public List<T>? Build { get; set; }
    
    [JsonPropertyName("buildType")]
    public List<T>? BuildType { get; set; }
    
    [JsonPropertyName("project")]
    public List<T>? Project { get; set; }
    
    [JsonPropertyName("agent")]
    public List<T>? Agent { get; set; }
}

