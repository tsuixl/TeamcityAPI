using TeamcityAPI.Authentication;
using TeamcityAPI.CLI;
using Microsoft.Extensions.Logging;

namespace TeamcityAPI.Interface;

public class TeamcityRestApi
{
    private static TeamcityRestApi s_Instance = null!;

    public static TeamcityRestApi Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = new TeamcityRestApi();
            }
            return s_Instance;
        }
    }

    public TeamCityClient Client => m_Client;

    private TeamCityClient m_Client = null!;

    public void Init(GlobalOptions opts)
    {
        using var loggerFactory = CreateLoggerFactory(opts.LogLevel);
        var authConfig = CreateAuthConfig(opts);
        var logger = loggerFactory.CreateLogger<TeamCityClient>();
        
        m_Client = authConfig is TokenAuthConfig tokenConfig 
            ? new TeamCityClient(tokenConfig, logger) 
            : new TeamCityClient((BasicAuthConfig)authConfig!, logger);
    }
    
    public static AuthConfig? CreateAuthConfig(GlobalOptions opts)
    {
        // 验证认证参数
        if (string.IsNullOrEmpty(opts.Token) && 
            (string.IsNullOrEmpty(opts.Username) || string.IsNullOrEmpty(opts.Password)))
        {
            Console.WriteLine("错误: 必须提供 --token 或 --username 和 --password");
            Console.WriteLine("使用 --help 查看帮助");
            return null;
        }

        // 创建认证配置
        if (!string.IsNullOrEmpty(opts.Token))
        {
            return new TokenAuthConfig
            {
                ServerUrl = opts.Server,
                AccessToken = opts.Token,
                Timeout = TimeSpan.FromSeconds(opts.Timeout)
            };
        }

        return new BasicAuthConfig
        {
            ServerUrl = opts.Server,
            Username = opts.Username!,
            Password = opts.Password!,
            Timeout = TimeSpan.FromSeconds(opts.Timeout)
        };
    }
    
    public static ILoggerFactory CreateLoggerFactory(string logLevel)
    {
        // 解析日志级别
        if (!Enum.TryParse<LogLevel>(logLevel, true, out var level))
        {
            level = LogLevel.Information;
        }

        return LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(level)
                .AddConsole(options =>
                {
                    options.FormatterName = "simple";
                })
                .AddSimpleConsole(options =>
                {
                    options.IncludeScopes = false;
                    options.SingleLine = true;
                    options.TimestampFormat = "[HH:mm:ss] ";
                });
        });
    }
}