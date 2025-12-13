using System.Text.Json;
using System.Text.Json.Nodes;
using EmailReceiver.WebApi.EmailReceiver.Adpaters;
using EmailReceiver.WebApi.EmailReceiver.Models.Responses;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll;

namespace EmailReceiver.IntegrationTest.Email;

[Binding]
public class 收信_Step : Steps
{
    [Given(@"已存在 Json 內容")]
    public void Given已存在Json內容(string json)
    {
        var jsonNode = JsonNode.Parse(json);
        this.ScenarioContext.SetJsonNode(jsonNode);
    }

    [When(@"模擬呼叫 API，得到以下內容")]
    public void When模擬呼叫api得到以下內容(string json)
    {
        this.ScenarioContext.SetHttpResponseBody(json);
    }

    [Given(@"模擬 POP3 伺服器回傳以下郵件")]
    public void Given模擬POP3伺服器回傳以下郵件(string json)
    {
        var serviceProvider = this.ScenarioContext.GetServiceProvider();
        var fakeAdapter = serviceProvider.GetRequiredService<IEmailReceiveAdapter>() as FakeEmailReceiveAdapter;
        
        if (fakeAdapter == null)
        {
            throw new InvalidOperationException("IEmailReceiveAdapter 不是 FakeEmailReceiveAdapter 類型");
        }

        var emails = JsonSerializer.Deserialize<List<EmailMessageResponse>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        fakeAdapter.SetEmails(emails ?? new List<EmailMessageResponse>());
    }

    [Given(@"模擬 POP3 伺服器無郵件")]
    public void Given模擬POP3伺服器無郵件()
    {
        var serviceProvider = this.ScenarioContext.GetServiceProvider();
        var fakeAdapter = serviceProvider.GetRequiredService<IEmailReceiveAdapter>() as FakeEmailReceiveAdapter;
        
        if (fakeAdapter == null)
        {
            throw new InvalidOperationException("IEmailReceiveAdapter 不是 FakeEmailReceiveAdapter 類型");
        }

        fakeAdapter.Clear();
    }
}