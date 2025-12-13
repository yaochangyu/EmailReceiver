using EmailReceiver.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailReceiver.WebApi.Data;

/// <summary>
/// EmailReceiver 資料庫上下文
/// </summary>
public class EmailReceiverDbContext : DbContext
{
    public EmailReceiverDbContext(DbContextOptions<EmailReceiverDbContext> options)
        : base(options)
    {
    }

    /// <summary>郵件訊息資料表 (新版)</summary>
    public DbSet<EmailMessage> EmailMessages { get; set; } = null!;
    
    /// <summary>來信管理主表 (舊版)</summary>
    public DbSet<Letter> Letters { get; set; } = null!;
    
    /// <summary>郵件回覆管理表 (舊版)</summary>
    public DbSet<MailReplay> MailReplays { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<EmailMessage>(entity =>
        {
            entity.ToTable("EmailMessages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Uidl).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Subject).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Body).IsRequired();
            entity.Property(e => e.From).IsRequired().HasMaxLength(500);
            entity.Property(e => e.To).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ReceivedAt).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.Uidl).IsUnique();
            entity.HasIndex(e => e.ReceivedAt);
        });

        modelBuilder.Entity<Letter>(entity =>
        {
            entity.ToTable("letters");
            entity.HasKey(e => e.LNo);
            entity.Property(e => e.LNo).HasColumnName("lNo").ValueGeneratedOnAdd();
            entity.Property(e => e.Rowguid).HasColumnName("rowguid");
            entity.Property(e => e.Sender).HasColumnName("sender").HasMaxLength(100);
            entity.Property(e => e.SEmail).HasColumnName("s_email").HasMaxLength(100);
            entity.Property(e => e.Telephone).HasColumnName("telephone").HasMaxLength(50);
            entity.Property(e => e.Towhom).HasColumnName("towhom").HasMaxLength(60);
            entity.Property(e => e.SDate).HasColumnName("s_date").HasColumnType("smalldatetime").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.SSubject).HasColumnName("s_subject").HasMaxLength(300);
            entity.Property(e => e.SQuestion).HasColumnName("s_question").HasColumnType("ntext");
            entity.Property(e => e.SFile1).HasColumnName("s_file1").HasMaxLength(50);
            entity.Property(e => e.SFile2).HasColumnName("s_file2").HasMaxLength(50);
            entity.Property(e => e.SFile3).HasColumnName("s_file3").HasMaxLength(50);
            entity.Property(e => e.SFile4).HasColumnName("s_file4").HasMaxLength(50);
            entity.Property(e => e.SFile5).HasColumnName("s_file5").HasMaxLength(50);
            entity.Property(e => e.Handle).HasColumnName("handle").HasMaxLength(200);
            entity.Property(e => e.Transactor).HasColumnName("transactor").HasMaxLength(15);
            entity.Property(e => e.Reply).HasColumnName("reply").HasColumnType("ntext");
            entity.Property(e => e.Circumstance).HasColumnName("circumstance").HasMaxLength(300);
            entity.Property(e => e.Datakind).HasColumnName("datakind").HasMaxLength(20);
            entity.Property(e => e.Track).HasColumnName("track").HasMaxLength(4);
            entity.Property(e => e.AskReply).HasColumnName("ask_reply").HasMaxLength(4);
            entity.Property(e => e.SenderReply).HasColumnName("sender_reply").HasMaxLength(3000);
            entity.Property(e => e.Memo).HasColumnName("memo").HasMaxLength(3000);
            entity.Property(e => e.RFile1).HasColumnName("r_file1").HasMaxLength(300);
            entity.Property(e => e.RFile2).HasColumnName("r_file2").HasMaxLength(300);
            entity.Property(e => e.RDate).HasColumnName("r_date").HasColumnType("datetime");
            entity.Property(e => e.Stand).HasColumnName("stand");
            entity.Property(e => e.Ok).HasColumnName("ok").HasDefaultValue((byte)0);
            entity.Property(e => e.Ip).HasColumnName("ip").HasMaxLength(50);
            entity.Property(e => e.Mno).HasColumnName("mno");
            entity.Property(e => e.Tables).HasColumnName("tables").HasMaxLength(20);
            entity.Property(e => e.Delimit).HasColumnName("delimit").HasMaxLength(50);
            entity.Property(e => e.ServerName).HasColumnName("ServerName").HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Rowguid37).HasColumnName("rowguid37").HasDefaultValueSql("NEWID()");
            entity.Property(e => e.Browser).HasColumnName("browser").HasMaxLength(100).IsUnicode(false);
            entity.Property(e => e.Agent).HasColumnName("agent").HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.Reply1).HasColumnName("reply1").HasColumnType("ntext");
            entity.Property(e => e.DateReply1).HasColumnName("dateReply1").HasColumnType("datetime");
            entity.Property(e => e.Assignto).HasColumnName("assignto").HasMaxLength(50);
            entity.Property(e => e.Assigndate).HasColumnName("assigndate").HasColumnType("datetime");
            entity.Property(e => e.IdNumber).HasColumnName("idNumber").HasMaxLength(50).IsUnicode(false);
            entity.Property(e => e.SBirth).HasColumnName("s_birth").HasMaxLength(20);
        });

        modelBuilder.Entity<MailReplay>(entity =>
        {
            entity.ToTable("mailReplay");
            entity.HasKey(e => e.MNo);
            entity.Property(e => e.MNo).HasColumnName("mNo").ValueGeneratedOnAdd();
            entity.Property(e => e.MailFrom).HasColumnName("mailFrom").HasMaxLength(200).IsRequired().HasDefaultValue("");
            entity.Property(e => e.MailFromName).HasColumnName("mailFromName").HasMaxLength(100).IsRequired().HasDefaultValue("");
            entity.Property(e => e.MailSubject).HasColumnName("mailSubject").HasMaxLength(200).IsRequired().HasDefaultValue("");
            entity.Property(e => e.MailDate).HasColumnName("mailDate").HasColumnType("smalldatetime").IsRequired().HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.MailBody).HasColumnName("mailBody").HasColumnType("ntext").IsRequired().HasDefaultValue("");
            entity.Property(e => e.MailType).HasColumnName("mailType").HasMaxLength(50).IsRequired().HasDefaultValue("0");
            entity.Property(e => e.Status).HasColumnName("status").IsRequired().HasDefaultValue((byte)1);
            entity.Property(e => e.Tracker).HasColumnName("tracker").HasMaxLength(50).IsRequired().HasDefaultValue("");
            entity.Property(e => e.DateIn).HasColumnName("dateIn").HasColumnType("smalldatetime").IsRequired().HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.LNo).HasColumnName("lNo").IsRequired().HasDefaultValue(0);
            entity.Property(e => e.Reply).HasColumnName("reply").HasColumnType("ntext").IsRequired().HasDefaultValue("");
            entity.Property(e => e.MailAttach).HasColumnName("mailAttach").HasMaxLength(4000).IsUnicode(false).IsRequired().HasDefaultValue("");
            entity.Property(e => e.MailAttachName).HasColumnName("mailAttachName").HasMaxLength(4000).IsRequired().HasDefaultValue("");
            entity.Property(e => e.MailAttachSize).HasColumnName("mailAttachSize").HasMaxLength(8000).IsUnicode(false).IsRequired().HasDefaultValue("0");
        });
    }
}
