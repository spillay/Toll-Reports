using Microsoft.EntityFrameworkCore;
using Toll.Reporting.Api.Repositories;
using Toll.Reporting.Api.Repositories.Implementations;
using Toll.Reporting.Api.Repositories.Interfaces;
using TollReportingSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// ====================
// Windows Service
// ====================
builder.Services.AddWindowsService();

// ====================
// Controllers + JSON
// ====================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Keep PascalCase for MVC frontend
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ====================
// Configuration
// ====================
var connectionString =
    builder.Configuration.GetConnectionString("SQLServerConnection")
    ?? throw new InvalidOperationException("Connection string 'SQLServerConnection' was not found.");

var host = builder.Configuration["Server:Host"] ?? "localhost";
var port = builder.Configuration["Server:Port"] ?? "4567";

// ====================
// DbContext
// ====================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sql =>
    {
        sql.CommandTimeout(120);
    }));

// ====================
// CORS
// ====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:8081")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ====================
// Dependency Injection
// ====================
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IComprehensiveRepository, ComprehensiveRepository>();
builder.Services.AddScoped<IDiscrepancyRepository, DiscrepancyRepository>();
builder.Services.AddScoped<IVarientPerformanceRepository, VarientPerformanceRepository>();
builder.Services.AddScoped<IHourlyTrafficRepository, HourlyTrafficRepository>();
builder.Services.AddScoped<IDailyTrafficRepository, DailyTrafficRepository>();
builder.Services.AddScoped<IMonthlyTrafficRepository, MonthlyTrafficRepository>();
builder.Services.AddScoped<IDailyCashupRepository, DailyCashupRepository>();
builder.Services.AddScoped<ITopUpRepository, TopUpRepository>();
builder.Services.AddScoped<IAccountHistoryRepository, AccountHistoryRepository>();
builder.Services.AddScoped<IAccountUsageSummaryRepository, AccountUsageSummaryRepository>();
builder.Services.AddScoped<IAccountUsageDetailsRepository, AccountUsageDetailsRepository>();
builder.Services.AddScoped<IEndOfDayReportRepository, EndOfDayReportRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

builder.Services.AddHttpClient();

// ====================
// Build App
// ====================
var app = builder.Build();

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

// ====================
// Run
// ====================
app.Run($"http://{host}:{port}");