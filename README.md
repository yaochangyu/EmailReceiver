# EmailReceiver Web API

使用 C# .NET 9.0 + MailKit 開發的 POP3 收信功能 Web API。

## 功能特色

- 使用 POP3 協定收取郵件
- 將郵件（UIDL、主旨、內容）儲存至 MSSQL 資料庫
- 遵循清潔架構（Clean Architecture）原則
- 使用 Result Pattern 進行錯誤處理
- 支援 Swagger API 文件

## 技術架構

- **Framework**: .NET 9.0
- **郵件處理**: MailKit (POP3)
- **資料庫**: Entity Framework Core 9.0 + SQL Server
- **錯誤處理**: CSharpFunctionalExtensions (Result Pattern)
- **API 文件**: Swagger/OpenAPI

## 專案結構

```
EmailReceiver.WebApi/
├── Controllers/        # API 控制器
├── Handlers/          # 商業邏輯處理器
├── Services/          # POP3 收信服務
├── Repositories/      # 資料存取層
├── Entities/          # 資料實體（不可變物件）
├── Data/              # DbContext
├── Models/            # DTO 和 Response Models
└── Options/           # 設定選項類別
```

## 架構圖

請參考 [收信功能循序圖](./docs/receive-emails-sequence.md)

## 環境需求

- .NET 9.0 SDK
- SQL Server 2019 或更新版本
- POP3 郵件帳號

## 安裝與設定

### 1. 設定資料庫連線

編輯 `appsettings.json`：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EmailReceiverDb;User Id=sa;Password=YourPassword123;TrustServerCertificate=True;"
  }
}
```

### 2. 設定 POP3 郵件帳號

編輯 `appsettings.json`：

```json
{
  "Pop3": {
    "Host": "pop.gmail.com",
    "Port": 995,
    "UseSsl": true,
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

**Gmail 使用者注意事項：**
- 需要啟用「兩步驟驗證」
- 使用「應用程式密碼」，而非帳號密碼
- 設定路徑：Google 帳戶 > 安全性 > 兩步驟驗證 > 應用程式密碼

### 3. 建立資料庫

執行資料庫遷移：

```bash
cd src/EmailReceiver.WebApi
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. 執行專案

```bash
dotnet run
```

API 將在以下位置執行：
- HTTPS: https://localhost:5001
- HTTP: http://localhost:5000
- Swagger UI: https://localhost:5001/swagger

## API 端點

### 1. 接收郵件

**POST** `/api/emails/receive`

從 POP3 伺服器接收郵件並儲存至資料庫。

**回應範例：**

```json
{
  "savedCount": 5,
  "message": "成功接收並儲存 5 封郵件"
}
```

### 2. 取得所有郵件

**GET** `/api/emails`

取得資料庫中所有已儲存的郵件。

**回應範例：**

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

## 使用範例

### 使用 curl

```bash
# 接收郵件
curl -X POST https://localhost:5001/api/emails/receive

# 取得所有郵件
curl https://localhost:5001/api/emails
```

### 使用 Swagger UI

1. 啟動專案後，開啟瀏覽器
2. 前往 https://localhost:5001/swagger
3. 測試 API 端點

## 資料庫結構

### EmailMessages 表格

| 欄位 | 型別 | 說明 |
|------|------|------|
| Id | uniqueidentifier | 主鍵 |
| Uidl | nvarchar(500) | 郵件唯一識別碼（唯一索引）|
| Subject | nvarchar(1000) | 郵件主旨 |
| Body | nvarchar(max) | 郵件內容 |
| From | nvarchar(500) | 寄件者 |
| To | nvarchar(500) | 收件者 |
| ReceivedAt | datetime2 | 郵件接收時間（索引）|
| CreatedAt | datetime2 | 資料建立時間 |

## 開發原則

本專案遵循以下開發原則：

1. **不可變物件設計**：Entity 使用 `init` 關鍵字
2. **清潔架構**：Controller → Handler → Service/Repository → Database
3. **Result Pattern**：使用 `CSharpFunctionalExtensions` 處理錯誤
4. **Repository Pattern**：封裝資料存取邏輯
5. **依賴注入**：使用 .NET 內建 DI 容器

## 故障排除

### POP3 連線失敗

- 確認防火牆允許 POP3 連接埠（通常是 995）
- 檢查郵件伺服器設定是否正確
- Gmail 使用者：確認已啟用「低安全性應用程式存取」或使用「應用程式密碼」

### 資料庫連線失敗

- 確認 SQL Server 正在執行
- 檢查連線字串是否正確
- 確認資料庫使用者具有適當權限

## 授權

MIT License
