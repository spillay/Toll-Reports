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
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }

        public int? Class_1_AnonymousCount { get; set; }
        public int? Class_2_AnonymousCount { get; set; }
        public int? Class_3_AnonymousCount { get; set; }
        public int? Class_4_AnonymousCount { get; set; }
        public int? Class_D_AnonymousCount { get; set; }
        public int? Class_M_AnonymousCount { get; set; }

        public decimal? Class_1_AnonymousAmount { get; set; }
        public decimal? Class_2_AnonymousAmount { get; set; }
        public decimal? Class_3_AnonymousAmount { get; set; }
        public decimal? Class_4_AnonymousAmount { get; set; }
        public decimal? Class_D_AnonymousAmount { get; set; }
        public decimal? Class_M_AnonymousAmount { get; set; }

        public int? Class_1_StaffCount { get; set; }
        public int? Class_2_StaffCount { get; set; }
        public int? Class_3_StaffCount { get; set; }
        public int? Class_4_StaffCount { get; set; }
        public int? Class_D_StaffCount { get; set; }
        public int? Class_M_StaffCount { get; set; }

        public decimal? Class_1_StaffAmount { get; set; }
        public decimal? Class_2_StaffAmount { get; set; }
        public decimal? Class_3_StaffAmount { get; set; }
        public decimal? Class_4_StaffAmount { get; set; }
        public decimal? Class_D_StaffAmount { get; set; }
        public decimal? Class_M_StaffAmount { get; set; }

        public decimal? Class_1_IndividualAmount { get; set; }
        public decimal? Class_2_IndividualAmount { get; set; }
        public decimal? Class_3_IndividualAmount { get; set; }
        public decimal? Class_4_IndividualAmount { get; set; }
        public decimal? Class_D_IndividualAmount { get; set; }
        public decimal? Class_M_IndividualAmount { get; set; }

        public decimal? Class_1_CorporateAmount { get; set; }
        public decimal? Class_2_CorporateAmount { get; set; }
        public decimal? Class_3_CorporateAmount { get; set; }
        public decimal? Class_4_CorporateAmount { get; set; }
        public decimal? Class_D_CorporateAmount { get; set; }
        public decimal? Class_M_CorporateAmount { get; set; }

        public int? TotalDiscountCount { get; set; }
        public decimal? TotalDiscountAmount { get; set; }

        public DateTime CreatedDateTime { get; set; }
    }
}