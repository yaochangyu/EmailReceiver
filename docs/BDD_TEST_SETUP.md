# BDD 測試專案建立說明

## 📦 專案建立成功

已成功建立 **EmailReceiver.IntegrationTest** BDD 測試專案！

---

## 📁 專案結構

```
tests/EmailReceiver.IntegrationTest/
├── Features/
│   └── ReceiveEmails.feature          # Gherkin 情境定義（中文）
├── StepDefinitions/
│   └── ReceiveEmailsSteps.cs          # 步驟定義實作
├── Infrastructure/
│   └── DockerTestEnvironment.cs       # Docker Testcontainers 設定
├── Support/
│   └── Hooks.cs                       # 測試生命週期 Hooks
└── EmailReceiver.IntegrationTest.csproj
```

---

## 🔧 安裝的套件

| 套件 | 版本 | 用途 |
|------|------|------|
| Reqnroll.xUnit | 3.2.1 | BDD 框架 (Gherkin 語法) |
| Testcontainers.MsSql | 4.9.0 | Docker SQL Server 容器 |
| FluentAssertions | 8.8.0 | 流暢的斷言語法 |
| Microsoft.AspNetCore.Mvc.Testing | 9.0.0 | ASP.NET Core 測試 |
| xUnit | 2.9.2 | 測試執行器 |

---

## 📝 功能檔案 (ReceiveEmails.feature)

使用 **Gherkin 中文語法** 定義測試情境：

### 已定義的場景

1. **成功接收新郵件**
   ```gherkin
   場景: 成功接收新郵件
     假如 POP3 伺服器有 3 封新郵件
     當 我發送 POST 請求到 "/api/v1/emails/receive"
     那麼 回應狀態碼應該是 200
     而且 回應訊息包含 "成功接收並儲存 3 封郵件"
     而且 資料庫中應該有 3 封郵件記錄
   ```

2. **POP3 伺服器沒有新郵件**
3. **接收郵件時包含 TraceId**
4. **POP3 連線失敗**

---

## 🐳 DockerTestEnvironment

### 架構特色

```csharp
public class DockerTestEnvironment : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlServerContainer;
    private WebApplicationFactory<Program>? _factory;
    
    // 使用 Testcontainers 提供真實的 SQL Server
    // 使用 WebApplicationFactory 建立測試 Web 主機
    // 自動管理容器生命週期
}
```

### 功能

✅ **自動啟動 Docker 容器**
- SQL Server 2022 容器
- 測試專用密碼與連線字串

✅ **自動資料庫初始化**
- EnsureCreated - 建立資料表
- 測試前清空資料

✅ **整合測試環境**
- 使用真實的 Web API
- 完整的 Middleware 管線
- 真實的資料庫連線

---

## 🎯 測試步驟定義 (ReceiveEmailsSteps)

### 已實作的步驟

#### Background
- `Given 測試環境已啟動`
- `Given 資料庫已清空`

#### Given 步驟
- `Given POP3 伺服器有 N 封新郵件`
- `Given POP3 伺服器沒有郵件`
- `Given 我設定請求標頭 "X" 為 "Y"`
- `Given POP3 伺服器無法連線`

#### When 步驟
- `When 我發送 POST 請求到 "/endpoint"`

#### Then 步驟
- `Then 回應狀態碼應該是 200`
- `Then 回應訊息包含 "文字"`
- `Then 資料庫中應該有 N 封郵件記錄`
- `Then 回應標頭 "X" 應該是 "Y"`
- `Then 錯誤代碼應該是 "Code"`
- `Then 回應應該包含 TraceId`

### 使用 FluentAssertions

```csharp
_response.Should().NotBeNull();
((int)_response!.StatusCode).Should().Be(expectedStatusCode);
content.Should().Contain(expectedMessage);
```

---

## 🚧 待完成項目 (TODO)

### 1. POP3 模擬伺服器
目前步驟標記為 `Pending`：
```csharp
[Given(@"POP3 伺服器有 (\d+) 封新郵件")]
public void GivenPOP3伺服器有新郵件(int emailCount)
{
    // TODO: 實作 POP3 模擬伺服器
    ScenarioContext.StepIsPending();
}
```

