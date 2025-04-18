using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Responses.Products;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Products.Handlers
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Result<GetAllProductsResponse>>
    {
        private readonly IProductRepository _repository;

        public GetAllProductsQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<GetAllProductsResponse>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken)
        {
            var result = new Result<GetAllProductsResponse>();

            try
            {
                // Buscar produtos com filtros  
                var productsResult = await _repository.Get(query.Filter);

                // Correctly retrieve the result and count using the out parameter  
                var products = productsResult.Result(out int totalCount).ToList();
                int pageCount = (int)Math.Ceiling(totalCount / (double)query.Filter.PageSize);

                // Buscar dados dos filtros  
                var filtersData = await _repository.GetFiltersData(cancellationToken);

                // Construir a resposta  
                var response = new GetAllProductsResponse
                {
                    Products = products.Select(p => new ProductListItemDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        StockQuantity = p.StockQuantity,
                        ImageUrl = p.ImageUrl,
                        IsActive = p.IsActive,
                        Categories = p.ProductCategories.Select(pc => pc.Category.Name).ToList(),
                        Attributes = p.Attributes.Select(a => new ProductAttributeDto
                        {
                            Key = a.Key,
                            Value = a.Value
                        }).ToList()
                    }).ToList(),
                    Pagination = new PaginationDto
                    {
                        CurrentPage = (int)query.Filter.Page,
                        PageSize = query.Filter.PageSize,
                        TotalItems = totalCount,
                        TotalPages = pageCount
                    },
                    Filters = filtersData
                };

                result.Value = response;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError($"Error retrieving products: {ex.Message}");
                return result;
            }
        }
    }
}
