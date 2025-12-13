namespace EmailReceiver.WebApi.Infrastructure.TraceContext;

/// <summary>
/// 內容取得器介面
/// </summary>
public interface IContextGetter<out T> where T : class
{
    T? Get();
}
