# GitHub Copilot 專案指引 - EmailReceiver

## 重要編碼原則

### 參考模板專案
- **編碼原則**：參考 https://github.com/yaochangyu/api.template 的 CLAUDE.md
- **實作方式**：從 https://github.com/yaochangyu/api.template 複製程式碼改寫，修改成符合本專案需求的命名空間
- **文件規範**：需包含以下 mermaid 圖表
  - 流程圖（Flowchart）
  - 有限狀態機（State Diagram）
  - 循序圖（Sequence Diagram）

## 專案概述

這是一個使用 **C# .NET 9.0** 開發的 **POP3 郵件接收 Web API**，採用 **Clean Architecture** 架構模式，使用 **MailKit** 處理郵件，並透過 **Entity Framework Core** 將郵件儲存至 **SQL Server** 資料庫。

### 核心功能
- 透過 POP3 協定接收郵件
- 雙表架構儲存郵件（letters 來信主表 + mailReplay 回覆管理表）
- 支援附件管理（資料表已包含附件欄位）
- 客服處理流程（處理狀態、客服指派、回信管理）
- 使用 Result Pattern 搭配 Failure 物件進行統一錯誤處理
- Middleware 管線提供例外處理、追蹤、日誌功能
- 提供 RESTful API 端點（含版本控制）
- Swagger API 文件
- 完整的 BDD 整合測試

### 專案演進說明

本專案遵循 Clean Architecture 原則，並參考 https://github.com/yaochangyu/api.template 的架構模式進行設計。主要演進歷程：

1. **資料庫架構演進**: 從簡化的單表設計改為雙表架構（letters + mailReplay），以支援完整的客服處理流程
2. **錯誤處理強化**: 引入 Infrastructure/ErrorHandling 層，提供統一的 Failure 物件和錯誤代碼對應
3. **追蹤與日誌**: 新增 TraceContext 和 Middleware 管線，提供請求追蹤和結構化日誌
4. **環境管理**: 實作環境變數管理系統，支援多環境部署
5. **測試框架**: 建立 BDD 整合測試專案，使用 Testcontainers 提供隔離的測試環境

## 技術堆疊

### 框架與函式庫
- **.NET 9.0** - 主要開發框架
- **MailKit 4.14.1** - POP3 郵件處理
- **Entity Framework Core 9.0** - ORM 資料存取
- **SQL Server** - 資料庫
- **CSharpFunctionalExtensions 3.1.0** - Result Pattern 實作
- **Swashbuckle.AspNetCore 10.0.1** - Swagger/OpenAPI 文件
- **Serilog.AspNetCore 8.0.3** - 結構化日誌記錄
- **SpecFlow** (測試專案) - BDD 測試框架
- **Testcontainers** (測試專案) - Docker 測試容器

### 開發原則
- **Clean Architecture** - 分層架構設計
- **Result Pattern** - 使用 `Result<TValue, TError>` 搭配 `Failure` 物件進行錯誤處理
- **Repository Pattern** - 封裝資料存取邏輯
- **依賴注入** - 使用 .NET 內建 DI 容器
- **不可變物件設計** - Entity 使用 `init` 關鍵字
- **Middleware 管線** - 統一處理例外、追蹤、日誌
- **環境變數管理** - 集中管理設定，支援多環境部署
- **BDD 測試** - 使用 Gherkin 語法描述業務場景

## 專案結構

