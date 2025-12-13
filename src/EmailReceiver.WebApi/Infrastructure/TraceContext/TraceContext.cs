namespace EmailReceiver.WebApi.Infrastructure.TraceContext;

/// <summary>
/// 追蹤內容 (不可變物件)
/// </summary>
public sealed record TraceContext
{
    /// <summary>追蹤識別碼</summary>
    public required string TraceId { get; init; }
    
    /// <summary>使用者識別碼</summary>
    public string? UserId { get; init; }
    
    /// <summary>請求開始時間</summary>
    public DateTime RequestStartTime { get; init; } = DateTime.UtcNow;
}
