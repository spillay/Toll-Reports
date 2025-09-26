namespace MIS.Web.Services
{
    public interface IReportService
    {
        public Task<string> GetShiftsAsync();
    }
}
