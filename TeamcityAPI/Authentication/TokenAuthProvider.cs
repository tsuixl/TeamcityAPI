using System.Net.Http.Headers;

namespace TeamcityAPI.Authentication;

/// <summary>
/// Token 认证提供者
/// </summary>
public class TokenAuthProvider : IAuthenticationProvider
{
    private readonly string _token;

    public TokenAuthProvider(string token)
    {
        _token = token ?? throw new ArgumentNullException(nameof(token));
    }

    public Task ConfigureHttpClientAsync(HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        return Task.CompletedTask;
    }
}

