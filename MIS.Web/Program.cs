
using Microsoft.AspNetCore.Authentication.Cookies;
using MIS.Web.Services;
using MIS.Web.Services.Interfaces;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var configServer = configBuilder.GetSection("Server");
var port = configServer["Port"];
var host = configServer["Host"];

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddHostedService<LoggerService>();

builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Register API services
builder.Services.AddHttpClient<ITransactionService, TransactionService>();
builder.Services.AddHttpClient<IDiscrepancyReportService, DiscrepancyReportService>();
builder.Services.AddScoped<IComprehensiveReportService, ComprehensiveReportService>();
builder.Services.AddScoped<IVarientPerfomanceReportService, VarientPerfomanceReportService>();
builder.Services.AddScoped<IHourlyTrafficReportService, HourlyTrafficReportService>();
builder.Services.AddScoped<IDailyTrafficReportService, DailyTrafficReportService>();
builder.Services.AddScoped<IMonthlyTrafficReportService, MonthlyTrafficReportService>();
builder.Services.AddHttpClient<MonthlyTrafficReportService>();
builder.Services.AddHttpClient<IDailyCashupReportService, DailyCashupReportService>();
builder.Services.AddHttpClient<ITopUpReportService, TopUpReportService>();
builder.Services.AddHttpClient<IAccountHistoryService, AccountHistoryService>();
builder.Services.AddScoped<IAccountUsageSummaryService, AccountUsageSummaryService>();
builder.Services.AddHttpClient<IAccountUsageDetailsService, AccountUsageDetailsService>();
builder.Services.AddHttpClient<IEndOfDayReportService, EndOfDayReportService>();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login";
        options.AccessDeniedPath = "/Login/Login";

        options.Cookie.Name = "MIS.Reports.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // change to Always when HTTPS

        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpClient();

builder.Services.AddWindowsService();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection(); // Enable this if you want HTTPS
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LandingPage}/{action=Index}");

app.Run($"http://{host}:{port}");
