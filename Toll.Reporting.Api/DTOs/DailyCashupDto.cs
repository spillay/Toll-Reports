using System;
using System.Collections.Generic;

namespace Toll.Reporting.Api.DTOs
{
    public class FilterItemDto<T>
    {
        public T Id { get; set; } = default!;
        public string Name { get; set; } = "";
    }

    public class DailyCashupDto
    {
        public DateTime ShiftDate { get; set; }
        public string ShiftDescription { get; set; } = "-- None --";
        public string TollOperator { get; set; } = "-- None --";

        public decimal NettAmount { get; set; }          // Lane Cash
        public decimal ActualAmount { get; set; }        // Top-ups
        public decimal TotalCashExpected { get; set; }   // Lane Cash + Top-ups
        public decimal TotalDeclared { get; set; }       // Cash Declared
        public decimal Difference { get; set; }          // Surplus /- Shortage
        public decimal TotalBanked { get; set; }         // same as declared for now
    }

    public class DailyCashupShiftTotalDto
    {
        public string ShiftDescription { get; set; } = "-- None --";

        public decimal NettAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal TotalCashExpected { get; set; }
        public decimal TotalDeclared { get; set; }
        public decimal Difference { get; set; }
        public decimal TotalBanked { get; set; }
    }

    public class DailyCashupGrandTotalDto
    {
        public decimal NettAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal TotalCashExpected { get; set; }
        public decimal TotalDeclared { get; set; }
        public decimal Difference { get; set; }
        public decimal TotalBanked { get; set; }
    }

    public class DailyCashupFilterOptionsDto
    {
        public List<FilterItemDto<int>> Shifts { get; set; } = new();
        public List<FilterItemDto<long>> TollOperators { get; set; } = new();
    }

    public class DailyCashupResultDto
    {
        public List<DailyCashupDto> FullItems { get; set; } = new(); // all rows for export/totals
        public List<DailyCashupDto> Items { get; set; } = new();     // current page
        public List<DailyCashupShiftTotalDto> ShiftTotals { get; set; } = new();
        public DailyCashupGrandTotalDto GrandTotal { get; set; } = new();

        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
    public class DailyCashupTopupAggRow
    {
        public long SystemUserId { get; set; }
        public int ShiftId { get; set; }
        public DateTime ShiftDate { get; set; }
        public decimal ActualAmount { get; set; }
    }
}