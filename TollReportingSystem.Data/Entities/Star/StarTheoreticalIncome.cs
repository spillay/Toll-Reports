using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TollReportingSystem.Data.Entities.Star
{
    [Table("TheoreticalIncome", Schema = "star")]
    [Keyless]
    public class StarTheoreticalIncome
    {
        public DateTime ReportDate { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public string Metric { get; set; } = string.Empty;

        public decimal? Class_M { get; set; }
        public decimal? Class_I { get; set; }
        public decimal? Class_II { get; set; }
        public decimal? Class_III { get; set; }

        public decimal? Total { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
