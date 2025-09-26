using Toll.Reporting.Api.Controllers;
using Microsoft.EntityFrameworkCore;
using TollReportingSystem.Data; // <-- Your EF DbContext (Database First)
using Toll.Reporting.Api.Repositories; // <-- Your repositories interfaces & implementations



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DbContext with SQL Server
// Connection string comes from appsettings.json

var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var configSection = configBuilder.GetSection("ConnectionStrings");
var connectionString = configSection["SQLServerConnection"] ?? null;


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// Register repositories with Dependency Injection
// Scoped = one instance per request (recommended for DbContext)
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IComprehensiveRepository, ComprehensiveRepository>();
builder.Services.AddScoped<IDiscrepancyRepository, DiscrepancyRepository>();

// Program.cs - inside builder.Services section
//builder.Services.Configure<SSRSOptions>(builder.Configuration.GetSection("SSRS"));
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
