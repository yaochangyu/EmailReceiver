using EmailReceiver.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmailReceiver.WebApi.Data;

public class EmailReceiverDbContext : DbContext
{
    public EmailReceiverDbContext(DbContextOptions<EmailReceiverDbContext> options)
        : base(options)
    {
    }

    public DbSet<EmailMessage> EmailMessages { get; set; } = null!;

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
    }
}
