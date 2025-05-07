namespace AcademiaLoja.Application.Models.Filters
{
    public class GetOrdersRequestFilter : BaseRequestFilter
    {
        public Guid? UserId { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string? SortBy { get; set; } = "OrderDate";
        public bool SortDesc { get; set; } = true;
    }
}
