using AcademiaLoja.Application.Models.Dtos;

namespace AcademiaLoja.Application.Models.Responses.Orders
{
    public class GetAllOrdersResponse
    {
        public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
        public PaginationDto Pagination { get; set; }
    }
}
