using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MIS.Web.Services;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddHttpClient<IReportService, ReportService>();
builder.Services.AddScoped<IDiscrepancyReportService, DiscrepancyReportService>();
builder.Services.AddScoped<IComprehensiveReportService, ComprehensiveReportService>();
builder.Services.AddScoped<IVarientPerfomanceReportService, VarientPerfomanceReportService>();
builder.Services.AddScoped<IHourlyTrafficReportService, HourlyTrafficReportService>();
builder.Services.AddScoped<IDailyTrafficReportService, DailyTrafficReportService>();
builder.Services.AddScoped<IMonthlyTrafficReportService, MonthlyTrafficReportService>();



builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.MapControllerRoute(
    name: "LandingPage",
    pattern: "{controller=LandingPage}/{action=Index}/");
app.Run();