```
EmailReceiver/
├── src/
│   └── EmailReceiver.WebApi/
│       ├── EmailReceiver/          # 郵件接收領域
│       │   ├── Controllers/        # API 控制器層
│       │   │   └── EmailsController.cs
│       │   ├── ReceiveEmailHandler.cs  # 業務邏輯處理層
│       │   ├── Adpaters/          # 適配器層（POP3 收信）
│       │   │   ├── IEmailReceiveAdapter.cs
│       │   │   └── Pop3EmailReceiveAdapter.cs
│       │   ├── Repositories/      # 資料存取層
│       │   │   ├── IReceiveEmailRepository.cs
│       │   │   └── ReceiveEmailRepository.cs
│       │   ├── Data/              # 資料層
│       │   │   ├── EmailReceiverDbContext.cs  # DbContext
│       │   │   └── Entities/      # 資料實體（不可變物件）
│       │   │       ├── Letter.cs         # 來信管理主表
│       │   │       └── MailReplay.cs     # 郵件回覆管理表
│       │   ├── Models/            # DTO 和 Request/Response Models
│       │   │   ├── InsertEmailRequest.cs
│       │   │   └── Responses/
│       │   │       ├── EmailMessageResponse.cs
│       │   │       └── ReceiveEmailsResponse.cs
│       │   └── Options/           # 設定選項類別
│       │       └── Pop3Options.cs
│       ├── Infrastructure/        # 基礎設施層
│       │   ├── ErrorHandling/     # 錯誤處理
│       │   │   ├── Failure.cs     # 統一錯誤物件
│       │   │   ├── FailureCode.cs # 錯誤代碼列舉
│       │   │   └── FailureCodeMapper.cs  # HTTP 狀態碼對應
│       │   ├── TraceContext/      # 追蹤上下文
│       │   │   ├── TraceContext.cs
│       │   │   ├── TraceContextAccessor.cs
│       │   │   ├── IContextGetter.cs
│       │   │   └── IContextSetter.cs
│       │   ├── Middleware/        # 中介軟體
│       │   │   ├── ExceptionHandlingMiddleware.cs
│       │   │   ├── TraceContextMiddleware.cs
│       │   │   ├── RequestLoggingMiddleware.cs
│       │   │   └── MiddlewareExtensions.cs
│       │   ├── EnvironmentVariableBase.cs  # 環境變數基底類別
│       │   ├── EnvironmentVariables.cs     # 環境變數定義
│       │   └── EnvironmentUtility.cs       # 環境變數工具
│       ├── ServiceCollectionExtension.cs   # DI 擴充方法
│       └── Program.cs             # 應用程式進入點
├── tests/
│   ├── EmailReceiver.IntegrationTest/  # BDD 整合測試
│   │   ├── Email/
│   │   │   ├── 收信.feature      # Gherkin 測試場景
│   │   │   └── 收信_Step.cs      # 測試步驟實作
│   │   ├── TestServer.cs          # 測試伺服器
│   │   ├── TestAssistant.cs       # 測試輔助工具
│   │   └── FakeEmailReceiveAdapter.cs  # 測試用假物件
│   └── EmailReceiver.Testing.Common/   # 測試共用專案
│       ├── MockServer/            # Mock 伺服器工具
│       ├── TestContainerFactory.cs  # 測試容器工廠
│       └── (其他測試輔助類別)
├── db/                  # 資料庫定義檔
│   ├── letters.sql
│   └── mailReplay.sql
├── docs/                # 文件資料夾
└── env/                 # 環境設定檔（本地開發）
    └── local.env
```

## 架構流程

### 請求流程（Clean Architecture + Middleware）
```
API Request
    ↓
ExceptionHandlingMiddleware (最外層，捕捉所有未處理例外)
    ↓
TraceContextMiddleware (設定 TraceId 與 TraceContext)
    ↓
RequestLoggingMiddleware (記錄請求與回應資訊)
    ↓
Controller (EmailsController)
    ↓
Handler (ReceiveEmailHandler)
    ↓
Adapter (Pop3EmailReceiveAdapter) ← 從 POP3 取得郵件
    ↓
Repository (ReceiveEmailRepository) ← 儲存至 letters 和 mailReplay 表
    ↓
Database (SQL Server - 雙表架構)
```

### 關鍵類別說明

#### 1. Controller 層
- **EmailsController**: 提供一個端點
  - `POST /api/v1/emails/receive` - 接收郵件並儲存至資料庫
  - 注入 `ReceiveEmailHandler` 和 `IContextGetter<TraceContext>`
  - 使用 TraceContext 提供錯誤追蹤功能
  - 失敗時透過 `Failure.ToActionResult()` 返回適當的 HTTP 狀態碼