**解決方案選項**：
1. **使用 WireMock 容器** - 模擬 POP3 回應
2. **使用真實的測試郵件伺服器** - MailHog/MailDev
3. **Mock IEmailReceiveAdapter** - 單元測試等級（不建議）

### 2. 資料庫驗證
```csharp
[Then(@"資料庫中應該有 (\d+) 封郵件記錄")]
public async Task Then資料庫中應該有郵件記錄(int expectedCount)
{
    // TODO: 從 DI 容器取得 DbContext 並查詢
}
```

**實作提示**：
```csharp
using var scope = _testEnvironment.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<EmailReceiverDbContext>();
var count = await context.Letters.CountAsync();
count.Should().Be(expectedCount);
```

### 3. 修正過時的 API
警告: `ScenarioContext.StepIsPending()` 已過時

**修正方式**：
```csharp
// Before
ScenarioContext.StepIsPending();

// After
throw new PendingStepException();
```

---

## ⚙️ 執行測試

### 命令列執行

```bash
# 執行所有測試
dotnet test tests/EmailReceiver.IntegrationTest/

# 執行特定場景
dotnet test --filter "DisplayName~成功接收新郵件"

# 詳細輸出
dotnet test --logger "console;verbosity=detailed"

# 產生測試報告
dotnet test --logger "trx;LogFileName=test-results.trx"
```

### Visual Studio / Rider
1. 開啟 Test Explorer
2. 執行測試 (需要 Docker 執行中)
3. 查看測試結果

---

## 🐳 Docker 需求

### 必須執行 Docker

測試依賴 Docker 容器，執行前確保：

```bash
# 檢查 Docker 狀態
docker --version
docker ps

# 確保 Docker 正在執行
```

### 容器資源

- **SQL Server**: ~2GB RAM
- **總計**: 建議至少 4GB 可用記憶體

---

## 📊 測試生命週期 (Hooks)

### 測試執行流程

```
BeforeTestRun (靜態)
    ↓
BeforeScenario
    ↓
BeforeStep → 執行步驟 → AfterStep
    ↓ (重複)
AfterScenario (包含清理)
    ↓
AfterTestRun (靜態)
```

### 輸出範例

```
=== BDD 測試開始 ===
時間: 2025-12-13 03:50:00

▶ 開始場景: 成功接收新郵件
  → 假如 POP3 伺服器有 3 封新郵件
  → 當 我發送 POST 請求到 "/api/v1/emails/receive"
  → 那麼 回應狀態碼應該是 200
◀ 結束場景: 成功接收新郵件 - ✅ 通過

=== BDD 測試結束 ===
```

---

## 🎯 後續步驟

### 立即可做
1. ✅ 專案已建立
2. ✅ 基礎架構已完成
3. ⏳ 實作 POP3 模擬伺服器
4. ⏳ 完成資料庫驗證步驟
5. ⏳ 修正過時 API 警告

### 短期目標
- 實作完整的測試步驟
- 加入更多測試場景
- 建立測試資料工廠
- 加入覆蓋率報告

### 長期目標
- CI/CD 整合
- 並行測試執行
- 效能測試場景
- 安全測試場景

---

## 🔗 相關文件

- **Reqnroll 文件**: https://reqnroll.net/
- **Testcontainers**: https://dotnet.testcontainers.org/
- **FluentAssertions**: https://fluentassertions.com/
- **Gherkin 語法**: https://cucumber.io/docs/gherkin/

---

## ✅ 建置狀態

```
✅ Build succeeded with 5 warning(s)
⚠️ 5 個警告 (StepIsPending 過時)
❌ 0 個錯誤
```

### 警告說明
所有警告都是 `ScenarioContext.StepIsPending()` 過時警告，這些是 TODO 步驟的標記，待實作完成後會消失。

---

**建立日期**: 2025-12-13  
**專案類型**: BDD 整合測試  
**測試框架**: xUnit + Reqnroll + Testcontainers  
**狀態**: ✅ 基礎架構完成，待實作測試邏輯
