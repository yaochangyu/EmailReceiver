using EmailReceiver.WebApi.EmailReceiver.Data;
using Microsoft.AspNetCore.Mvc;

namespace EmailReceiver.WebApi.Health;

[ApiController]
[Route("api/v1")]
public class HealthController : ControllerBase
{
    private readonly EmailReceiverDbContext _dbContext;

    public HealthController(EmailReceiverDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("_health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow
        });
    }

    [HttpGet("_health/db")]
    public async Task<IActionResult> DatabaseHealth(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.Database.CanConnectAsync(cancellationToken);
            
            return Ok(new
            {
                Status = "Healthy",
                Database = "Connected",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new
            {
                Status = "Unhealthy",
                Database = "Disconnected",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
