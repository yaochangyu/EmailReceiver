using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.Data;
using EmailReceiver.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailReceiver.WebApi.Repositories;

public class EmailMessageRepository : IEmailMessageRepository
{
    private readonly EmailReceiverDbContext _context;

    public EmailMessageRepository(EmailReceiverDbContext context)
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

    public async Task<Result<bool>> ExistsByUidlAsync(string uidl, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = await _context.EmailMessages
                .AnyAsync(e => e.Uidl == uidl, cancellationToken);
            return Result.Success(exists);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool>($"檢查郵件是否存在時發生錯誤: {ex.Message}");
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
}
