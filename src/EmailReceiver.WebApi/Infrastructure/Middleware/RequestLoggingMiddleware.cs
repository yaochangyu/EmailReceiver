using EmailReceiver.WebApi.Infrastructure.TraceContext;
using TraceContextType = EmailReceiver.WebApi.Infrastructure.TraceContext.TraceContext;

namespace EmailReceiver.WebApi.Infrastructure.Middleware;

/// <summary>
/// 請求日誌記錄中介軟體
/// 負責：
/// 1. 當請求成功完成時記錄請求資訊
/// 2. 記錄回應狀態碼與處理時間
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IContextGetter<TraceContextType> contextGetter)
    {
        var startTime = DateTime.UtcNow;

        // 讓請求繼續處理
        await _next(context);

        // 僅在成功完成時記錄（例外情況已在 ExceptionHandlingMiddleware 記錄）
        var elapsed = DateTime.UtcNow - startTime;
        var traceContext = contextGetter.Get();

        var requestInfo = new
        {
            Method = context.Request.Method,
            Path = context.Request.Path.ToString(),
            QueryString = context.Request.QueryString.ToString(),
            StatusCode = context.Response.StatusCode,
            ElapsedMs = elapsed.TotalMilliseconds,
            TraceId = traceContext?.TraceId,
            UserId = traceContext?.UserId
        };

        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 400)
        {
            _logger.LogInformation("Request completed successfully - {@RequestInfo}", requestInfo);
        }
        else if (context.Response.StatusCode >= 400 && context.Response.StatusCode < 500)
        {
            _logger.LogWarning("Request completed with client error - {@RequestInfo}", requestInfo);
        }
        else if (context.Response.StatusCode >= 500)
        {
            _logger.LogError("Request completed with server error - {@RequestInfo}", requestInfo);
        }
    }
}
