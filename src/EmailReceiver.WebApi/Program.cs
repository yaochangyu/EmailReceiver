using EmailReceiver.WebApi;
using EmailReceiver.WebApi.EmailReceiver;
using EmailReceiver.WebApi.EmailReceiver.Adpaters;
using EmailReceiver.WebApi.EmailReceiver.Options;
using EmailReceiver.WebApi.EmailReceiver.Repositories;
using EmailReceiver.WebApi.Infrastructure;
using EmailReceiver.WebApi.Infrastructure.Middleware;
using EmailReceiver.WebApi.Infrastructure.TraceContext;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/host-.txt", rollingInterval: RollingInterval.Hour)
    .CreateLogger();
Log.Information("Starting web host");
if (Array.FindIndex(args, x => x == "--local") >= 0)
{
    var envFolder = EnvironmentUtility.FindParentFolder("env");
    EnvironmentUtility.ReadEnvironmentFile(envFolder, "local.env");
}

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddSysEnvironments();

// Add services to the container.
builder.Services.AddControllers();

// 註冊 DbContext
// builder.Services.AddDbContext<EmailReceiverDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabase();

// 註冊 Options
builder.Services.Configure<Pop3Options>(builder.Configuration.GetSection(Pop3Options.SectionName));

// 註冊 TraceContext 基礎設施
builder.Services.AddSingleton<TraceContextAccessor>();
builder.Services.AddSingleton<IContextGetter<TraceContext>>(sp => sp.GetRequiredService<TraceContextAccessor>());
builder.Services.AddSingleton<IContextSetter<TraceContext>>(sp => sp.GetRequiredService<TraceContextAccessor>());

// 註冊 Repositories
builder.Services.AddScoped<IReceiveEmailRepository, ReceiveEmailRepository>();

// 註冊 Services
builder.Services.AddScoped<IEmailReceiveAdapter, Pop3EmailReceiveAdapter>();

// 註冊 Handlers
builder.Services.AddScoped<ReceiveEmailHandler>();

// 加入 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

// Middleware 管線順序（由外到內）
// 1. 例外處理 - 最外層，捕捉所有未處理例外
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 2. 追蹤內容 - 設定 TraceId 與 TraceContext
app.UseMiddleware<TraceContextMiddleware>();

// 3. 請求日誌 - 記錄請求與回應資訊
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// 讓 Program 類別對測試專案可見
namespace EmailReceiver.WebApi
{
    public partial class Program
    {
    }
}