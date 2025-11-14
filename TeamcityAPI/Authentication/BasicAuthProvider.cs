using System.Net.Http.Headers;
using System.Text;

namespace TeamcityAPI.Authentication;

/// <summary>
/// 基本认证提供者
/// </summary>
public class BasicAuthProvider : IAuthenticationProvider
{
    private readonly string _username;
    private readonly string _password;

    public BasicAuthProvider(string username, string password)
    {
        _username = username ?? throw new ArgumentNullException(nameof(username));
        _password = password ?? throw new ArgumentNullException(nameof(password));
    }

    public Task ConfigureHttpClientAsync(HttpClient client)
    {
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        return Task.CompletedTask;
    }
}

