using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MIS.Web.Models;
using MIS.Web.Models.Transaction;
using MIS.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MIS.Web.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private readonly ITransactionService _reportService;

        public TransactionController(ITransactionService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> Transaction(TransactionInputModel model)
        {
            try
            {
                ApplyDefaults(model);
                NormalizeSelections(model);

                // 1) Load filter options (ALL values in system)
                await PopulateFilterOptions(model);

                // 2) Load paged table data
                await PopulatePagedData(model);

                // 3) Load full dataset for export (same filters)
                await PopulateExportData(model);

                // 4) Totals based on FULL filtered set (export items)
                model.TotalTariff = (double)model.ExportItems.Sum(x => x.tariff ?? 0m);

                return View("~/Views/Transaction/Index.cshtml", model);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ ERROR in TransactionController: " + ex.Message);
                return View("~/Views/Transaction/Index.cshtml", CreateEmptyModel());
            }
        }

        private static void ApplyDefaults(TransactionInputModel model)
        {
            // Operational day defaults 
            if (model.StartDate == default)
                model.StartDate = DateTime.Today.AddDays(-30).Date.AddHours(5).AddMinutes(30);

            if (model.EndDate == default)
                model.EndDate = DateTime.Today.Date.AddHours(5).AddMinutes(29);

            if (model.EndDate < model.StartDate)
                model.EndDate = model.StartDate.AddHours(1);

            model.page = model.page <= 0 ? 1 : model.page;
            model.pageSize = model.pageSize <= 0 ? 50 : model.pageSize;
        }

        private static void NormalizeSelections(TransactionInputModel model)
        {
            model.SelectedShifts = NormalizeList(model.SelectedShifts);
            model.SelectedTollOperators = NormalizeList(model.SelectedTollOperators);
            model.SelectedLanes = NormalizeList(model.SelectedLanes);
            model.SelectedPaymentMethods = NormalizeList(model.SelectedPaymentMethods);
        }

        private static List<string> NormalizeList(List<string>? list)
        {
            return (list ?? new List<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private async Task PopulateFilterOptions(TransactionInputModel model)
        {
            var filters = await _reportService.GetTransactionFilterOptionsAsync(model);

            model.Shifts = filters.Shifts ?? new List<string>();
            model.TollOperators = filters.TollOperators ?? new List<string>();
            model.Lanes = filters.Lanes ?? new List<string>();
            model.PaymentMethods = filters.PaymentMethods ?? new List<string>();
        }

        private async Task PopulatePagedData(TransactionInputModel model)
        {
            var paged = await _reportService.GetTransactionDetailsAsync(model);

            model.items = paged.items ?? new List<TransactionModel>();
            model.totalCount = paged.totalCount;
            model.page = paged.page;
            model.pageSize = paged.pageSize;
            model.totalPages = paged.totalPages;
        }

        private async Task PopulateExportData(TransactionInputModel model)
        {
            var exportModel = new TransactionInputModel
            {
                StartDate = model.StartDate,
                EndDate = model.EndDate,

                SelectedShifts = model.SelectedShifts,
                SelectedTollOperators = model.SelectedTollOperators,
                SelectedLanes = model.SelectedLanes,
                SelectedPaymentMethods = model.SelectedPaymentMethods,

                ExportAll = true,
                page = 1,
                pageSize = int.MaxValue
            };

            var export = await _reportService.GetTransactionExportAsync(exportModel);
            model.ExportItems = export.items ?? new List<TransactionModel>();
        }

        private static TransactionInputModel CreateEmptyModel()
        {
            return new TransactionInputModel
            {
                items = new List<TransactionModel>(),
                ExportItems = new List<TransactionModel>(),
                Shifts = new List<string>(),
                TollOperators = new List<string>(),
                Lanes = new List<string>(),
                PaymentMethods = new List<string>(),
                SelectedShifts = new List<string>(),
                SelectedTollOperators = new List<string>(),
                SelectedLanes = new List<string>(),
                SelectedPaymentMethods = new List<string>()
            };
        }
    }
}