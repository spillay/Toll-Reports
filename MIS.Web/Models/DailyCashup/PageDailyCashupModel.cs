using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace MIS.Web.Models.DailyCashup
{
    // ✅ Keep everything in this same file (no new file)
    public class CheckItemModel<T>
    {
        public T Id { get; set; } = default!;
        public string Name { get; set; } = "";
        public bool Selected { get; set; }
    }

    public class PageDailyCashupModel : PageModel
    {
        public List<DailyCashupModel> Items { get; set; } = new();

        public List<CheckItemModel<int>> ShiftOptions { get; set; } = new();
        public List<CheckItemModel<long>> TollOperatorOptions { get; set; } = new();

        public List<int> SelectedShiftIds { get; set; } = new();
        public List<long> SelectedSystemUserIds { get; set; } = new();

        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today;
    }
}