#### 2. Handler 層
- **ReceiveEmailHandler**: 協調郵件接收流程
  1. 呼叫 Adapter 從 POP3 取得郵件
  2. 將郵件資料轉換為 InsertEmailRequest
  3. 透過 Repository 儲存至資料庫（letters 和 mailReplay 表）
  4. 處理儲存失敗的情況
  5. 返回成功儲存的郵件數量

#### 3. Adapter 層
- **Pop3EmailReceiveAdapter**: 實作 POP3 郵件接收
  - 連線至 POP3 伺服器
  - 取得郵件訊息（主旨、內容、寄件者等）
  - 返回 `Result<IReadOnlyList<EmailMessageResponse>, Failure>`

#### 4. Repository 層
- **ReceiveEmailRepository**: 封裝資料存取
  - `AddAsync` - 新增郵件至 letters 和 mailReplay 表（使用交易）
    - 回傳類型: `Result<int, Failure>`（成功時回傳 lNo）
  - `GetAllUidlsAsync` - 取得所有 UIDL（從 mailReplay.MailAttachName）
    - 回傳類型: `Result<IReadOnlyList<string>, Failure>`

#### 5. Infrastructure 層

##### ErrorHandling（錯誤處理）
- **Failure**: 統一的錯誤回應物件
  - 屬性: Code, Message, TraceId, Exception, Data
  - 提供靜態工廠方法: `DbError()`, `Pop3Error()`, `EmailReceiveError()` 等
  - 支援 `ToActionResult()` 轉換為 HTTP 回應

- **FailureCode**: 錯誤代碼列舉
  - 定義所有可能的錯誤類型

- **FailureCodeMapper**: 將 FailureCode 對應到 HTTP 狀態碼

##### TraceContext（追蹤上下文）
- **TraceContext**: 追蹤上下文資料物件（包含 TraceId）
- **TraceContextAccessor**: 存取當前請求的 TraceContext
- **IContextGetter<T>** / **IContextSetter<T>**: 泛型上下文存取介面

##### Middleware（中介軟體）
- **ExceptionHandlingMiddleware**: 全域例外處理
  - 捕捉所有未處理的例外
  - 轉換為統一的錯誤回應格式

- **TraceContextMiddleware**: 追蹤上下文設定
  - 為每個請求產生 TraceId
  - 設定 TraceContext 供後續使用

- **RequestLoggingMiddleware**: 請求日誌記錄
  - 使用 Serilog 記錄請求與回應資訊
  - 包含執行時間、狀態碼等

##### 環境變數管理
- **EnvironmentVariableBase**: 環境變數基底類別
- **EnvironmentVariables**: 具體環境變數定義（如 `SYS_DATABASE_CONNECTION_STRING`）
- **EnvironmentUtility**: 環境變數讀取工具
  - 支援從 `.env` 檔案讀取
  - 支援 `--local` 參數載入本地設定

#### 6. Entity 層
- **Letter**: 來信管理主表實體（不可變物件）
  - 使用 `init` 屬性設定
  - 提供靜態工廠方法 `Create()`
  - 主要欄位: LNo, Sender, SEmail, SSubject, SQuestion, SDate, Ok（處理狀態）
  - 支援附件、回信處理、客服指派等進階功能

- **MailReplay**: 郵件回覆管理表實體（不可變物件）
  - 使用 `init` 屬性設定
  - 提供靜態工廠方法 `Create()`
  - 主要欄位: MNo, MailFrom, MailFromName, MailSubject, MailBody, MailDate, Status, LNo（關聯至 letters 表）
  - 支援附件管理、處理狀態追蹤

## 資料庫設計

### 目前系統架構（雙表設計）

本專案採用雙表設計，分離來信與回覆管理，以支援完整的客服處理流程。

#### 1. letters 資料表（來信管理主表）

**主要欄位：**

