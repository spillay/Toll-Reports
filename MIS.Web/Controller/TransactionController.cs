using Microsoft.AspNetCore.Mvc;
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

        public TransactionController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> Transaction(TransactionInputModel model)
        {
            try
            {
                /* ============================================
                   1. DEFAULT FILTERS (IF USER ENTERED NONE)
                =============================================*/
                if (model.StartDate == default)
                    model.StartDate = DateTime.Today.AddDays(-30).Date.AddHours(5).AddMinutes(30);

                if (model.EndDate == default)
                    model.EndDate = DateTime.Today.Date.AddHours(5).AddMinutes(29);

                model.page = model.page <= 0 ? 1 : model.page;
                model.pageSize = model.pageSize <= 0 ? 50 : model.pageSize;

                /* ============================================
                   2. LOAD PAGINATED DATA FOR TABLE
                =============================================*/
                var data = await _reportService.GetTransactionDetailsAsync(model);

                model.items = data.items ?? new List<TransactionModel>();
                model.totalCount = data.totalCount;
                model.page = data.page;
                model.pageSize = data.pageSize;
                model.totalPages = data.totalPages;


                /* ============================================
                   3. LOAD FULL UNPAGINATED DATA FOR EXPORT
                =============================================*/
                var exportModel = new TransactionInputModel
                {
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Shift = model.Shift,
                    TollOperatorID = model.TollOperatorID,
                    lane_Nr = model.lane_Nr,
                    PaymentMethod = model.PaymentMethod,
                    page = 1,
                    pageSize = 999999, // All data
                    ExportAll = true
                };

                var exportData = await _reportService.GetTransactionExportAsync(exportModel);
                model.ExportItems = exportData.items ?? new List<TransactionModel>();


                /* ============================================
                   4. LOAD FILTER OPTIONS FOR DROPDOWNS
                =============================================*/
                var filters = await _reportService.GetTransactionFilterOptionsAsync(model);

                model.Shifts = filters.Shifts;
                model.TollOperators = filters.TollOperators;
                model.Lanes = filters.Lanes;
                model.PaymentMethods = filters.PaymentMethods;


                /* ============================================
                   5. COMPUTE TOTALS
                =============================================*/
                model.TotalTariff = (double)(model.items?.Sum(x => (decimal?)x.tariff ?? 0) ?? 0);


                /* ============================================
                   6. RETURN VIEW
                =============================================*/
                return View("~/Views/Transaction/Index.cshtml", model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR in TransactionController: " + ex.Message);

                return View("~/Views/Transaction/Index.cshtml", new TransactionInputModel
                {
                    items = new List<TransactionModel>(),
                    ExportItems = new List<TransactionModel>()
                });
            }
        }
    }
}
