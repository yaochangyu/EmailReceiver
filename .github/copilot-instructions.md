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
- 儲存郵件（UIDL、主旨、內容、寄件者、收件者）至 MSSQL 資料庫
- 使用 Result Pattern 進行錯誤處理
- 提供 RESTful API 端點
- Swagger API 文件

## 技術堆疊

### 框架與函式庫
- **.NET 9.0** - 主要開發框架
- **MailKit 4.14.1** - POP3 郵件處理
- **Entity Framework Core 9.0** - ORM 資料存取
- **SQL Server** - 資料庫
- **CSharpFunctionalExtensions 3.1.0** - Result Pattern 實作
- **Swashbuckle.AspNetCore 10.0.1** - Swagger/OpenAPI 文件

### 開發原則
- **Clean Architecture** - 分層架構設計
- **Result Pattern** - 使用 `Result<T>` 進行錯誤處理，避免例外處理
- **Repository Pattern** - 封裝資料存取邏輯
- **依賴注入** - 使用 .NET 內建 DI 容器
- **不可變物件設計** - Entity 使用 `init` 關鍵字

## 專案結構

```
EmailReceiver.WebApi/
├── Controllers/          # API 控制器層
│   └── EmailsController.cs
├── Handlers/            # 業務邏輯處理層
│   └── ReceiveEmailsHandler.cs
├── Services/            # 服務層（POP3 收信）
│   ├── IEmailReceiveService.cs
│   └── Pop3EmailReceiveService.cs
├── Repositories/        # 資料存取層
│   ├── IEmailMessageRepository.cs
│   └── EmailMessageRepository.cs
├── Entities/            # 資料實體（不可變物件）
│   └── EmailMessage.cs
├── Data/                # DbContext
│   └── EmailReceiverDbContext.cs
├── Models/              # DTO 和 Response Models
│   └── Responses/
│       ├── EmailMessageResponse.cs
│       └── ReceiveEmailsResponse.cs
├── Options/             # 設定選項類別
│   └── Pop3Options.cs
└── Program.cs           # 應用程式進入點與 DI 設定
```

## 架構流程

### 請求流程（Clean Architecture）
```
API Request
    ↓
Controller (EmailsController)
    ↓
Handler (ReceiveEmailsHandler)
    ↓
Service (Pop3EmailReceiveService) ← 取得郵件
    ↓
Repository (EmailMessageRepository) ← 儲存郵件
    ↓
Database (SQL Server)
```

### 關鍵類別說明

#### 1. Controller 層
- **EmailsController**: 提供兩個端點
  - `POST /api/emails/receive` - 接收郵件
  - `GET /api/emails` - 取得所有郵件

#### 2. Handler 層
- **ReceiveEmailsHandler**: 協調郵件接收流程
  1. 呼叫 Service 取得郵件
  2. 透過 Repository 查詢已存在的 UIDL
  3. 過濾新郵件
  4. 批次儲存新郵件

#### 3. Service 層
- **Pop3EmailReceiveService**: 實作 POP3 郵件接收
  - 連線至 POP3 伺服器
  - 取得郵件訊息與 UIDL
  - 返回 `Result<IReadOnlyList<EmailMessageResponse>>`

#### 4. Repository 層
- **EmailMessageRepository**: 封裝資料存取
  - `AddAsync` - 新增單筆郵件
  - `AddRangeAsync` - 批次新增郵件
  - `GetAllAsync` - 取得所有郵件
  - `GetAllUidlsAsync` - 取得所有 UIDL（用於去重）

#### 5. Entity 層
- **EmailMessage**: 不可變郵件實體
  - 使用 `init` 屬性設定
  - 提供靜態工廠方法 `Create()`
  - 欄位: Id, Uidl, Subject, Body, From, To, ReceivedAt, CreatedAt

## 資料庫設計

### EmailMessages 資料表（目前專案使用）

