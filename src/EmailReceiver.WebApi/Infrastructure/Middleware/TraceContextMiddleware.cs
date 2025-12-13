using EmailReceiver.WebApi.Infrastructure.TraceContext;
using TraceContextType = EmailReceiver.WebApi.Infrastructure.TraceContext.TraceContext;

namespace EmailReceiver.WebApi.Infrastructure.Middleware;

/// <summary>
/// 追蹤內容中介軟體
/// 負責：
/// 1. 從請求標頭擷取或產生 TraceId
/// 2. 設定 TraceContext 至 AsyncLocal
/// 3. 將 TraceId 加入回應標頭
/// </summary>
public sealed class TraceContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TraceContextMiddleware> _logger;
    private const string TraceIdHeaderName = "X-Trace-Id";

    public TraceContextMiddleware(
        RequestDelegate next,
        ILogger<TraceContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IContextSetter<TraceContextType> contextSetter)
    {
        // 從標頭取得 TraceId，若無則產生新的
        var traceId = context.Request.Headers[TraceIdHeaderName].FirstOrDefault()
                      ?? Guid.NewGuid().ToString("N");

        // TODO: 從驗證資訊取得 UserId (目前專案尚未實作身分驗證)
        string? userId = null;

        // 建立並設定 TraceContext
        var traceContext = new TraceContextType
        {
            TraceId = traceId,
            UserId = userId,
            RequestStartTime = DateTime.UtcNow
        };

        contextSetter.Set(traceContext);

        // 將 TraceId 加入回應標頭
        context.Response.Headers[TraceIdHeaderName] = traceId;

        _logger.LogInformation("Request started - TraceId: {TraceId}, Path: {Path}", 
            traceId, context.Request.Path);

        await _next(context);

        var elapsed = DateTime.UtcNow - traceContext.RequestStartTime;
        _logger.LogInformation("Request completed - TraceId: {TraceId}, Elapsed: {ElapsedMs}ms", 
            traceId, elapsed.TotalMilliseconds);
    }
}
