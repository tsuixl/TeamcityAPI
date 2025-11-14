using System.Text.Json.Serialization;

namespace TeamcityAPI.Models;

/// <summary>
/// 构建参数集合
/// </summary>
public class BuildProperties
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("property")]
    public List<BuildProperty> Property { get; set; } = new();
}

/// <summary>
/// 构建参数
/// </summary>
public class BuildProperty
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;
    
    // [JsonPropertyName("inherited")]
    // public bool? Inherited { get; set; }
}

/// <summary>
/// 构建标签集合
/// </summary>
public class BuildTags
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("tag")]
    public List<BuildTag> Tag { get; set; } = new();
}

/// <summary>
/// 构建标签
/// </summary>
public class BuildTag
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}


/// <summary>
/// 构建信息
/// </summary>
public class Build
{
    /// <summary>
    /// 构建 ID
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }
    
    /// <summary>
    /// 构建编号
    /// </summary>
    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;
    
    /// <summary>
    /// 构建状态
    /// </summary>
    [JsonPropertyName("state")]
    public string StatusText { get; set; } = string.Empty;
    
    /// <summary>
    /// 构建参数
    /// </summary>
    [JsonPropertyName("properties")]
    public BuildProperties? Properties { get; set; }
    
    /// <summary>
    /// 构建标签
    /// </summary>
    [JsonPropertyName("tags")]
    public BuildTags? Tags { get; set; }
    
    /// <summary>
    /// 构建状态（枚举）
    /// </summary>
    [JsonIgnore]
    public BuildStatus Status
    {
        get
        {
            return StatusText?.ToUpper() switch
            {
                "SUCCESS" => BuildStatus.Success,
                "FAILURE" => BuildStatus.Failure,
                "RUNNING" => BuildStatus.Running,
                "QUEUED" => BuildStatus.Queued,
                "CANCELLED" => BuildStatus.Cancelled,
                _ => BuildStatus.Unknown
            };
        }
    }
    
    /// <summary>
    /// 构建类型 ID
    /// </summary>
    [JsonPropertyName("buildTypeId")]
    public string BuildTypeId { get; set; } = string.Empty;
    
    /// <summary>
    /// 分支名称
    /// </summary>
    [JsonPropertyName("branchName")]
    public string? BranchName { get; set; }
    
    /// <summary>
    /// 构建代理
    /// </summary>
    [JsonPropertyName("agent")]
    public BuildAgent? Agent { get; set; }
    
    /// <summary>
    /// 构建网页链接
    /// </summary>
    [JsonPropertyName("webUrl")]
    public string? WebUrl { get; set; }

    
    public bool HasParameterValue(string name, string value)
    {
        if (Properties == null)
        {
            return false;
        }
        
        return Properties.Property.Any(p => p.Name == name && p.Value == value);
    }

    public override string ToString()
    {
        return
            $"Build ID: {Id}, Number: {Number}, Status: {Status}, BuildTypeId: {BuildTypeId}, Branch: {BranchName}";
    }
}

