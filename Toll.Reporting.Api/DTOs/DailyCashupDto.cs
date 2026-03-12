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

        public double NettAmount { get; set; }       
        public double ActualAmount { get; set; }     
        public double TotalDeclared { get; set; }   
        public double Difference { get; set; }        
    }

    public class DailyCashupFilterOptionsDto
    {
        public List<FilterItemDto<int>> Shifts { get; set; } = new();
        public List<FilterItemDto<long>> TollOperators { get; set; } = new();
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}