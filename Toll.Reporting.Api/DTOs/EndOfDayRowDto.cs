namespace Toll.Reporting.Api.DTOs.EndOfDay
{
    public class EndOfDayMatrixRowDto
    {
        public string Label { get; set; } = string.Empty;
        public decimal ClassM { get; set; }
        public decimal Class1 { get; set; }
        public decimal Class2 { get; set; }
        public decimal Class3 { get; set; }
        public decimal Class4 { get; set; }
        public decimal ClassD { get; set; }
        public decimal Total { get; set; }
    }
}