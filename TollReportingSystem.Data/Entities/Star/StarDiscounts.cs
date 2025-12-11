using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TollReportingSystem.Data.Entities.Star
{
    [Table("Discounts", Schema = "star")]
    [Keyless]

    public class StarDiscounts
    {
        public DateTime ReportDate { get; set; }

        public decimal? Class_M_AnonymousAmount { get; set; }
        public decimal? Class_I_AnonymousAmount { get; set; }
        public decimal? Class_II_AnonymousAmount { get; set; }
        public decimal? Class_III_AnonymousAmount { get; set; }

        public decimal? Class_M_StaffAmount { get; set; }
        public decimal? Class_I_StaffAmount { get; set; }
        public decimal? Class_II_StaffAmount { get; set; }
        public decimal? Class_III_StaffAmount { get; set; }

        public decimal? Class_M_IndividualAmount { get; set; }
        public decimal? Class_I_IndividualAmount { get; set; }
        public decimal? Class_II_IndividualAmount { get; set; }
        public decimal? Class_III_IndividualAmount { get; set; }

        public decimal? Class_M_CorporateAmount { get; set; }
        public decimal? Class_I_CorporateAmount { get; set; }
        public decimal? Class_II_CorporateAmount { get; set; }
        public decimal? Class_III_CorporateAmount { get; set; }

        public int? TotalDiscountCount { get; set; }
        public decimal? TotalDiscountAmount { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}
