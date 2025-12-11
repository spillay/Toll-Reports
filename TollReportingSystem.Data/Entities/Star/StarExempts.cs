using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TollReportingSystem.Data.Entities.Star
{
    [Table("Exempts", Schema = "star")]
    [Keyless]
    public class StarExempts
    {
        public DateTime ReportDate { get; set; }

        public decimal? Class_M_ExemptAmount { get; set; }
        public decimal? Class_I_ExemptAmount { get; set; }
        public decimal? Class_II_ExemptAmount { get; set; }
        public decimal? Class_III_ExemptAmount { get; set; }

        public int? TotalExemptCount { get; set; }
        public decimal? TotalExemptAmount { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
