using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Products;
using AcademiaLoja.Application.Models.Responses.Products;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Domain.Helpers;

namespace AcademiaLoja.Application.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<AsyncOutResult<IEnumerable<Product>, int>> Get(GetProductsRequestFilter filter);
        Task<FiltersDto> GetFiltersData(CancellationToken cancellationToken);
        Task<UpdateProductResponse> UpdateProduct(Product product, UpdateProductRequest request, CancellationToken cancellationToken);
        Task<Product> GetProductById(Guid id);
        Task<CreateProductResponse> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken);
    }
}