| 欄位 | 型別 | 說明 | 預設值 |
|------|------|------|--------|
| lNo | int | 主鍵（自動編號） | - |
| rowguid | uniqueidentifier | 資料列 GUID | - |
| sender | nvarchar(100) | 來信姓名 | - |
| s_email | nvarchar(100) | 來信者 Email | - |
| telephone | nvarchar(50) | 來信者手機 | - |
| towhom | nvarchar(60) | 收信人 | - |
| s_date | smalldatetime | 來信日期 | GETDATE() |
| s_subject | nvarchar(300) | 來信主旨 | - |
| s_question | ntext | 來信內容 | - |
| s_file1 ~ s_file5 | nvarchar(50) | 附件檔名（支援 5 個附件） | - |
| handle | nvarchar(200) | 回信處理方式 | - |
| transactor | nvarchar(15) | 處理人員 | - |
| reply | ntext | 處理內容 | - |
| circumstance | nvarchar(300) | 來信問題類別 | - |
| ok | tinyint | 處理狀態（1:已處理, 2:未處理, 3:暫擱） | 0 |
| rowguid37 | uniqueidentifier | 第二組 GUID | NEWID() |

**設計特色：**
- 完整的客服處理流程欄位
- 支援多檔案附件
- 包含追蹤狀態與指派機制
- 使用舊版資料型別（smalldatetime, ntext）以相容舊系統

#### 2. mailReplay 資料表（郵件回覆管理表）

**主要欄位：**

| 欄位 | 型別 | 說明 | 預設值 |
|------|------|------|--------|
| mNo | int | 主鍵（自動編號） | - |
| mailFrom | nvarchar(200) | 寄件 Email | "" |
| mailFromName | nvarchar(100) | 寄件者姓名 | "" |
| mailSubject | nvarchar(200) | 信件標題 | "" |
| mailDate | smalldatetime | 寄信日期 | GETDATE() |
| mailBody | ntext | 信件內容 | "" |
| mailType | nvarchar(50) | 郵件類型 | "0" |
| status | tinyint | 處理狀態（0:刪除, 1:待處理, 2:結案） | 1 |
| tracker | nvarchar(50) | 客服人員 | "" |
| dateIn | smalldatetime | 建立日期 | GETDATE() |
| lNo | int | 關聯至 letters 表的編號 | 0 |
| reply | ntext | 回覆內容 | "" |
| mailAttach | varchar(4000) | 附件名稱 | "" |
| mailAttachName | nvarchar(4000) | 附件顯示名稱 | "" |
| mailAttachSize | varchar(8000) | 附件大小 | "0" |

**設計特色：**
- 透過 `lNo` 關聯至 letters 表
- 支援附件管理
- 包含處理狀態追蹤
- 所有欄位皆有預設值，確保資料完整性

#### 3. 資料表關聯
```
letters (1) ←──→ (N) mailReplay
  └─ lNo           └─ lNo (外鍵)
```

**儲存流程（使用交易）：**
1. 先新增 Letter 記錄至 letters 表，取得 lNo
2. 使用 lNo 建立 MailReplay 記錄至 mailReplay 表
3. 兩個操作在同一交易中執行，確保資料一致性

### 資料寫入流程詳細說明

#### 1. letters 表寫入

**對應的 SQL 語法：**
```sql
Insert Into letters(sender, s_email, towhom, s_date, s_subject, s_question, circumstance, ok)
Values(N'寄件者姓名', N'寄件Email', 1111, '寄信日期', N'信件標題', N'信件內容', N'-使用敢言、感言-', 2);
```

**程式碼實作（ReceiveEmailRepository.cs:43-50）：**
```csharp
var letter = Letter.Create(
    sender: request.SenderName,        // → sender
    sEmail: request.SenderEmail,       // → s_email
    sSubject: request.Subject,         // → s_subject
    sQuestion: request.Body,           // → s_question
    sDate: request.MailDate,           // → s_date
    towhom: request.ToWhom,            // → towhom (預設: "1111")
    circumstance: request.Circumstance); // → circumstance (預設: "-使用敢言、感言-")
    // Ok = 2 自動設定於 Letter.Create() 內部
```