| 欄位 | 型別 | 說明 | 索引 |
|------|------|------|------|
| Id | uniqueidentifier | 主鍵 | PK |
| Uidl | nvarchar(500) | 郵件唯一識別碼 | Unique Index |
| Subject | nvarchar(1000) | 郵件主旨 | - |
| Body | nvarchar(max) | 郵件內容 | - |
| From | nvarchar(500) | 寄件者 | - |
| To | nvarchar(500) | 收件者 | - |
| ReceivedAt | datetime2 | 郵件接收時間 | Index |
| CreatedAt | datetime2 | 資料建立時間 | - |

### Entity Framework 設定
- Uidl 欄位具有唯一索引，防止重複儲存
- ReceivedAt 欄位建立索引，加速查詢排序

### 參考資料表（舊系統架構）

專案根目錄的 `db/` 資料夾包含兩個參考用的 SQL 資料表定義檔，這些是舊系統的設計，可作為未來功能擴充的參考：

#### 1. letters 資料表（`db/letters.sql`）
舊系統的**來信管理主表**，包含完整的信件處理流程欄位：

**核心欄位群組：**
- **來信者資訊**：姓名、Email、手機、身分證字號、生日
- **信件基本資訊**：主旨、內容、來信日期、收信人、來信 IP
- **附件管理**：支援 5 個附件欄位（s_file1 ~ s_file5）
- **回信處理**：回信處理方式、處理人員、處理內容、回信日期
- **狀態管理**：處理狀態（已處理/未處理/暫擱）、問題類別
- **進階功能**：追蹤狀態、客服指派、重複回信機制（reply1、dateReply1）

**設計特色：**
- 使用 `NTEXT` 儲存長文本內容
- 包含 `rowguid` 與 `rowguid37` 雙 GUID 設計
- 支援多檔案附件
- 完整的客服處理流程欄位

#### 2. mailReplay 資料表（`db/mailReplay.sql`）
舊系統的**郵件回覆管理表**，專門記錄回信處理：

**核心欄位群組：**
- **寄件者資訊**：寄件 Email、寄件者姓名
- **信件內容**：標題、內容、寄信日期
- **附件管理**：附件檔名、顯示名稱、檔案大小（支援多檔案）
- **處理流程**：處理狀態、客服人員、建立日期
- **關聯設計**：透過 `lNo` 欄位關聯至 letters 資料表

**設計特色：**
- 使用 `status` 欄位管理狀態（待處理/結案/刪除）
- 包含 `mailType` 分類欄位
- 與 letters 表的 `lNo` 建立關聯

#### 與目前系統的差異

| 面向 | 目前系統 (EmailMessages) | 舊系統 (letters/mailReplay) |
|------|-------------------------|---------------------------|
| 架構設計 | 單一資料表，簡化設計 | 雙表設計，分離來信與回信 |
| 附件處理 | 不支援 | 支援多檔案附件 |
| 處理流程 | 僅儲存郵件 | 完整的客服處理流程 |
| 欄位數量 | 8 個核心欄位 | letters 43 個欄位 + mailReplay 16 個欄位 |
| 資料型別 | 使用現代化型別（datetime2） | 使用舊型別（NTEXT、SMALLDATETIME） |
| 識別機制 | UIDL (POP3 標準) | 自動編號 + GUID |

#### 未來擴充方向參考

若需要將目前系統擴充為完整的客服郵件管理系統，可參考舊系統設計：

1. **附件處理**：新增附件資料表或欄位
2. **處理流程**：新增處理狀態、處理人員、回信內容等欄位
3. **分類機制**：新增郵件類別、問題類別欄位
4. **客服指派**：新增指派機制與追蹤功能
5. **回信管理**：考慮是否需要分離回信至獨立資料表

## API 端點

### 1. 接收郵件
```http
POST /api/emails/receive
```

**回應範例:**
```json
{
  "savedCount": 5,
  "message": "成功接收並儲存 5 封郵件"
}
```

### 2. 取得所有郵件
```http
GET /api/emails
```

**回應範例:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "uidl": "1234567890abcdef",
    "subject": "測試郵件",
    "body": "這是郵件內容",
    "from": "sender@example.com",
    "to": "receiver@example.com",
    "receivedAt": "2025-12-10T10:30:00Z",
    "createdAt": "2025-12-10T10:35:00Z"
  }
]
```

## 設定檔

### appsettings.json

#### 資料庫連線
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EmailReceiverDb;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;"
  }
}
```

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

