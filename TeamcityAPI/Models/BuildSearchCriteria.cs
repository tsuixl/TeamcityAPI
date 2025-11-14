namespace TeamcityAPI.Models;

/// <summary>
/// 构建搜索条件
/// </summary>
public class BuildSearchCriteria
{
    /// <summary>
    /// 项目 ID
    /// </summary>
    public string? ProjectId { get; set; }
    
    /// <summary>
    /// 构建配置 ID
    /// </summary>
    public string? BuildTypeId { get; set; }
    
    /// <summary>
    /// 构建状态
    /// </summary>
    public BuildStatus? Status { get; set; }
    
    /// <summary>
    /// 分支名称
    /// </summary>
    public string? Branch { get; set; }
    
    /// <summary>
    /// 返回数量（默认 20）
    /// </summary>
    public int Count { get; set; } = 20;
    
    /// <summary>
    /// 起始日期
    /// </summary>
    public DateTime? SinceDate { get; set; }
    
    /// <summary>
    /// 标签
    /// </summary>
    public string? Tags { get; set; }
}