#### 2. mailReplay 表寫入

**對應的 SQL 語法：**
```sql
Insert Into mailReplay(mailFrom, mailFromName, mailSubject, mailDate, mailBody, mailType, tracker, lNo, mailAttach, mailAttachName, mailAttachSize)
Values(N'寄件Email', N'寄件者姓名', N'信件標題', '寄信日期', N'信件內容', N'-使用敢言、感言-', N'客服人員', letters.lNo, '附件', N'附件名稱', '附件大小');
```

**程式碼實作（ReceiveEmailRepository.cs:55-66）：**
```csharp
var mailReplay = MailReplay.Create(
    mailFrom: request.SenderEmail,              // → mailFrom
    mailFromName: request.SenderName,           // → mailFromName
    mailSubject: request.Subject,               // → mailSubject
    mailBody: request.Body,                     // → mailBody
    mailDate: request.MailDate,                 // → mailDate
    lNo: letter.LNo,                            // → lNo (從 letters 表取得)
    mailType: request.Circumstance,             // → mailType (如: "-使用敢言、感言-")
    tracker: request.Tracker,                   // → tracker (客服人員)
    mailAttach: request.Attachment ?? "",       // → mailAttach (附件)
    mailAttachName: request.AttachmentName ?? "", // → mailAttachName (附件名稱)
    mailAttachSize: request.AttachmentSize ?? "0"); // → mailAttachSize (附件大小)
    // Status = 1 自動設定於 MailReplay.Create() 內部
```

#### 3. 完整交易流程

```csharp
using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
try
{
    // 步驟 1: 新增 Letter 並取得 LNo
    await _context.Letters.AddAsync(letter, cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);

    // 步驟 2: 使用 LNo 新增 MailReplay
    await _context.MailReplays.AddAsync(mailReplay, cancellationToken);
    await _context.SaveChangesAsync(cancellationToken);

    // 步驟 3: 提交交易
    await transaction.CommitAsync(cancellationToken);
}
catch (Exception ex)
{
    // 發生錯誤時回滾交易
    await transaction.RollbackAsync(cancellationToken);
    return Result.Failure<int, Failure>(Failure.DbError("儲存郵件失敗", ex));
}
```

### 設計說明

#### 為何採用雙表架構？

本專案從初始的簡化單表設計 (EmailMessages) 改為雙表架構 (letters + mailReplay)，主要原因：

1. **相容舊系統**：需要整合既有的客服郵件管理系統
2. **支援完整流程**：包含附件處理、客服指派、處理狀態追蹤等進階功能
3. **資料分離**：來信與回覆分開管理，便於查詢與維護
4. **可擴充性**：為未來的客服管理功能預留空間

#### Git 歷史記錄

專案重構歷程：
```
🗑️ 刪除 EmailMessage 實體及其相關資料庫邏輯
✨ 儲存郵件至 letters 和 mailReplay 資料表，重構為使用實體建立方式
```

#### 資料庫定義檔案

專案根目錄的 `db/` 資料夾包含完整的 SQL 資料表定義檔：
- `db/letters.sql` - 來信管理主表定義
- `db/mailReplay.sql` - 郵件回覆管理表定義

這些檔案可用於建立資料庫結構或理解完整的欄位定義。

## API 端點

### 接收郵件
```http
POST /api/v1/emails/receive
```

**成功回應範例 (200 OK):**
```json
{
  "savedCount": 5,
  "message": "成功接收並儲存 5 封郵件"
}
```

**錯誤回應範例 (依錯誤類型回傳不同狀態碼):**
```json
{
  "code": "Pop3ConnectionError",
  "message": "無法連線至 POP3 伺服器",
  "traceId": "00-abc123-def456-01"
}
```