## 開發指引

### 程式碼風格

#### 1. 命名慣例
- **類別/介面**: PascalCase (例如: `EmailMessage`, `IEmailReceiveService`)
- **方法**: PascalCase (例如: `HandleAsync`, `GetAllAsync`)
- **參數/區域變數**: camelCase (例如: `cancellationToken`, `emailMessage`)
- **私有欄位**: _camelCase (例如: `_repository`, `_logger`)

#### 2. 非同步方法
- 所有 I/O 操作都應使用非同步方法
- 方法名稱以 `Async` 結尾
- 接受 `CancellationToken` 參數

#### 3. Result Pattern 使用
```csharp
// 成功情況
return Result.Success(value);

// 失敗情況
return Result.Failure<T>("錯誤訊息");

// 檢查結果
if (result.IsFailure)
{
    return Result.Failure<T>(result.Error);
}
```

#### 4. 不可變物件設計
```csharp
public sealed class EmailMessage
{
    public Guid Id { get; init; }
    public string Uidl { get; init; }
    // ... 其他屬性
    
    private EmailMessage() { }
    
    public static EmailMessage Create(...)
    {
        return new EmailMessage { ... };
    }
}
```

### 依賴注入設定 (Program.cs)

```csharp
// DbContext
builder.Services.AddDbContext<EmailReceiverDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Options
builder.Services.Configure<Pop3Options>(builder.Configuration.GetSection(Pop3Options.SectionName));

// 分層註冊
builder.Services.AddScoped<IEmailMessageRepository, EmailMessageRepository>();
builder.Services.AddScoped<IEmailReceiveService, Pop3EmailReceiveService>();
builder.Services.AddScoped<ReceiveEmailsHandler>();
```

### 日誌記錄

使用 `ILogger<T>` 進行日誌記錄：

```csharp
_logger.LogInformation("開始接收郵件");
_logger.LogError(ex, "從 POP3 伺服器取得郵件時發生錯誤");
```

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
```bash
cd src/EmailReceiver.WebApi
dotnet restore
dotnet build
dotnet run
```

### 存取點
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000
- Swagger UI: https://localhost:5001/swagger

### 測試 API
```bash
# 接收郵件
curl -X POST https://localhost:5001/api/emails/receive -k

# 取得所有郵件
curl https://localhost:5001/api/emails -k
```

## 常見問題與注意事項

### POP3 連線設定
- Gmail 需要啟用「兩步驟驗證」並使用「應用程式密碼」
- 目前 Port 設定為 110（一般 POP3），若使用 SSL 建議改為 995

### 錯誤處理
- 所有方法都返回 `Result<T>` 或 `Result`
- Controller 層檢查 `result.IsFailure` 並返回適當的 HTTP 狀態碼
- 使用 `_logger.LogError` 記錄錯誤訊息

### 效能考量
- UIDL 去重邏輯使用 `HashSet` 提升效能
- 使用 `AddRangeAsync` 批次新增郵件
- ReceivedAt 欄位建立索引，支援排序查詢

## 擴充建議

### 可能的功能增強
1. **分頁支援**: 為 GET /api/emails 新增分頁參數
2. **郵件搜尋**: 新增依主旨、寄件者搜尋功能
3. **排程接收**: 使用 Hangfire 或 Quartz.NET 定時接收郵件
4. **附件處理**: 擴充郵件實體以支援附件儲存
5. **郵件刪除**: 新增刪除已讀取郵件的功能
6. **IMAP 支援**: 新增 IMAP 協定支援

### 測試建議
1. 新增單元測試專案
2. 使用 Moq 模擬依賴項目
3. 新增整合測試驗證 POP3 連線
4. 新增 API 端對端測試

## Git 資訊

- 預設分支: `main`
- 最新提交: 修改 Pop3 端口號為 110

---

**更新日期**: 2025-12-13  
**框架版本**: .NET 9.0  
**專案類型**: Web API
