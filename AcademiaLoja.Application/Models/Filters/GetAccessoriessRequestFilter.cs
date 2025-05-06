using AcademiaLoja.Domain.Enums;
using System;

namespace AcademiaLoja.Application.Models.Filters
{
    public class GetAccessoriessRequestFilter : BaseRequestFilter
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public EnumColor? Color { get; set; }
        public EnumModel? Model { get; set; }
        public EnumSize? Size { get; set; }
        public decimal? MinPrice { get; set; } // Preço mínimo
        public decimal? MaxPrice { get; set; } // Preço máximo
        public string? BrandName { get; set; }

    }
}
