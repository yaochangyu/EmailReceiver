# Middleware 管線與 Failure 格式實作說明

## 概述

已成功實作完整的 Middleware 管線與統一的 Failure 錯誤處理格式，符合 api.template 專案的編碼原則。

## 實作內容

### 1. 錯誤處理基礎設施

#### FailureCode 列舉
`Infrastructure/ErrorHandling/FailureCode.cs`
- 定義所有錯誤代碼類型
- 包含：Unauthorized, DbError, DuplicateEmail, Pop3ConnectionError, EmailReceiveError 等

#### Failure Record
`Infrastructure/ErrorHandling/Failure.cs`
- **Code**: 錯誤代碼（使用 `nameof(FailureCode.*)` 格式）
- **Message**: 錯誤訊息
- **TraceId**: 追蹤識別碼（由 TraceContext 提供）
- **Exception**: 原始例外物件（不序列化到客戶端）
- **Data**: 額外的結構化資料

提供便利方法：
```csharp
Failure.DbError("錯誤訊息", exception);
Failure.Pop3Error("錯誤訊息", exception);
Failure.EmailReceiveError("錯誤訊息", exception);
Failure.ValidationError("錯誤訊息", validationErrors);
```

#### FailureCodeMapper
`Infrastructure/ErrorHandling/FailureCodeMapper.cs`
- 將 FailureCode 映射至 HTTP 狀態碼
- 提供 `.ToActionResult()` 擴充方法

### 2. TraceContext 追蹤機制

#### TraceContext Record
`Infrastructure/TraceContext/TraceContext.cs`
```csharp
public sealed record TraceContext
{
    public required string TraceId { get; init; }
    public string? UserId { get; init; }
    public DateTime RequestStartTime { get; init; }
}
```

#### IContextGetter/IContextSetter 介面
- `IContextGetter<T>`: 取得內容
- `IContextSetter<T>`: 設定內容

#### TraceContextAccessor
`Infrastructure/TraceContext/TraceContextAccessor.cs`
- 使用 `AsyncLocal<T>` 確保執行緒安全
- 在整個請求生命週期內可用

### 3. Middleware 管線

#### 管線順序（由外到內）

```
ExceptionHandlingMiddleware (最外層)
    ↓
TraceContextMiddleware
    ↓
RequestLoggingMiddleware
    ↓
ASP.NET Core Pipeline
    ↓
Controller → Handler → Repository
```

#### ExceptionHandlingMiddleware
`Infrastructure/Middleware/ExceptionHandlingMiddleware.cs`

**職責**：
- 捕捉系統層級的未處理例外
- 記錄完整的錯誤資訊與請求參數
- 返回統一的 Failure 格式回應

**功能**：
- 自動擷取請求資訊（路由、查詢參數、標頭、本文）
- 過濾敏感標頭（Authorization, Cookie, X-API-Key 等）
- 環境區分：開發環境顯示詳細訊息，生產環境隱藏
- 記錄結構化日誌（包含 TraceId）

#### TraceContextMiddleware
`Infrastructure/Middleware/TraceContextMiddleware.cs`

**職責**：
- 從請求標頭擷取或產生 TraceId
- 設定 TraceContext 至 AsyncLocal
- 將 TraceId 加入回應標頭

**流程**：
1. 檢查 `X-Trace-Id` 標頭，若無則產生新的 GUID
2. 建立 TraceContext 物件
3. 透過 IContextSetter 設定到 AsyncLocal
4. 將 TraceId 加入回應標頭供客戶端追蹤
5. 記錄請求開始與完成時間

#### RequestLoggingMiddleware
`Infrastructure/Middleware/RequestLoggingMiddleware.cs`

**職責**：
- 記錄請求完成資訊
- 根據回應狀態碼記錄不同等級日誌

**日誌等級**：
- 200-399: Information
- 400-499: Warning
- 500+: Error

### 4. 分層錯誤處理更新

#### Controller 層
```csharp
public async Task<IActionResult> ReceiveEmails(CancellationToken cancellationToken)
{
    var result = await _receiveEmailHandler.HandleAsync(cancellationToken);

    if (result.IsFailure)
    {
        var traceContext = _contextGetter.Get();
        var failure = result.Error with { TraceId = traceContext?.TraceId };
        return failure.ToActionResult();
    }

    return Ok(response);
}
```

**改進**：
- 移除日誌記錄（由 Middleware 集中處理）
- 使用統一的 Failure.ToActionResult()
- 自動加入 TraceId

#### Handler 層
```csharp
public async Task<Result<int, Failure>> HandleAsync(CancellationToken cancellationToken)
{
    var fetchResult = await _emailReceiveAdapter.FetchEmailsAsync(cancellationToken);
    if (fetchResult.IsFailure)
    {
        return Result.Failure<int, Failure>(fetchResult.Error);
    }
    
    // ... 業務邏輯
}
```

**改進**：
- 移除所有 `ILogger` 注入與日誌記錄
- 返回 `Result<T, Failure>` 型別
- 錯誤直接傳播，不記錄

