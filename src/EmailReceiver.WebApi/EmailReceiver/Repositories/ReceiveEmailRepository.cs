using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.EmailReceiver.Data;
using EmailReceiver.WebApi.EmailReceiver.Data.Entities;
using EmailReceiver.WebApi.EmailReceiver.Models;
using Microsoft.EntityFrameworkCore;

namespace EmailReceiver.WebApi.EmailReceiver.Repositories;

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

    public async Task<Result<int>> AddAsync(InsertEmailRequest request, CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // 1. 建立 Letter 實體
            var letter = Letter.Create(
                sender: request.SenderName,
                sEmail: request.SenderEmail,
                sSubject: request.Subject,
                sQuestion: request.Body,
                sDate: request.MailDate);

            // 需要先新增 Letter，以取得生成的 LNo
            await _context.Letters.AddAsync(letter, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // 2. 建立 MailReplay 實體，關聯至新建立的 Letter
            var mailReplay = MailReplay.Create(
                mailFrom: request.SenderEmail,
                mailFromName: request.SenderName,
                mailSubject: request.Subject,
                mailBody: request.Body,
                mailDate: request.MailDate,
                lNo: letter.LNo,
                mailType: request.Circumstance,
                tracker: request.Tracker,
                mailAttach: request.Attachment ?? string.Empty,
                mailAttachName: request.AttachmentName ?? string.Empty,
                mailAttachSize: request.AttachmentSize ?? "0");

            await _context.MailReplays.AddAsync(mailReplay, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            
            return Result.Success(letter.LNo);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<int>($"儲存郵件至 letters 和 mailReplay 時發生錯誤: {ex.Message}");
        }
    }
}
