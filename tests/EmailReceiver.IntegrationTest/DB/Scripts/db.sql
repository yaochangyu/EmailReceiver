-- 建立資料庫 MISC
CREATE DATABASE [MISC]
GO

-- 切換到 MISC 資料庫
USE [MISC]
GO

-- 建立 dbo schema (通常預設存在)
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'dbo')
    EXEC sp_executesql N'CREATE SCHEMA [dbo]'
GO

-- 建立 letters 資料表
CREATE TABLE [MISC].[dbo].[letters] (
    [lNo]           INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [rowguid]       UNIQUEIDENTIFIER NULL,
    [sender]        NVARCHAR(100) NULL,
    [s_email]       NVARCHAR(100) NULL,
    [telephone]     NVARCHAR(50) NULL,
    [towhom]        NVARCHAR(60) NULL,
    [s_date]        SMALLDATETIME NULL CONSTRAINT [DF_letters_s_date] DEFAULT (GETDATE()),
    [s_subject]     NVARCHAR(300) NULL,
    [s_question]    NTEXT NULL,
    [s_file1]       NVARCHAR(50) NULL,
    [s_file2]       NVARCHAR(50) NULL,
    [s_file3]       NVARCHAR(50) NULL,
    [s_file4]       NVARCHAR(50) NULL,
    [s_file5]       NVARCHAR(50) NULL,
    [handle]        NVARCHAR(200) NULL,
    [transactor]    NVARCHAR(15) NULL,
    [reply]         NTEXT NULL,
    [circumstance]  NVARCHAR(300) NULL,
    [datakind]      NVARCHAR(20) NULL,
    [track]         NVARCHAR(4) NULL,
    [ask_reply]     NVARCHAR(4) NULL,
    [sender_reply]  NVARCHAR(3000) NULL,
    [memo]          NVARCHAR(3000) NULL,
    [r_file1]       NVARCHAR(300) NULL,
    [r_file2]       NVARCHAR(300) NULL,
    [r_date]        DATETIME NULL,
    [stand]         INT NULL,
    [ok]            TINYINT NOT NULL CONSTRAINT [DF_letters_ok] DEFAULT ((0)),
    [ip]            NVARCHAR(50) NULL,
    [mno]           INT NULL,
    [tables]        NVARCHAR(20) NULL,
    [delimit]       NVARCHAR(50) NULL,
    [ServerName]    VARCHAR(100) NULL,
    [rowguid37]     UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_letters_rowguid37] DEFAULT (NEWID()),
    [browser]       VARCHAR(100) NULL,
    [agent]         VARCHAR(50) NULL,
    [reply1]        NTEXT NULL,
    [dateReply1]    DATETIME NULL,
    [assignto]      NVARCHAR(50) NULL,
    [assigndate]    DATETIME NULL,
    [idNumber]      VARCHAR(50) NULL,
    [s_birth]       NVARCHAR(20) NULL
    )
    GO

-- 建立 mailReplay 資料表
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