namespace EmailReceiver.WebApi.Infrastructure.Middleware;

/// <summary>
/// Middleware 擴充方法
/// </summary>
public static class MiddlewareExtensions
{
    /// <summary>
    /// 註冊所有自訂 Middleware（按正確順序）
    /// </summary>
    public static IApplicationBuilder UseCustomMiddleware(this IApplicationBuilder app)
    {
        // 1. 例外處理 - 最外層，捕捉所有未處理例外
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // 2. 追蹤內容 - 設定 TraceId 與 TraceContext
        app.UseMiddleware<TraceContextMiddleware>();

        // 3. 請求日誌 - 記錄請求與回應資訊
        app.UseMiddleware<RequestLoggingMiddleware>();

        return app;
    }
}
