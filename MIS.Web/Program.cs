using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging.EventLog;
using MIS.Web.Services;
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
builder.Services.AddHttpClient<IReportService, ReportService>();
builder.Services.AddScoped<IDiscrepancyReportService, DiscrepancyReportService>();
builder.Services.AddScoped<IComprehensiveReportService, ComprehensiveReportService>();
builder.Services.AddScoped<IVarientPerfomanceReportService, VarientPerfomanceReportService>();
builder.Services.AddScoped<IHourlyTrafficReportService, HourlyTrafficReportService>();
builder.Services.AddScoped<IDailyTrafficReportService, DailyTrafficReportService>();
builder.Services.AddScoped<IMonthlyTrafficReportService, MonthlyTrafficReportService>();
builder.Services.AddHttpClient<MonthlyTrafficReportService>();

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
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LandingPage}/{action=Index}");

app.Run($"http://{host}:{port}");
