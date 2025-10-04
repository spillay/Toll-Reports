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


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();

app.MapControllerRoute(
    name: "Transaction",
    pattern: "{controller=Transaction}/{action=Index}/");

app.Run();
