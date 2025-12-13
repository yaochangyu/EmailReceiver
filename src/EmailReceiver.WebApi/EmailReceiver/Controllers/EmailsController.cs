using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.EmailReceiver.Models.Responses;
using EmailReceiver.WebApi.Infrastructure.ErrorHandling;
using EmailReceiver.WebApi.Infrastructure.TraceContext;
using Microsoft.AspNetCore.Mvc;

namespace EmailReceiver.WebApi.EmailReceiver.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmailsController : ControllerBase
{
    private readonly ReceiveEmailHandler _receiveEmailHandler;
    private readonly IContextGetter<TraceContext> _contextGetter;

    public EmailsController(
        ReceiveEmailHandler receiveEmailHandler,
        IContextGetter<TraceContext> contextGetter)
    {
        _receiveEmailHandler = receiveEmailHandler;
        _contextGetter = contextGetter;
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveEmails(CancellationToken cancellationToken)
    {
        var result = await _receiveEmailHandler.HandleAsync(cancellationToken);

        if (result.IsFailure)
        {
            var traceContext = _contextGetter.Get();
            var failure = result.Error with { TraceId = traceContext?.TraceId };
            return failure.ToActionResult();
        }

        var response = new ReceiveEmailsResponse(
            SavedCount: result.Value,
            Message: $"成功接收並儲存 {result.Value} 封郵件"
        );

        return Ok(response);
    }
}