**可能的 HTTP 狀態碼:**
- `200 OK` - 成功接收並儲存郵件
- `400 Bad Request` - 驗證錯誤
- `500 Internal Server Error` - 伺服器內部錯誤
- `502 Bad Gateway` - POP3 連線錯誤
- `503 Service Unavailable` - 資料庫錯誤

## 設定檔

### 環境變數設定

本專案使用環境變數進行設定，而非傳統的 `appsettings.json`。

#### 本地開發設定 (env/local.env)
```bash
# 資料庫連線字串
SYS_DATABASE_CONNECTION_STRING=Server=localhost;Database=EmailReceiverDb;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;
```

#### 啟動參數
```bash
# 使用本地環境變數檔案
dotnet run --local
```

當使用 `--local` 參數時，程式會從 `env/local.env` 讀取環境變數。

### appsettings.json

#### POP3 設定
```json
{
  "Pop3": {
    "Host": "pop.gmail.com",
    "Port": 110,
    "UseSsl": true,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

**注意:** POP3 設定仍使用 `appsettings.json`，透過 `Pop3Options` 類別注入。

## 開發指引

### 程式碼風格

#### 1. 命名慣例
- **類別/介面**: PascalCase (例如: `Letter`, `MailReplay`, `IEmailReceiveAdapter`)
- **方法**: PascalCase (例如: `HandleAsync`, `AddAsync`)
- **參數/區域變數**: camelCase (例如: `cancellationToken`, `request`)
- **私有欄位**: _camelCase (例如: `_repository`, `_context`)

#### 2. 非同步方法
- 所有 I/O 操作都應使用非同步方法
- 方法名稱以 `Async` 結尾
- 接受 `CancellationToken` 參數

#### 3. Result Pattern 使用

本專案使用 `CSharpFunctionalExtensions` 的 `Result<TValue, TError>` 搭配自訂的 `Failure` 物件。

```csharp
// 成功情況
return Result.Success<int, Failure>(value);

// 失敗情況 - 使用 Failure 靜態方法
return Result.Failure<int, Failure>(Failure.DbError("資料庫錯誤訊息", ex));
return Result.Failure<int, Failure>(Failure.Pop3Error("POP3 連線失敗", ex));

// 檢查結果
if (result.IsFailure)
{
    // result.Error 是 Failure 物件
    return Result.Failure<T, Failure>(result.Error);
}

// Controller 中轉換為 ActionResult
if (result.IsFailure)
{
    var traceContext = _contextGetter.Get();
    var failure = result.Error with { TraceId = traceContext?.TraceId };
    return failure.ToActionResult();  // 自動對應到適當的 HTTP 狀態碼
}
```

**Failure 靜態工廠方法:**
- `Failure.DbError(message, exception)` - 資料庫錯誤
- `Failure.Pop3Error(message, exception)` - POP3 錯誤
- `Failure.EmailReceiveError(message, exception)` - 郵件接收錯誤
- `Failure.ValidationError(message, validationErrors)` - 驗證錯誤
- `Failure.InternalServerError(exception)` - 伺服器內部錯誤

#### 4. 不可變物件設計

**Letter 實體：**
```csharp
public sealed class Letter
{
    public int LNo { get; init; }
    public string? Sender { get; init; }
    public string? SEmail { get; init; }
    public string? Towhom { get; init; }
    public string? Circumstance { get; init; }
    public byte Ok { get; init; }
    // ... 其他屬性

    private Letter() { }

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
```

**MailReplay 實體：**
```csharp
public sealed class MailReplay
{
    public int MNo { get; init; }
    public string MailFrom { get; init; } = string.Empty;
    public string MailFromName { get; init; } = string.Empty;
    public string MailType { get; init; } = "0";
    public int LNo { get; init; }
    // ... 其他屬性

    private MailReplay() { }

