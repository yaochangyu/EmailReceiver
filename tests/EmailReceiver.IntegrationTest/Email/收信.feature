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
        Given 模擬 POP3 伺服器回傳以下郵件
        """
        [
          {
            "Id": "11111111-1111-1111-1111-111111111111",
            "Uidl": "unique-id-001",
            "Subject": "測試郵件主旨",
            "Body": "這是測試郵件的內容",
            "From": "sender@example.com",
            "To": "yao@9527",
            "ReceivedAt": "2000-01-01T00:00:00+00:00",
            "CreatedAt": "2000-01-01T00:00:00+00:00"
          }
        ]
        """
        When 調用端發送 "POST" 請求至 "api/v1/emails/receive"
        Then 預期得到 HttpStatusCode 為 "200"
#        Then 預期資料庫已存在 Member 資料為
#          | Email    | Name | Age | CreatedAt                 | CreatedAt                 |
#          | yao@9527 | yao  | 18  | 2000-01-01T00:00:00+00:00 | 2000-01-01T00:00:00+00:00 |