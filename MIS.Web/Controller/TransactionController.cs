using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    public class TransactionController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IConfiguration _config;

        public TransactionController(IReportService reportService, IConfiguration config)
        {
            _reportService = reportService;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Transaction(TransactionInputModel model)
        {
            try
            {
                // === DEFAULT FILTERS ===
                if (model.StartDate == default)
                    model.StartDate = DateTime.Today.AddDays(-30).Date.AddHours(5).AddMinutes(30);

                if (model.EndDate == default)
                    model.EndDate = DateTime.Today.Date.AddHours(5).AddMinutes(29);

                model.page = model.page <= 0 ? 1 : model.page;
                model.pageSize = model.pageSize <= 0 ? 50 : model.pageSize;

                // === API CALL FOR DATA ===
                var data = await _reportService.GetTransactionDetailsAsync(model);
                var filters = await _reportService.GetTransactionFilterOptionsAsync(model);

                // === Binder for dropdowns ===
                model.Shifts = filters.Shifts;
                model.TollOperators = filters.TollOperators;
                model.Lanes = filters.Lanes;
                model.PaymentMethods = filters.PaymentMethods;

                // === Assign main dataset ===
                model.items = data.items ?? new List<TransactionModel>();
                model.totalCount = data.totalCount;
                model.page = data.page;
                model.pageSize = data.pageSize;
                model.totalPages = data.totalPages;

                // === Compute Total Tariff ===
                model.TotalTariff = (double)(model.items?.Sum(x => (decimal?)x.tariff ?? 0) ?? 0);

                // === IMPORTANT: BUILD EXPORT URL FOR THIS VIEW ===
                string baseUrl = _config["BaseApiUrl:Link"];
                string endpoint = _config["ApiSettings:TransactionEndpoint"];
                ViewData["TransactionApi"] = $"{baseUrl}{endpoint}";

                return View("~/Views/Transaction/Index.cshtml", model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR in TransactionController: " + ex.Message);
                return View("~/Views/Transaction/Index.cshtml", new TransactionInputModel
                {
                    items = new List<TransactionModel>()
                });
            }
        }
    }
}
