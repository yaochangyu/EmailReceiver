namespace EmailReceiver.WebApi.Infrastructure.TraceContext;

/// <summary>
/// 內容設定器介面
/// </summary>
public interface IContextSetter<in T> where T : class
{
    void Set(T context);
}
