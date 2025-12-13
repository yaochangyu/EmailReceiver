using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.Data;
using EmailReceiver.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailReceiver.WebApi.Repositories;

public class ReceiveEmailRepository : IReceiveEmailRepository
{
    private readonly EmailReceiverDbContext _context;

    public ReceiveEmailRepository(EmailReceiverDbContext context)
    {
        _context = context;
    }

    public async Task<Result<EmailMessage>> AddAsync(EmailMessage emailMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.EmailMessages.AddAsync(emailMessage, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(emailMessage);
        }
        catch (Exception ex)
        {
            return Result.Failure<EmailMessage>($"儲存郵件時發生錯誤: {ex.Message}");
        }
    }

    public async Task<Result> AddRangeAsync(IEnumerable<EmailMessage> emailMessages,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.EmailMessages.AddRangeAsync(emailMessages, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"儲存多筆郵件時發生錯誤: {ex.Message}");
        }
    }

    public async Task<Result<IReadOnlyList<EmailMessage>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var messages = await _context.EmailMessages
                .OrderByDescending(e => e.ReceivedAt)
                .ToListAsync(cancellationToken);
            return Result.Success<IReadOnlyList<EmailMessage>>(messages);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<EmailMessage>>($"取得郵件清單時發生錯誤: {ex.Message}");
        }
    }

    public async Task<Result<IReadOnlyList<string>>> GetAllUidlsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var uidls = await _context.EmailMessages
                .Select(e => e.Uidl)
                .ToListAsync(cancellationToken);
            return Result.Success<IReadOnlyList<string>>(uidls);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<string>>($"取得所有 UIDL 時發生錯誤: {ex.Message}");
        }
    }
}
