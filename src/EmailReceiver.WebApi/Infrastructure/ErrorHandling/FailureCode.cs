namespace EmailReceiver.WebApi.Infrastructure.ErrorHandling;

/// <summary>
/// 錯誤代碼列舉
/// </summary>
public enum FailureCode
{
    /// <summary>未授權存取</summary>
    Unauthorized,
    
    /// <summary>資料庫錯誤</summary>
    DbError,
    
    /// <summary>重複郵件地址</summary>
    DuplicateEmail,
    
    /// <summary>資料庫併發衝突</summary>
    DbConcurrency,
    
    /// <summary>驗證錯誤</summary>
    ValidationError,
    
    /// <summary>無效操作</summary>
    InvalidOperation,
    
    /// <summary>逾時</summary>
    Timeout,
    
    /// <summary>內部伺服器錯誤</summary>
    InternalServerError,
    
    /// <summary>POP3 連線錯誤</summary>
    Pop3ConnectionError,
    
    /// <summary>郵件接收錯誤</summary>
    EmailReceiveError,
    
    /// <summary>未知錯誤</summary>
    Unknown
}
