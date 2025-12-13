CREATE TABLE [MISC].[dbo].[letters] (
        [lNo]           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,   -- 自動編號/主鍵
        [rowguid]       UNIQUEIDENTIFIER NULL,
        [sender]        NVARCHAR(100) NULL,      -- 來信姓名
        [s_email]       NVARCHAR(100) NULL,      -- 來信者Email
        [telephone]     NVARCHAR(50) NULL,       -- 來信者手機
        [towhom]        NVARCHAR(60) NULL,       -- 1111(收信人)
        [s_date]        SMALLDATETIME NULL CONSTRAINT [DF_letters_s_date] DEFAULT (GETDATE()), -- 來信日期
        [s_subject]     NVARCHAR(300) NULL,      -- 來信主旨
        [s_question]    NTEXT NULL,              -- 來信內容
        [s_file1]       NVARCHAR(50) NULL,       -- 附檔
        [s_file2]       NVARCHAR(50) NULL,       -- 附檔
        [s_file3]       NVARCHAR(50) NULL,       -- 附檔
        [s_file4]       NVARCHAR(50) NULL,       -- 附檔
        [s_file5]       NVARCHAR(50) NULL,       -- 附檔
        [handle]        NVARCHAR(200) NULL,      -- 回信處理方式
        [transactor]    NVARCHAR(15) NULL,       -- 處理人員
        [reply]         NTEXT NULL,              -- 處理內容
        [circumstance]  NVARCHAR(300) NULL,      -- 來信問題類別
        [datakind]      NVARCHAR(20) NULL,
        [track]         NVARCHAR(4) NULL,
        [ask_reply]     NVARCHAR(4) NULL,
        [sender_reply]  NVARCHAR(3000) NULL,
        [memo]          NVARCHAR(3000) NULL,
        [r_file1]       NVARCHAR(300) NULL,
        [r_file2]       NVARCHAR(300) NULL,
        [r_date]        DATETIME NULL,           -- 回信日期
        [stand]         INT NULL,
        [ok]            TINYINT NOT NULL CONSTRAINT [DF_letters_ok] DEFAULT ((0)), -- 1_已處理,2_未處理,3_暫擱
        [ip]            NVARCHAR(50) NULL,       -- 來信IP
        [mno]           INT NULL,                -- 求職編號
        [tables]        NVARCHAR(20) NULL,
        [delimit]       NVARCHAR(50) NULL,
        [ServerName]    VARCHAR(100) NULL,       -- 執行哪部1111Server或來源
        [rowguid37]     UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_letters_rowguid37] DEFAULT (NEWID()),
        [browser]       VARCHAR(100) NULL,       -- 瀏覽器或裝置
        [agent]         VARCHAR(50) NULL,        -- 來源
        [reply1]        NTEXT NULL,
        [dateReply1]    DATETIME NULL,
        [assignto]      NVARCHAR(50) NULL,       -- 處理人員
        [assigndate]    DATETIME NULL,           -- 處理日期
        [idNumber]      VARCHAR(50) NULL,        -- 來信者身分證字號
        [s_birth]       NVARCHAR(20) NULL        -- 來信者生日
        );
        GO