using Microsoft.AspNetCore.Mvc;

namespace JobBank1111.Job.WebAPI;

[ApiController]
public class TestController : ControllerBase
{
    [HttpGet]
    [Route("api/v1/tests")]
    public async Task<ActionResult> GetTests(
        [FromQuery] string? userId = null,
        [FromQuery] string? description = null,
        CancellationToken cancel = default)
    {
        var response = new
        {
            userId = userId,
            description = description
        };
        return this.Ok(response);
    }
}