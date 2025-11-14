using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TeamcityAPI.Authentication;
using TeamcityAPI.Exceptions;
using TeamcityAPI.Services;

namespace TeamcityAPI;

/// <summary>
/// TeamCity API 客户端
/// </summary>
public class TeamCityClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _serverUrl;
    private readonly IAuthenticationProvider _authProvider;
    private readonly ILogger<TeamCityClient> _logger;
    private bool _disposed;

    /// <summary>
    /// 构建服务
    /// </summary>
    public BuildService Builds { get; }
    
    /// <summary>
    /// 查询服务
    /// </summary>
    public QueryService Query { get; }
    
    /// <summary>
    /// 项目服务
    /// </summary>
    public ProjectService Projects { get; }

    /// <summary>
    /// JSON 序列化选项
    /// </summary>
    internal static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// 使用 Token 认证创建客户端
    /// </summary>
    public TeamCityClient(TokenAuthConfig config, ILogger<TeamCityClient>? logger = null)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));
        
        _logger = logger ?? NullLogger<TeamCityClient>.Instance;
        _serverUrl = config.ServerUrl.TrimEnd('/');
        _authProvider = new TokenAuthProvider(config.AccessToken);
        _httpClient = CreateHttpClient(config.Timeout);
        
        _logger.LogInformation("TeamCity 客户端已创建 (Token 认证) - 服务器: {ServerUrl}, 超时: {Timeout}秒", 
            _serverUrl, config.Timeout.TotalSeconds);
        
        // 初始化服务
        Builds = new BuildService(this, _logger);
        Query = new QueryService(this, _logger);
        Projects = new ProjectService(this, _logger);
    }

    /// <summary>
    /// 使用基本认证创建客户端
    /// </summary>
    public TeamCityClient(BasicAuthConfig? config, ILogger<TeamCityClient>? logger = null)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));
        
        _logger = logger ?? NullLogger<TeamCityClient>.Instance;
        _serverUrl = config.ServerUrl.TrimEnd('/');
        _authProvider = new BasicAuthProvider(config.Username, config.Password);
        _httpClient = CreateHttpClient(config.Timeout);
        
        _logger.LogInformation("TeamCity 客户端已创建 (基本认证) - 服务器: {ServerUrl}, 用户名: {Username}, 超时: {Timeout}秒", 
            _serverUrl, config.Username, config.Timeout.TotalSeconds);
        
        // 初始化服务
        Builds = new BuildService(this, _logger);
        Query = new QueryService(this, _logger);
        Projects = new ProjectService(this, _logger);
    }

    private HttpClient CreateHttpClient(TimeSpan timeout)
    {
        var client = new HttpClient
        {
            Timeout = timeout,
            BaseAddress = new Uri(_serverUrl)
        };
        
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        _authProvider.ConfigureHttpClientAsync(client).Wait();
        
        return client;
    }

    /// <summary>
    /// 测试连接
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            _logger.LogDebug("测试 TeamCity 服务器连接: {ServerUrl}", _serverUrl);
            var response = await GetAsync<object>("/app/rest/server");
            _logger.LogInformation("成功连接到 TeamCity 服务器: {ServerUrl}", _serverUrl);
            return response != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "连接 TeamCity 服务器失败: {ServerUrl}", _serverUrl);
            return false;
        }
    }

    /// <summary>
    /// 发送 GET 请求
    /// </summary>
    internal async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            _logger.LogDebug("发送 GET 请求: {Endpoint}", endpoint);
            var response = await _httpClient.GetAsync(endpoint);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("GET 请求失败: {Endpoint} - 状态码: {StatusCode}, 原因: {Reason}", 
                    endpoint, response.StatusCode, response.ReasonPhrase);
                throw new TeamCityApiException(
                    $"请求失败: {response.ReasonPhrase}",
                    response.StatusCode,
                    content
                );
            }

            _logger.LogDebug("GET 请求成功: {Endpoint} - 状态码: {StatusCode}", endpoint, response.StatusCode);
            return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        }
        catch (TeamCityApiException)
        {
            throw;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "GET 请求超时: {Endpoint}", endpoint);
            throw new TeamCityApiException("请求超时");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GET 请求异常: {Endpoint}", endpoint);
            throw new TeamCityApiException($"请求异常: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 发送 POST 请求
    /// </summary>
    internal async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        try
        {
            _logger.LogDebug("发送 POST 请求: {Endpoint}", endpoint);
            var response = await _httpClient.PostAsJsonAsync(endpoint, data, JsonOptions);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("POST 请求失败: {Endpoint} - 状态码: {StatusCode}, 原因: {Reason}", 
                    endpoint, response.StatusCode, response.ReasonPhrase);
                throw new TeamCityApiException(
                    $"请求失败: {response.ReasonPhrase}",
                    response.StatusCode,
                    content
                );
            }

            _logger.LogDebug("POST 请求成功: {Endpoint} - 状态码: {StatusCode}", endpoint, response.StatusCode);
            return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
        }
        catch (TeamCityApiException)
        {
            throw;
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "POST 请求超时: {Endpoint}", endpoint);
            throw new TeamCityApiException("请求超时");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "POST 请求异常: {Endpoint}", endpoint);
            throw new TeamCityApiException($"请求异常: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 发送 POST 请求（无返回值）
    /// </summary>
    internal async Task PostAsync<TRequest>(string endpoint, TRequest data)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, data, JsonOptions);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new TeamCityApiException(
                    $"请求失败: {response.ReasonPhrase}",
                    response.StatusCode,
                    content
                );
            }
        }
        catch (TeamCityApiException)
        {
            throw;
        }
        catch (TaskCanceledException)
        {
            throw new TeamCityApiException("请求超时");
        }
        catch (Exception ex)
        {
            throw new TeamCityApiException($"请求异常: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// 发送 DELETE 请求
    /// </summary>
    internal async Task DeleteAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(endpoint);
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                throw new TeamCityApiException(
                    $"请求失败: {response.ReasonPhrase}",
                    response.StatusCode,
                    content
                );
            }
        }
        catch (TeamCityApiException)
        {
            throw;
        }
        catch (TaskCanceledException)
        {
            throw new TeamCityApiException("请求超时");
        }
        catch (Exception ex)
        {
            throw new TeamCityApiException($"请求异常: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}

