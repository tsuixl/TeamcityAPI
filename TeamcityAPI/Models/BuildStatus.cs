namespace TeamcityAPI.Models;

/// <summary>
/// 构建状态枚举
/// </summary>
public enum BuildStatus
{
    /// <summary>
    /// 未知状态
    /// </summary>
    Unknown,
    
    /// <summary>
    /// 排队中
    /// </summary>
    Queued,
    
    /// <summary>
    /// 运行中
    /// </summary>
    Running,
    
    /// <summary>
    /// 成功
    /// </summary>
    Success,
    
    /// <summary>
    /// 失败
    /// </summary>
    Failure,
    
    /// <summary>
    /// 已取消
    /// </summary>
    Cancelled
}

