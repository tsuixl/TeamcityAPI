using System.Text.Json.Serialization;

namespace TeamcityAPI.Models;

/// <summary>
/// 构建代理
/// </summary>
public class BuildAgent
{
    /// <summary>
    /// 代理 ID
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    /// <summary>
    /// 代理名称
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// 是否已连接
    /// </summary>
    [JsonPropertyName("connected")]
    public bool Connected { get; set; }
    
    /// <summary>
    /// 是否已启用
    /// </summary>
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    /// IP 地址
    /// </summary>
    [JsonPropertyName("ip")]
    public string? IpAddress { get; set; }

    public override string ToString()
    {
        return $"{Name} (ID: {Id}, Connected: {Connected}, Enabled: {Enabled}, IP: {IpAddress})";
    }
}

