Feature: 收信

    Background:
        Given 調用端已準備 Header 參數
          | x-trace-id |
          | TW         |
        Given 調用端已準備 Query 參數
          | select-profile |
          | avatarUrl      |
        Given 初始化測試伺服器
          | Now                       | UserId |
          | 2000-01-01T00:00:00+00:00 | yao    |

    Scenario: 收信
        Given 調用端已準備 Body 參數(Json)
        """
        {
          "email": "yao@9527",
          "name": "yao",
          "age": 18
        }
        """
        When 調用端發送 "POST" 請求至 "api/v1/emails/receive"
        Then 預期得到 HttpStatusCode 為 "204"
        Then 預期資料庫已存在 Member 資料為
          | Email    | Name | Age | CreatedAt                 | CreatedAt                 |
          | yao@9527 | yao  | 18  | 2000-01-01T00:00:00+00:00 | 2000-01-01T00:00:00+00:00 |