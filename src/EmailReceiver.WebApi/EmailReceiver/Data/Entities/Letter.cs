namespace EmailReceiver.WebApi.EmailReceiver.Data.Entities;

/// <summary>
/// 來信管理主表實體
/// </summary>
public sealed class Letter
{
    /// <summary>自動編號/主鍵</summary>
    public int LNo { get; init; }
    
    /// <summary>資料列 GUID</summary>
    public Guid? Rowguid { get; init; }
    
    /// <summary>來信姓名</summary>
    public string? Sender { get; init; }
    
    /// <summary>來信者 Email</summary>
    public string? SEmail { get; init; }
    
    /// <summary>來信者手機</summary>
    public string? Telephone { get; init; }
    
    /// <summary>收信人</summary>
    public string? Towhom { get; init; }
    
    /// <summary>來信日期</summary>
    public DateTime? SDate { get; init; }
    
    /// <summary>來信主旨</summary>
    public string? SSubject { get; init; }
    
    /// <summary>來信內容</summary>
    public string? SQuestion { get; init; }
    
    /// <summary>附檔 1</summary>
    public string? SFile1 { get; init; }
    
    /// <summary>附檔 2</summary>
    public string? SFile2 { get; init; }
    
    /// <summary>附檔 3</summary>
    public string? SFile3 { get; init; }
    
    /// <summary>附檔 4</summary>
    public string? SFile4 { get; init; }
    
    /// <summary>附檔 5</summary>
    public string? SFile5 { get; init; }
    
    /// <summary>回信處理方式</summary>
    public string? Handle { get; init; }
    
    /// <summary>處理人員</summary>
    public string? Transactor { get; init; }
    
    /// <summary>處理內容</summary>
    public string? Reply { get; init; }
    
    /// <summary>來信問題類別</summary>
    public string? Circumstance { get; init; }
    
    /// <summary>資料類型</summary>
    public string? Datakind { get; init; }
    
    /// <summary>追蹤狀態</summary>
    public string? Track { get; init; }
    
    /// <summary>是否需要回信</summary>
    public string? AskReply { get; init; }
    
    /// <summary>寄件者回應</summary>
    public string? SenderReply { get; init; }
    
    /// <summary>備註</summary>
    public string? Memo { get; init; }
    
    /// <summary>回信附檔 1</summary>
    public string? RFile1 { get; init; }
    
    /// <summary>回信附檔 2</summary>
    public string? RFile2 { get; init; }
    
    /// <summary>回信日期</summary>
    public DateTime? RDate { get; init; }
    
    /// <summary>狀態標記</summary>
    public int? Stand { get; init; }
    
    /// <summary>處理狀態 (1:已處理, 2:未處理, 3:暫擱)</summary>
    public byte Ok { get; init; }
    
    /// <summary>來信 IP</summary>
    public string? Ip { get; init; }
    
    /// <summary>求職編號</summary>
    public int? Mno { get; init; }
    
    /// <summary>資料表名稱</summary>
    public string? Tables { get; init; }
    
    /// <summary>分隔符號</summary>
    public string? Delimit { get; init; }
    
    /// <summary>執行哪部 Server 或來源</summary>
    public string? ServerName { get; init; }
    
    /// <summary>資料列 GUID (第二組)</summary>
    public Guid Rowguid37 { get; init; }
    
    /// <summary>瀏覽器或裝置</summary>
    public string? Browser { get; init; }
    
    /// <summary>來源</summary>
    public string? Agent { get; init; }
    
    /// <summary>第二次回覆內容</summary>
    public string? Reply1 { get; init; }
    
    /// <summary>第二次回覆日期</summary>
    public DateTime? DateReply1 { get; init; }
    
    /// <summary>指派處理人員</summary>
    public string? Assignto { get; init; }
    
    /// <summary>指派日期</summary>
    public DateTime? Assigndate { get; init; }
    
    /// <summary>來信者身分證字號</summary>
    public string? IdNumber { get; init; }
    
    /// <summary>來信者生日</summary>
    public string? SBirth { get; init; }

    private Letter()
    {
    }

    /// <summary>
    /// 建立新的來信記錄
    /// </summary>
    /// <param name="sender">來信姓名</param>
    /// <param name="sEmail">來信者 Email</param>
    /// <param name="sSubject">來信主旨</param>
    /// <param name="sQuestion">來信內容</param>
    /// <param name="sDate">來信日期 (預設為目前時間)</param>
    /// <param name="towhom">收信人 (預設為 1111)</param>
    /// <param name="circumstance">來信問題類別 (預設: -使用敢言、感言-)</param>
    /// <param name="ip">來信 IP</param>
    /// <returns>Letter 實體</returns>
    public static Letter Create(
        string? sender,
        string? sEmail,
        string? sSubject,
        string? sQuestion,
        DateTime? sDate = null,
        string? towhom = null,
        string? circumstance = null,
        string? ip = null)
    {
        return new Letter
        {
            Sender = sender,
            SEmail = sEmail,
            SSubject = sSubject,
            SQuestion = sQuestion,
            SDate = sDate ?? DateTime.Now,
            Towhom = towhom ?? "1111",
            Circumstance = circumstance ?? "-使用敢言、感言-",
            Ok = 2, // 2: 未處理
            Rowguid37 = Guid.NewGuid(),
            Ip = ip
        };
    }
}
