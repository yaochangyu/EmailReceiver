using EmailReceiver.WebApi.Handlers;
using EmailReceiver.WebApi.Models.Responses;
using EmailReceiver.WebApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmailReceiver.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailsController : ControllerBase
{
    private readonly ReceiveEmailsHandler _receiveEmailsHandler;
    private readonly IEmailMessageRepository _repository;
    private readonly ILogger<EmailsController> _logger;

    public EmailsController(
        ReceiveEmailsHandler receiveEmailsHandler,
        IEmailMessageRepository repository,
        ILogger<EmailsController> logger)
    {
        _receiveEmailsHandler = receiveEmailsHandler;
        _repository = repository;
        _logger = logger;
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveEmails(CancellationToken cancellationToken)
    {
        _logger.LogInformation("接收到收信請求");

        var result = await _receiveEmailsHandler.HandleAsync(cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogError("收信失敗: {Error}", result.Error);
            return BadRequest(new { error = result.Error });
        }

        var response = new ReceiveEmailsResponse(
            SavedCount: result.Value,
            Message: $"成功接收並儲存 {result.Value} 封郵件"
        );

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEmails(CancellationToken cancellationToken)
    {
        _logger.LogInformation("取得所有郵件");

        var result = await _repository.GetAllAsync(cancellationToken);

        if (result.IsFailure)
        {
            _logger.LogError("取得郵件清單失敗: {Error}", result.Error);
            return BadRequest(new { error = result.Error });
        }

        var response = result.Value.Select(e => new EmailMessageResponse(
            Id: e.Id,
            Uidl: e.Uidl,
            Subject: e.Subject,
            Body: e.Body,
            From: e.From,
            To: e.To,
            ReceivedAt: e.ReceivedAt,
            CreatedAt: e.CreatedAt
        )).ToList();

        return Ok(response);
    }
}