    public static MailReplay Create(
        string mailFrom,
        string mailFromName,
        string mailSubject,
        string mailBody,
        DateTime mailDate,
        int lNo = 0,
        string mailType = "0",
        string tracker = "",
        string mailAttach = "",
        string mailAttachName = "",
        string mailAttachSize = "0")
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
            LNo = lNo,
            MailType = mailType,
            Tracker = tracker,
            MailAttach = mailAttach,
            MailAttachName = mailAttachName,
            MailAttachSize = mailAttachSize
        };
    }
}
```

### 依賴注入設定 (Program.cs)

```csharp
// 註冊環境變數
builder.Services.AddSysEnvironments();

// DbContext（使用 DbContextFactory）
builder.Services.AddDatabase();

// Options
builder.Services.Configure<Pop3Options>(builder.Configuration.GetSection(Pop3Options.SectionName));

// TraceContext 基礎設施
builder.Services.AddSingleton<TraceContextAccessor>();
builder.Services.AddSingleton<IContextGetter<TraceContext>>(sp => sp.GetRequiredService<TraceContextAccessor>());
builder.Services.AddSingleton<IContextSetter<TraceContext>>(sp => sp.GetRequiredService<TraceContextAccessor>());

// 分層註冊
builder.Services.AddScoped<IReceiveEmailRepository, ReceiveEmailRepository>();
builder.Services.AddScoped<IEmailReceiveAdapter, Pop3EmailReceiveAdapter>();
builder.Services.AddScoped<ReceiveEmailHandler>();
```

### Middleware 管線設定 (Program.cs)

```csharp
// Middleware 管線順序（由外到內，重要！）
// 1. 例外處理 - 最外層，捕捉所有未處理例外
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 2. 追蹤內容 - 設定 TraceId 與 TraceContext
app.UseMiddleware<TraceContextMiddleware>();

// 3. 請求日誌 - 記錄請求與回應資訊
app.UseMiddleware<RequestLoggingMiddleware>();
```

**注意:** Middleware 的順序非常重要，必須按照上述順序註冊。

### 日誌記錄

本專案使用 **Serilog** 進行結構化日誌記錄。

#### 設定 (Program.cs)
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/host-.txt", rollingInterval: RollingInterval.Hour)
    .CreateLogger();

builder.Host.UseSerilog();
```

#### 使用方式
```csharp
// 注入 ILogger<T>（Serilog 會自動接管）
private readonly ILogger<ReceiveEmailHandler> _logger;

// 記錄資訊
_logger.LogInformation("開始接收郵件");

// 記錄錯誤（包含例外物件）
_logger.LogError(ex, "從 POP3 伺服器取得郵件時發生錯誤");
```

#### 日誌輸出位置
- **主控台**: 即時顯示
- **檔案**: `logs/host-{timestamp}.txt`（每小時輪替）

### Entity Framework 指令

```bash
# 建立 Migration
dotnet ef migrations add MigrationName

# 更新資料庫
dotnet ef database update

# 移除最後一個 Migration
dotnet ef migrations remove
```

## 測試與執行

### 執行專案

#### 方式一：使用環境變數
```bash
cd src/EmailReceiver.WebApi

# 設定環境變數
export SYS_DATABASE_CONNECTION_STRING="Server=localhost;Database=EmailReceiverDb;..."

dotnet restore
dotnet build
dotnet run
```

#### 方式二：使用本地設定檔 (推薦)
```bash
cd src/EmailReceiver.WebApi

# 建立 env/local.env 檔案並設定環境變數
# 然後使用 --local 參數執行
dotnet run --local
```

### 存取點
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000
- Swagger UI: https://localhost:5001/swagger

### 測試 API
```bash
# 接收郵件
curl -X POST https://localhost:5001/api/v1/emails/receive -k
```

### 執行測試

本專案使用 **SpecFlow** 進行 BDD 整合測試。

#### 執行所有測試
```bash
cd tests/EmailReceiver.IntegrationTest
dotnet test
```

#### 測試專案結構
- **EmailReceiver.IntegrationTest**: BDD 整合測試專案
  - 使用 SpecFlow + xUnit
  - 測試場景位於 `Email/收信.feature`
  - 測試步驟實作位於 `Email/收信_Step.cs`
  - 使用 `FakeEmailReceiveAdapter` 模擬 POP3 伺服器回應

