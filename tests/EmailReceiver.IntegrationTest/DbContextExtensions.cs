using System.Data;
using EmailReceiver.WebApi.EmailReceiver.Data;
using JobBank1111.Testing.Common;
using Microsoft.EntityFrameworkCore;

namespace EmailReceiver.IntegrationTest;

public static class DbContextExtensions
{
    public static void ClearAllData(this EmailReceiverDbContext dbContext)
    {
        SqlServerGenerateScript.OnlySupportLocal(dbContext.Database.GetConnectionString());
        dbContext.Database.ExecuteSqlRaw(SqlServerGenerateScript.ClearAllRecord());
    }

    public static async Task Initial(this EmailReceiverDbContext dbContext)
    {
        SqlServerGenerateScript.OnlySupportLocal(dbContext.Database.GetConnectionString());
        // dbContext.Database.EnsureDeleted();

        var migrations = dbContext.Database.GetMigrations();
        if (migrations != null && migrations.Any())
        {
            dbContext.Database.Migrate();
        }
        else
        {
            dbContext.Database.EnsureCreated();
        }

        await dbContext.Seed();
    }

    public static async Task Seed(this EmailReceiverDbContext dbContext)
    {
        SqlServerGenerateScript.OnlySupportLocal(dbContext.Database.GetConnectionString());

        var dbConnection = dbContext.Database.GetDbConnection();
        if (dbConnection.State != ConnectionState.Open)
        {
            await dbConnection.OpenAsync();
        }

        //讀取資料夾的所有 sql 檔案，並執行
        var sqlFiles = Directory.GetFiles("DB/Scripts", "*.sql");

        foreach (var sqlFile in sqlFiles)
        {
            var sql = await File.ReadAllTextAsync(sqlFile);
            
            // 使用 Regex 分割 GO，支援各種換行格式
            var batches = System.Text.RegularExpressions.Regex.Split(
                sql, 
                @"^\s*GO\s*$", 
                System.Text.RegularExpressions.RegexOptions.Multiline | 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            foreach (var batch in batches)
            {
                var cleanBatch = batch.Trim();
                
                // 過濾不適合在 ADO.NET 中執行的指令
                if (string.IsNullOrWhiteSpace(cleanBatch) ||
                    cleanBatch.StartsWith("CREATE DATABASE", StringComparison.OrdinalIgnoreCase) ||
                    cleanBatch.StartsWith("USE ", StringComparison.OrdinalIgnoreCase) ||
                    cleanBatch.StartsWith("USE[", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                await using var cmd = dbConnection.CreateCommand();
                cmd.CommandText = cleanBatch;
                await cmd.ExecuteNonQueryAsync();
            }
        }
    }
}