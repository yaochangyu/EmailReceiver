using Microsoft.AspNetCore.Mvc;

namespace EmailReceiver.WebApi.Infrastructure.ErrorHandling;

/// <summary>
/// 將錯誤代碼映射至 HTTP 狀態碼
/// </summary>
public static class FailureCodeMapper
{
    public static IActionResult ToActionResult(this Failure failure)
    {
        var statusCode = MapToHttpStatusCode(failure.Code);
        
        var response = new
        {
            failure.Code,
            failure.Message,
            failure.TraceId,
            failure.Data
        };

        return new ObjectResult(response)
        {
            StatusCode = statusCode
        };
    }

    private static int MapToHttpStatusCode(string code)
    {
        return code switch
        {
            nameof(FailureCode.Unauthorized) => 401,
            nameof(FailureCode.ValidationError) => 400,
            nameof(FailureCode.DuplicateEmail) => 409,
            nameof(FailureCode.Pop3ConnectionError) => 502,
            nameof(FailureCode.EmailReceiveError) => 500,
            nameof(FailureCode.DbError) => 500,
            nameof(FailureCode.DbConcurrency) => 409,
            nameof(FailureCode.InvalidOperation) => 400,
            nameof(FailureCode.Timeout) => 408,
            nameof(FailureCode.InternalServerError) => 500,
            _ => 500
        };
    }
}
