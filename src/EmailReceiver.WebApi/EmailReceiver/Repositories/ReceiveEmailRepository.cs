using CSharpFunctionalExtensions;
using EmailReceiver.WebApi.EmailReceiver.Data;
using EmailReceiver.WebApi.EmailReceiver.Data.Entities;
using EmailReceiver.WebApi.EmailReceiver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;

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

    public async Task<Result<int>> AddAsync(InsertEmailRequest model, CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            var dbTransaction = transaction.GetDbTransaction() as DbTransaction;

            // 1. 插入 letters 資料表
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO letters(sender, s_email, towhom, s_date, s_subject, s_question, circumstance, ok)
                VALUES(@sender, @s_email, @towhom, @s_date, @s_subject, @s_question, @circumstance, 2);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            
            command.Transaction = dbTransaction;
            
            command.Parameters.Add(CreateParameter(command, "@sender", model.SenderName));
            command.Parameters.Add(CreateParameter(command, "@s_email", model.SenderEmail));
            command.Parameters.Add(CreateParameter(command, "@towhom", model.ToWhom));
            command.Parameters.Add(CreateParameter(command, "@s_date", model.MailDate));
            command.Parameters.Add(CreateParameter(command, "@s_subject", model.Subject));
            command.Parameters.Add(CreateParameter(command, "@s_question", model.Body));
            command.Parameters.Add(CreateParameter(command, "@circumstance", model.Circumstance));

            var lNoResult = await command.ExecuteScalarAsync(cancellationToken);
            var letterLNo = Convert.ToInt32(lNoResult);

            // 2. 插入 mailReplay 資料表
            using var command2 = connection.CreateCommand();
            command2.CommandText = @"
                INSERT INTO mailReplay(mailFrom, mailFromName, mailSubject, mailDate, mailBody, mailType, tracker, lNo, mailAttach, mailAttachName, mailAttachSize)
                VALUES(@mailFrom, @mailFromName, @mailSubject, @mailDate, @mailBody, @mailType, @tracker, @lNo, @mailAttach, @mailAttachName, @mailAttachSize);";
            
            command2.Transaction = dbTransaction;
            
            command2.Parameters.Add(CreateParameter(command2, "@mailFrom", model.SenderEmail));
            command2.Parameters.Add(CreateParameter(command2, "@mailFromName", model.SenderName));
            command2.Parameters.Add(CreateParameter(command2, "@mailSubject", model.Subject));
            command2.Parameters.Add(CreateParameter(command2, "@mailDate", model.MailDate));
            command2.Parameters.Add(CreateParameter(command2, "@mailBody", model.Body));
            command2.Parameters.Add(CreateParameter(command2, "@mailType", model.Circumstance));
            command2.Parameters.Add(CreateParameter(command2, "@tracker", model.Tracker));
            command2.Parameters.Add(CreateParameter(command2, "@lNo", letterLNo));
            command2.Parameters.Add(CreateParameter(command2, "@mailAttach", model.Attachment ?? string.Empty));
            command2.Parameters.Add(CreateParameter(command2, "@mailAttachName", model.AttachmentName ?? string.Empty));
            command2.Parameters.Add(CreateParameter(command2, "@mailAttachSize", model.AttachmentSize ?? "0"));

            await command2.ExecuteNonQueryAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
            
            return Result.Success(letterLNo);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return Result.Failure<int>($"儲存郵件至 letters 和 mailReplay 時發生錯誤: {ex.Message}");
        }
    }

    private static IDbDataParameter CreateParameter(IDbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        return parameter;
    }
}
