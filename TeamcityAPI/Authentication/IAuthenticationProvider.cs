namespace TeamcityAPI.Authentication;

/// <summary>
/// 认证提供者接口
/// </summary>
public interface IAuthenticationProvider
{
    /// <summary>
    /// 配置 HttpClient 的认证信息
    /// </summary>
    /// <param name="client">HttpClient 实例</param>
    Task ConfigureHttpClientAsync(HttpClient client);
}

