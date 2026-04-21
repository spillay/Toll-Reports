using System;
using System.Collections.Generic;

namespace MIS.Web.Models.Traffic.Monthly
{
    public class MonthlyTrafficInputModel
    {
        public int? Year { get; set; }
        public int? Month { get; set; }
        public bool OperationalMonth { get; set; } = false;
        public List<int> Shifts { get; set; } = new List<int>();
       // public string? Classification { get; set; }
        public List<string> Classifications { get; set; } = new();


    }
}
