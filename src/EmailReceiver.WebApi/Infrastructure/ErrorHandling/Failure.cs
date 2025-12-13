namespace EmailReceiver.WebApi.Infrastructure.ErrorHandling;

/// <summary>
/// 統一的錯誤回應物件
/// </summary>
public sealed record Failure
{
    /// <summary>錯誤代碼</summary>
    public required string Code { get; init; }
    
    /// <summary>錯誤訊息</summary>
    public required string Message { get; init; }
    
    /// <summary>追蹤識別碼</summary>
    public string? TraceId { get; init; }
    
    /// <summary>原始例外物件（不會序列化到客戶端）</summary>
    public Exception? Exception { get; init; }
    
    /// <summary>額外的結構化資料</summary>
    public object? Data { get; init; }

    /// <summary>
    /// 建立錯誤物件
    /// </summary>
    public static Failure Create(FailureCode code, string message, Exception? exception = null, object? data = null)
    {
        return new Failure
        {
            Code = nameof(code),
            Message = message,
            Exception = exception,
            Data = data
        };
    }

    /// <summary>
    /// 從例外建立錯誤物件
    /// </summary>
    public static Failure FromException(FailureCode code, Exception exception, string? customMessage = null)
    {
        return new Failure
        {
            Code = nameof(code),
            Message = customMessage ?? exception.Message,
            Exception = exception,
            Data = new { ExceptionType = exception.GetType().Name, Timestamp = DateTime.UtcNow }
        };
    }

    // 常用的錯誤建立方法
    public static Failure DbError(string message, Exception? exception = null) 
        => Create(FailureCode.DbError, message, exception);

    public static Failure Pop3Error(string message, Exception? exception = null) 
        => Create(FailureCode.Pop3ConnectionError, message, exception);

    public static Failure EmailReceiveError(string message, Exception? exception = null) 
        => Create(FailureCode.EmailReceiveError, message, exception);

    public static Failure ValidationError(string message, object? validationErrors = null) 
        => Create(FailureCode.ValidationError, message, data: validationErrors);

    public static Failure InternalServerError(Exception exception) 
        => FromException(FailureCode.InternalServerError, exception);
}
