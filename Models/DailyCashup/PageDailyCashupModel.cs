using System;
using System.Collections.Generic;
using MIS.Web.Models;

namespace MIS.Web.Models.DailyCashup
{
    public class CheckItemModel<T>
    {
        public T Id { get; set; } = default!;
        public string Name { get; set; } = string.Empty;
        public bool Selected { get; set; }
    }

    public class DailyCashupShiftTotalModel
    {
        public string ShiftDescription { get; set; } = string.Empty;

        public decimal NettAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal TotalCashExpected { get; set; }
        public decimal TotalDeclared { get; set; }
        public decimal Difference { get; set; }
        public decimal TotalBanked { get; set; }

        public decimal SurplusShortage => Difference;
    }

    public class DailyCashupGrandTotalModel
    {
        public decimal NettAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal TotalCashExpected { get; set; }
        public decimal TotalDeclared { get; set; }
        public decimal Difference { get; set; }
        public decimal TotalBanked { get; set; }

        public decimal SurplusShortage => Difference;
    }

    public class PageDailyCashupModel : MIS.Web.Models.PageModel
    {
        // Current page rows
        public List<DailyCashupModel> Items { get; set; } = new();

        // All rows for export / full report totals
        public List<DailyCashupModel> FullItems { get; set; } = new();

        // Totals
        public List<DailyCashupShiftTotalModel> ShiftTotals { get; set; } = new();
        public DailyCashupGrandTotalModel GrandTotal { get; set; } = new();

        // Filter options
        public List<CheckItemModel<int>> ShiftOptions { get; set; } = new();
        public List<CheckItemModel<long>> TollOperatorOptions { get; set; } = new();

        // Selected filters
        public List<int> SelectedShiftIds { get; set; } = new();
        public List<long> SelectedSystemUserIds { get; set; } = new();

        // Date range
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today;
    }
}