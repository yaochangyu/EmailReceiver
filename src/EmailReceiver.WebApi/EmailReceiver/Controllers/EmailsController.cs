using EmailReceiver.WebApi.EmailReceiver.Models.Responses;
using EmailReceiver.WebApi.EmailReceiver.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmailReceiver.WebApi.EmailReceiver.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailsController : ControllerBase
{
    private readonly ReceiveEmailHandler _receiveEmailHandler;
    private readonly IReceiveEmailRepository _repository;
    private readonly ILogger<EmailsController> _logger;

    public EmailsController(
        ReceiveEmailHandler receiveEmailHandler,
        IReceiveEmailRepository repository,
        ILogger<EmailsController> logger)
    {
        _receiveEmailHandler = receiveEmailHandler;
        _repository = repository;
        _logger = logger;
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveEmails(CancellationToken cancellationToken)
    {
        _logger.LogInformation("接收到收信請求");

        var result = await _receiveEmailHandler.HandleAsync(cancellationToken);

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
}
