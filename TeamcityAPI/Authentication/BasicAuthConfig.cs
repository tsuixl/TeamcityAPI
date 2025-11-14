namespace TeamcityAPI.Authentication;

/// <summary>
/// 基本认证（用户名/密码）配置
/// </summary>
public class BasicAuthConfig : AuthConfig
{
    /// <summary>
    /// 用户名
    /// </summary>
    public required string Username { get; set; }
    
    /// <summary>
    /// 密码
    /// </summary>
    public required string Password { get; set; }
}

