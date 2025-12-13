namespace EmailReceiver.WebApi.Infrastructure.TraceContext;

/// <summary>
/// 追蹤內容存取器 (使用 AsyncLocal 確保執行緒安全)
/// </summary>
public sealed class TraceContextAccessor : IContextGetter<TraceContext>, IContextSetter<TraceContext>
{
    private static readonly AsyncLocal<TraceContext?> _context = new();

    public TraceContext? Get() => _context.Value;

    public void Set(TraceContext context)
    {
        _context.Value = context;
    }
}
