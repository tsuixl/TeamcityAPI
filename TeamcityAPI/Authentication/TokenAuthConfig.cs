namespace TeamcityAPI.Authentication;

/// <summary>
/// Access Token 认证配置
/// </summary>
public class TokenAuthConfig : AuthConfig
{
    /// <summary>
    /// TeamCity Access Token
    /// </summary>
    public required string AccessToken { get; set; }
}

