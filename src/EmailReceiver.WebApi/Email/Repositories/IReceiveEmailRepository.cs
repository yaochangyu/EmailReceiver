using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.EmailReceiver.Models;
using EmailReceiver.WebApi.Infrastructure.ErrorHandling;

namespace EmailReceiver.WebApi.EmailReceiver.Repositories;

public interface IReceiveEmailRepository
{
    Task<Result<IReadOnlyList<string>, Failure>> GetAllUidlsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// 儲存郵件至 letters 和 mailReplay 資料表 (舊系統)
    /// </summary>
    /// <param name="request">郵件資料模型</param>
    /// <param name="cancellationToken">取消權杖</param>
    /// <returns>成功時返回 letters 表的 LNo</returns>
    Task<Result<int, Failure>> AddAsync(InsertEmailRequest request, CancellationToken cancellationToken = default);
}
