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

        [Column("Class_M")]
        public decimal? Class_M { get; set; }

        [Column("Class_1")]
        public decimal? Class_1 { get; set; }

        [Column("Class_2")]
        public decimal? Class_2 { get; set; }

        [Column("Class_3")]
        public decimal? Class_3 { get; set; }

        [Column("Class_4")]
        public decimal? Class_4 { get; set; }

        [Column("Class_D")]
        public decimal? Class_D { get; set; }

        public decimal? Total { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}