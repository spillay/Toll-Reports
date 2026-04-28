using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Toll.Reporting.Api.Repositories;
using Toll.Reporting.Api.Repositories.Implementations;
using Toll.Reporting.Api.Repositories.Interfaces;
using TollReportingSystem.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

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
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Toll Reporting API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ====================
// Configuration
// ====================
var connectionString =
    builder.Configuration.GetConnectionString("SQLServerConnection")
    ?? throw new InvalidOperationException("Connection string 'SQLServerConnection' was not found.");

var host = builder.Configuration["Server:Host"] ?? "localhost";
var port = builder.Configuration["Server:Port"] ?? "4567";
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

var jwtKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JWT SecretKey is missing in configuration.");

var jwtIssuer = builder.Configuration["JwtSettings:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer is missing in configuration.");

var jwtAudience = builder.Configuration["JwtSettings:Audience"]
    ?? throw new InvalidOperationException("JWT Audience is missing in configuration.");

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
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});

// ====================
// Authentication - JWT
// ====================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // set true in production with HTTPS
    options.SaveToken = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,

        ValidateAudience = true,
        ValidAudience = jwtAudience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

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
builder.Services.AddScoped<IAvcAccuracyRepository, AvcAccuracyRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ====================
// Run
// ====================
app.Run($"http://{host}:{port}");
