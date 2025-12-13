using EmailReceiver.WebApi.EmailReceiver;
using EmailReceiver.WebApi.EmailReceiver.Adpaters;
using EmailReceiver.WebApi.EmailReceiver.Data;
using EmailReceiver.WebApi.EmailReceiver.Options;
using EmailReceiver.WebApi.EmailReceiver.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// 註冊 DbContext
builder.Services.AddDbContext<EmailReceiverDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 註冊 Options
builder.Services.Configure<Pop3Options>(builder.Configuration.GetSection(Pop3Options.SectionName));

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
