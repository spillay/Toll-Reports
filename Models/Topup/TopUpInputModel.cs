using MIS.Web.Models.TopUp;
using System;
using System.Collections.Generic;

namespace MIS.Web.Models
{
    public class TopUpInputModel : PageModel
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<string>? OperatorIds { get; set; } = new();
        public List<string>? Lanes { get; set; } = new();
        public List<string>? Shifts { get; set; } = new();
        public List<string>? PaymentMethods { get; set; } = new();

        public string? AccountNumber { get; set; }

        public bool? OperationalDate { get; set; }

        public List<string>? OperatorOptions { get; set; } = new();
        public List<string>? LaneOptions { get; set; } = new();
        public List<string>? ShiftOptions { get; set; } = new();
        public List<string>? PaymentMethodOptions { get; set; } = new();

        public PageTopUpModel? PageData { get; set; }
    }
}