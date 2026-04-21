using MIS.Web.Models.Comprehensive;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MIS.Web.Services
{
    public interface IComprehensiveReportService
    {
        Task<List<ComprehensiveModel>> GetComprehensiveDetailsAsync(
            DateTime startDate,
            DateTime endDate,
            List<byte>? shiftIds = null,
            List<long>? operatorIds = null,
            List<int>? laneIds = null,
            List<byte>? discountTypeIds = null,
            List<byte>? tollClassIds = null,
            List<byte>? paymentMethodIds = null
        );

        // ✅ Must return ALL values regardless of date range (master tables)
        Task<ComprehensiveOptionsResponse> GetComprehensiveOptionsAsync();
    }

    // ✅ Keep this inside the same file (no new file)
    public class ComprehensiveOptionsResponse
    {
        public List<PageComprehensiveModel.FilterOption<byte>> Shifts { get; set; } = new();
        public List<PageComprehensiveModel.FilterOption<long>> Operators { get; set; } = new();
        public List<PageComprehensiveModel.FilterOption<int>> Lanes { get; set; } = new();
        public List<PageComprehensiveModel.FilterOption<byte>> DiscountTypes { get; set; } = new();
        public List<PageComprehensiveModel.FilterOption<byte>> TollClasses { get; set; } = new();
        public List<PageComprehensiveModel.FilterOption<byte>> PaymentMethods { get; set; } = new();
    }
}