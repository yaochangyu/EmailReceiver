namespace EmailReceiver.WebApi.EmailReceiver.Models;

/// <summary>
/// 儲存郵件至 letters 和 mailReplay 的資料模型
/// </summary>
public sealed class InsertEmailRequest
{
    /// <summary>寄件者姓名</summary>
    public string SenderName { get; init; } = string.Empty;
    
    /// <summary>寄件 Email</summary>
    public string SenderEmail { get; init; } = string.Empty;
    
    /// <summary>收信人 (預設為 1111)</summary>
    public string ToWhom { get; init; } = "1111";
    
    /// <summary>寄信日期</summary>
    public DateTime MailDate { get; init; }
    
    /// <summary>信件標題</summary>
    public string Subject { get; init; } = string.Empty;
    
    /// <summary>信件內容</summary>
    public string Body { get; init; } = string.Empty;
    
    /// <summary>來信問題類別 (預設: -使用敢言、感言-)</summary>
    public string Circumstance { get; init; } = "-使用敢言、感言-";
    
    /// <summary>客服人員</summary>
    public string Tracker { get; init; } = string.Empty;
    
    /// <summary>附件檔名</summary>
    public string? Attachment { get; init; }
    
    /// <summary>附件顯示名稱</summary>
    public string? AttachmentName { get; init; }
    
    /// <summary>附件大小</summary>
    public string? AttachmentSize { get; init; }

    public static InsertEmailRequest Create(
        string senderName,
        string senderEmail,
        string subject,
        string body,
        DateTime mailDate,
        string? toWhom = null,
        string? circumstance = null,
        string? tracker = null,
        string? attachment = null,
        string? attachmentName = null,
        string? attachmentSize = null)
    {
        return new InsertEmailRequest
        {
            SenderName = senderName,
            SenderEmail = senderEmail,
            Subject = subject,
            Body = body,
            MailDate = mailDate,
            ToWhom = toWhom ?? "1111",
            Circumstance = circumstance ?? "-使用敢言、感言-",
            Tracker = tracker ?? string.Empty,
            Attachment = attachment,
            AttachmentName = attachmentName,
            AttachmentSize = attachmentSize
        };
    }
}
