using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.EmailReceiver.Data;
using EmailReceiver.WebApi.EmailReceiver.Data.Entities;
using EmailReceiver.WebApi.EmailReceiver.Models;
using EmailReceiver.WebApi.Infrastructure.ErrorHandling;
using Microsoft.EntityFrameworkCore;

namespace EmailReceiver.WebApi.EmailReceiver.Repositories;

public class ReceiveEmailRepository : IReceiveEmailRepository
{
    private readonly EmailReceiverDbContext _context;

    public ReceiveEmailRepository(EmailReceiverDbContext context)
    {
        _context = context;
    }

    public async Task<Result<IReadOnlyList<string>, Failure>> GetAllUidlsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var uidls = await _context.MailReplays.Select(e => e.MailAttachName)
                .ToListAsync(cancellationToken);
            return Result.Success<IReadOnlyList<string>, Failure>(uidls);
        }
        catch (Exception ex)
        {
            return Result.Failure<IReadOnlyList<string>, Failure>(
                Failure.DbError("取得所有 UIDL 時發生錯誤", ex));
        }
    }

    public async Task<Result<int, Failure>> AddAsync(InsertEmailRequest request,
        CancellationToken cancellationToken = default)
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
                sDate: request.MailDate,
                towhom: request.ToWhom,
                circumstance: request.Circumstance);

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

            return Result.Success<int, Failure>(letter.LNo);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<int, Failure>(
                Failure.DbError("儲存郵件至 letters 和 mailReplay 時發生錯誤", ex));
        }
    }
}