namespace EmailReceiver.WebApi.EmailReceiver.Data.Entities;

/// <summary>
/// 郵件回覆管理表實體
/// </summary>
public sealed class MailReplay
{
    /// <summary>自動編號/主鍵</summary>
    public int MNo { get; init; }
    
    /// <summary>寄件 Email</summary>
    public string MailFrom { get; init; } = string.Empty;
    
    /// <summary>寄件者姓名</summary>
    public string MailFromName { get; init; } = string.Empty;
    
    /// <summary>信件標題</summary>
    public string MailSubject { get; init; } = string.Empty;
    
    /// <summary>寄信日期</summary>
    public DateTime MailDate { get; init; }
    
    /// <summary>信件內容</summary>
    public string MailBody { get; init; } = string.Empty;
    
    /// <summary>郵件類型 (參考 allArray.asp)</summary>
    public string MailType { get; init; } = "0";
    
    /// <summary>處理狀態 (0:刪除, 1:待處理, 2:結案)</summary>
    public byte Status { get; init; }
    
    /// <summary>客服人員</summary>
    public string Tracker { get; init; } = string.Empty;
    
    /// <summary>建立日期</summary>
    public DateTime DateIn { get; init; }
    
    /// <summary>letters 資料表內的編號</summary>
    public int LNo { get; init; }
    
    /// <summary>回覆內容 (於 letters 內相同)</summary>
    public string Reply { get; init; } = string.Empty;
    
    /// <summary>附件名稱</summary>
    public string MailAttach { get; init; } = string.Empty;
    
    /// <summary>附件顯示名稱</summary>
    public string MailAttachName { get; init; } = string.Empty;
    
    /// <summary>附件大小</summary>
    public string MailAttachSize { get; init; } = "0";

    private MailReplay()
    {
    }

    /// <summary>
    /// 建立新的郵件回覆記錄
    /// </summary>
    /// <param name="mailFrom">寄件 Email</param>
    /// <param name="mailFromName">寄件者姓名</param>
    /// <param name="mailSubject">信件標題</param>
    /// <param name="mailBody">信件內容</param>
    /// <param name="mailDate">寄信日期</param>
    /// <param name="lNo">對應 letters 表的編號</param>
    /// <returns>MailReplay 實體</returns>
    public static MailReplay Create(
        string mailFrom,
        string mailFromName,
        string mailSubject,
        string mailBody,
        DateTime mailDate,
        int lNo = 0)
    {
        return new MailReplay
        {
            MailFrom = mailFrom,
            MailFromName = mailFromName,
            MailSubject = mailSubject,
            MailBody = mailBody,
            MailDate = mailDate,
            Status = 1, // 1: 待處理
            DateIn = DateTime.Now,
            LNo = lNo
        };
    }
}
