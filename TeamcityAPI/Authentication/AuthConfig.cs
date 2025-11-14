namespace TeamcityAPI.Authentication;

/// <summary>
/// 认证配置基类
/// </summary>
public abstract class AuthConfig
{
    /// <summary>
    /// TeamCity 服务器地址
    /// </summary>
    public required string ServerUrl { get; set; }
    
    /// <summary>
    /// 请求超时时间（默认 30 秒）
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}

