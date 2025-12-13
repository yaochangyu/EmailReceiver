using System.Text.Json;
using EmailReceiver.WebApi.Infrastructure.ErrorHandling;
using EmailReceiver.WebApi.Infrastructure.TraceContext;
using TraceContextType = EmailReceiver.WebApi.Infrastructure.TraceContext.TraceContext;

namespace EmailReceiver.WebApi.Infrastructure.Middleware;

/// <summary>
/// 例外處理中介軟體
/// 負責：
/// 1. 捕捉系統層級的未處理例外
/// 2. 記錄完整的錯誤資訊與請求參數
/// 3. 返回統一的 Failure 格式回應
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _env;
    private readonly JsonSerializerOptions _jsonOptions;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task InvokeAsync(
        HttpContext context,
        IContextGetter<TraceContextType> contextGetter)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, contextGetter);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        IContextGetter<TraceContextType> contextGetter)
    {
        var traceContext = contextGetter.Get();
        var traceId = traceContext?.TraceId;

        // 擷取請求資訊
        var requestInfo = await ExtractRequestInfoAsync(context);

        // 記錄錯誤日誌（包含完整請求資訊）
        _logger.LogError(exception, 
            "Unhandled exception - TraceId: {TraceId}, RequestInfo: {@RequestInfo}", 
            traceId, requestInfo);

        // 建立 Failure 物件
        var failure = new Failure
        {
            Code = nameof(FailureCode.InternalServerError),
            Message = _env.IsDevelopment() ? exception.Message : "內部伺服器錯誤",
            TraceId = traceId,
            Data = _env.IsDevelopment() 
                ? new { ExceptionType = exception.GetType().Name, Timestamp = DateTime.UtcNow }
                : null
        };

        // 設定回應
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 500;

        var response = new
        {
            failure.Code,
            failure.Message,
            failure.TraceId,
            failure.Data
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, _jsonOptions));
    }

    private async Task<object> ExtractRequestInfoAsync(HttpContext context)
    {
        var request = context.Request;
        
        // 擷取路由參數
        var routeValues = context.GetRouteData()?.Values;

        // 擷取查詢參數
        var queryParams = request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());

        // 擷取請求標頭（排除敏感資訊）
        var headers = request.Headers
            .Where(h => !IsSensitiveHeader(h.Key))
            .ToDictionary(h => h.Key, h => h.Value.ToString());

        // 擷取請求本文（僅限 POST/PUT/PATCH）
        string? body = null;
        if (request.Method is "POST" or "PUT" or "PATCH" && request.ContentLength > 0)
        {
            request.EnableBuffering();
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
        }

        return new
        {
            Method = request.Method,
            Path = request.Path.ToString(),
            QueryString = request.QueryString.ToString(),
            RouteValues = routeValues,
            QueryParams = queryParams,
            Headers = headers,
            Body = body,
            ContentType = request.ContentType,
            ContentLength = request.ContentLength
        };
    }

    private static readonly string[] SensitiveHeaders = 
    {
        "Authorization", "Cookie", "X-API-Key", "X-Auth-Token", 
        "Set-Cookie", "Proxy-Authorization"
    };

    private static bool IsSensitiveHeader(string headerName)
    {
        return SensitiveHeaders.Contains(headerName, StringComparer.OrdinalIgnoreCase);
    }
}
