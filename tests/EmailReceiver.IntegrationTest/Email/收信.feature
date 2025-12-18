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
        Then 預期資料庫已存在 letters 資料為
          | sender             | s_email            | towhom | s_date                    | s_subject    | s_question         | circumstance     | ok |
          | sender@example.com | sender@example.com | 1111   | 2000-01-01T00:00:00+00:00 | 測試郵件主旨 | 這是測試郵件的內容 | -使用敢言、感言- | 2  |
        Then 預期資料庫已存在 mailReplay 資料為
          | mailFrom           | mailFromName       | mailSubject  | mailDate                  | mailBody           | mailType         | tracker | lNo | mailAttach | mailAttachName | mailAttachSize |
          | sender@example.com | sender@example.com | 測試郵件主旨 | 2000-01-01T00:00:00+00:00 | 這是測試郵件的內容 | -使用敢言、感言- |         | 1   |            |                | 0              |

    Scenario: 天氣
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
        When 調用端發送 "GET" 請求至 "WeatherForecast"
        Then 預期得到 HttpStatusCode 為 "200"

    Scenario: 測試 API Header-2
        Given 調用端已準備以下 Query 參數
            | userId | description                          |
            | yao    | 我跟你說你不要跟別人說，你若跟別人說，不要跟別人說，是我叫你不要跟別人說 |
        When 調用端發送 "GET" 請求至 "api/v1/tests"
        Then 預期回傳內容為
        """
        {
          "userId": "yao",
          "description": "我跟你說你不要跟別人說，你若跟別人說，不要跟別人說，是我叫你不要跟別人說"
        }
        """