using System.Net;

namespace TeamcityAPI.Exceptions;

/// <summary>
/// TeamCity API 异常
/// </summary>
public class TeamCityApiException : Exception
{
    /// <summary>
    /// HTTP 状态码
    /// </summary>
    public HttpStatusCode StatusCode { get; }
    
    /// <summary>
    /// 原始响应内容
    /// </summary>
    public string? ResponseContent { get; }

    public TeamCityApiException(string message) : base(message)
    {
        StatusCode = HttpStatusCode.InternalServerError;
    }

    public TeamCityApiException(string message, HttpStatusCode statusCode, string? responseContent = null) 
        : base(message)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }

    public TeamCityApiException(string message, Exception innerException) 
        : base(message, innerException)
    {
        StatusCode = HttpStatusCode.InternalServerError;
    }
}