#### Repository 層
```csharp
public async Task<Result<int, Failure>> AddAsync(InsertEmailRequest request, CancellationToken cancellationToken)
{
    try
    {
        // ... 資料庫操作
        return Result.Success<int, Failure>(letter.LNo);
    }
    catch (Exception ex)
    {
        return Result.Failure<int, Failure>(
            Failure.DbError("儲存郵件時發生錯誤", ex));
    }
}
```

**改進**：
- 捕捉例外並封裝到 Failure.Exception
- 使用 Failure.DbError() 建立錯誤物件
- 移除日誌記錄

#### Adapter 層
```csharp
public async Task<Result<IReadOnlyList<EmailMessageResponse>, Failure>> FetchEmailsAsync(CancellationToken cancellationToken)
{
    try
    {
        // ... POP3 操作
        return Result.Success<IReadOnlyList<EmailMessageResponse>, Failure>(emails);
    }
    catch (Exception ex)
    {
        return Result.Failure<IReadOnlyList<EmailMessageResponse>, Failure>(
            Failure.Pop3Error("從 POP3 伺服器取得郵件時發生錯誤", ex));
    }
}
```

**改進**：
- 移除 ILogger 依賴
- 使用 Failure.Pop3Error()
- 例外封裝到 Failure.Exception

## 核心原則符合度

### ✅ 已符合

1. **集中式日誌管理**
   - 所有日誌統一在 Middleware 記錄
   - Handler/Repository 不再記錄日誌

2. **統一錯誤格式**
   - 使用 Failure record 統一錯誤結構
   - 所有 Result 返回 `Result<T, Failure>`

3. **TraceContext 追蹤**
   - 使用 AsyncLocal 管理 TraceContext
   - 自動加入 TraceId 到日誌與回應

4. **例外封裝**
   - 所有例外都保存在 Failure.Exception
   - Middleware 從 Exception 讀取並記錄

5. **分層職責明確**
   - Middleware: 日誌記錄、例外處理、追蹤管理
   - Handler: 業務邏輯協調
   - Repository: 資料存取
   - Adapter: 外部服務整合

## 使用方式

### Program.cs 設定

```csharp
// 註冊 TraceContext 基礎設施
builder.Services.AddSingleton<TraceContextAccessor>();
builder.Services.AddSingleton<IContextGetter<TraceContext>>(sp => sp.GetRequiredService<TraceContextAccessor>());
builder.Services.AddSingleton<IContextSetter<TraceContext>>(sp => sp.GetRequiredService<TraceContextAccessor>());

// 註冊 Middleware（順序很重要）
app.UseMiddleware<ExceptionHandlingMiddleware>();  // 最外層
app.UseMiddleware<TraceContextMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
```

### 錯誤回應格式

**成功回應**:
```json
{
  "savedCount": 5,
  "message": "成功接收並儲存 5 封郵件"
}
```

**錯誤回應**:
```json
{
  "code": "Pop3ConnectionError",
  "message": "從 POP3 伺服器取得郵件時發生錯誤",
  "traceId": "a1b2c3d4e5f6",
  "data": {
    "exceptionType": "SocketException",
    "timestamp": "2025-12-13T03:25:00Z"
  }
}
```

### TraceId 使用

**請求標頭**:
```
X-Trace-Id: custom-trace-id-123
```

**回應標頭**:
```
X-Trace-Id: custom-trace-id-123
```

所有日誌都會包含此 TraceId，方便追蹤完整請求流程。

## 日誌範例

### 正常請求
```
[Information] Request started - TraceId: a1b2c3d4, Path: /api/v1/emails/receive
[Information] Request completed successfully - RequestInfo: {...}
[Information] Request completed - TraceId: a1b2c3d4, Elapsed: 1234ms
```

### 錯誤請求
```
[Information] Request started - TraceId: x1y2z3w4, Path: /api/v1/emails/receive
[Error] Unhandled exception - TraceId: x1y2z3w4, RequestInfo: {...}
  System.Exception: POP3 連線失敗
```

## 改進後的優勢

1. **可追蹤性**：每個請求都有唯一 TraceId，方便問題追蹤
2. **集中管理**：日誌、例外、追蹤都在 Middleware 集中處理
3. **職責分離**：業務邏輯層專注於業務邏輯，不處理橫切關注點
4. **統一格式**：所有錯誤都使用相同的 Failure 格式
5. **安全性**：自動過濾敏感標頭，環境區分錯誤詳細程度
6. **可測試性**：業務邏輯層不依賴 ILogger，更容易測試

## 後續建議

1. **加入 Serilog**：結構化日誌輸出至 Seq
2. **加入健康檢查**：`/health` 端點
3. **加入 FluentValidation**：輸入驗證
4. **建立 BDD 測試**：使用 Testcontainers 進行整合測試
5. **加入 API 版本控制**：統一使用 `api/v1/` 路由

## 測試驗證

建置成功，所有編譯錯誤已解決：
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```