- **EmailReceiver.Testing.Common**: 測試共用專案
  - 提供 `TestContainerFactory` 用於建立測試容器
  - 提供 `MockServer` 工具用於模擬外部服務
  - 提供資料庫腳本生成工具

#### 測試特色
- 使用 Docker Testcontainers 建立隔離的測試環境
- 使用 Fake Adapter 模擬 POP3 伺服器，避免真實連線
- BDD 測試場景以中文 Gherkin 語法撰寫，易於閱讀理解

## 常見問題與注意事項

### POP3 連線設定
- Gmail 需要啟用「兩步驟驗證」並使用「應用程式密碼」
- 目前 Port 設定為 110（一般 POP3），若使用 SSL 建議改為 995

### 錯誤處理
- 所有方法都返回 `Result<TValue, Failure>` 而非拋出例外
- Controller 層檢查 `result.IsFailure` 並透過 `Failure.ToActionResult()` 返回適當的 HTTP 狀態碼
- ExceptionHandlingMiddleware 捕捉所有未預期的例外並轉換為統一格式
- 使用 Serilog 記錄錯誤訊息（包含結構化資料）
- 每個錯誤都包含 TraceId 以便追蹤

### 效能考量
- 使用交易確保 letters 和 mailReplay 表的資料一致性
- Repository 層處理資料庫操作的異常情況
- 使用 DbContextFactory 而非直接注入 DbContext，提升並行效能
- 建議為常用查詢欄位（如 s_date, status）建立索引

### 環境設定
- **本地開發**: 使用 `--local` 參數載入 `env/local.env`
- **容器部署**: 透過環境變數注入設定（如 `SYS_DATABASE_CONNECTION_STRING`）
- **設定優先順序**: 環境變數 > appsettings.json

### Middleware 管線
- **順序很重要**: ExceptionHandling → TraceContext → RequestLogging
- ExceptionHandlingMiddleware 必須在最外層，才能捕捉所有例外
- TraceContextMiddleware 設定 TraceId 後，後續 Middleware 和服務都可使用
- RequestLoggingMiddleware 記錄完整的請求與回應資訊

### 測試
- 整合測試使用 Testcontainers 建立隔離的 SQL Server 容器
- 使用 FakeEmailReceiveAdapter 模擬 POP3 伺服器回應
- BDD 測試場景以中文撰寫，便於產品人員理解
- 測試資料庫會在測試結束後自動清理

## 擴充建議

### 可能的功能增強
1. **分頁支援**: 為 GET /api/v1/emails 新增分頁參數
2. **郵件搜尋**: 新增依主旨、寄件者、處理狀態搜尋功能
3. **排程接收**: 使用 Hangfire 或 Quartz.NET 定時接收郵件
4. **附件實作**: 實作附件上傳、下載功能（資料表已支援附件欄位）
5. **客服管理**: 新增客服指派、處理狀態更新等管理功能
6. **回信功能**: 實作郵件回覆功能，更新 reply 和 r_date 欄位
7. **IMAP 支援**: 新增 IMAP 協定支援
8. **統計報表**: 新增郵件處理統計、客服績效報表

### 測試現況與建議

**已完成:**
- ✅ BDD 整合測試專案 (`EmailReceiver.IntegrationTest`)
- ✅ 測試容器支援 (`Testcontainers`)
- ✅ Fake Adapter 模擬 POP3 伺服器
- ✅ 測試輔助專案 (`EmailReceiver.Testing.Common`)

**未來建議:**
1. 新增單元測試專案（針對個別類別）
2. 擴充整合測試場景（處理狀態變更、附件處理等）
3. 新增效能測試
4. 新增 API 合約測試

## Git 資訊

- 預設分支: `main`
- 最新提交: 修改 Pop3 端口號為 110

---

**更新日期**: 2025-12-13  
**框架版本**: .NET 9.0  
**專案類型**: Web API
