CREATE TABLE [MISC].[dbo].[mailReplay] (
    [mNo]            INT IDENTITY(1,1) NOT NULL PRIMARY KEY, -- 自動編號/主鍵
    [mailFrom]       NVARCHAR(200) NOT NULL CONSTRAINT [DF_mailReplay_mailFrom]       DEFAULT (''),   -- 寄件Email
    [mailFromName]   NVARCHAR(100) NOT NULL CONSTRAINT [DF_mailReplay_mailFromName]   DEFAULT (''),   -- 寄件者姓名
    [mailSubject]    NVARCHAR(200) NOT NULL CONSTRAINT [DF_mailReplay_mailSubject]    DEFAULT (''),   -- 信件標題
    [mailDate]       SMALLDATETIME NOT NULL CONSTRAINT [DF_mailReplay_mailDate]       DEFAULT (GETDATE()), -- 寄信日期
    [mailBody]       NTEXT NOT NULL CONSTRAINT [DF_mailReplay_mailBody]              DEFAULT (''),   -- 信件內容
    [mailType]       NVARCHAR(50) NOT NULL CONSTRAINT [DF_mailReplay_mailType]       DEFAULT (0),    -- 參考 allArray.asp
    [status]         TINYINT NOT NULL CONSTRAINT [DF_mailReplay_status]              DEFAULT (1),    -- 0.刪除 1.待處理 2.結案
    [tracker]        NVARCHAR(50) NOT NULL CONSTRAINT [DF_mailReplay_tracker]        DEFAULT (''),   -- 客服人員
    [dateIn]         SMALLDATETIME NOT NULL CONSTRAINT [DF_mailReplay_dateIn]        DEFAULT (GETDATE()), -- 建立日期
    [lNo]            INT NOT NULL CONSTRAINT [DF_mailReplay_lNo]                     DEFAULT (0),    -- letters 內的編號
    [reply]          NTEXT NOT NULL CONSTRAINT [DF_mailReplay_reply]                 DEFAULT (''),   -- 回覆內容(於 letters 內相同)
    [mailAttach]     VARCHAR(4000) NOT NULL CONSTRAINT [DF_mailReplay_mailAttach]    DEFAULT (''),   -- 附件名稱
    [mailAttachName] NVARCHAR(4000) NOT NULL CONSTRAINT [DF_mailReplay_mailAttachName] DEFAULT (''), -- 附件顯示名稱
    [mailAttachSize] VARCHAR(8000) NOT NULL CONSTRAINT [DF_mailReplay_mailAttachSize] DEFAULT (0)    -- 附件大小
    );
GO