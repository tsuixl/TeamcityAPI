namespace TeamcityAPI.Models;

/// <summary>
/// 触发构建选项
/// </summary>
public class TriggerOptions
{
    /// <summary>
    /// 分支名称
    /// </summary>
    public string? BranchName { get; set; }
    
    /// <summary>
    /// 构建参数
    /// </summary>
    public Dictionary<string, string> Parameters { get; set; } = new();
    
    /// <summary>
    /// 触发注释
    /// </summary>
    public string? Comment { get; set; }
    
    /// <summary>
    /// 是否为个人构建
    /// </summary>
    public bool Personal { get; set; } = false;
}

