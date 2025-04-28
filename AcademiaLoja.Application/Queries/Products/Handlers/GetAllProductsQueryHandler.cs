using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Filters;
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
                var filter = query.Filter ?? new GetProductsRequestFilter();
                var productsResult = await _repository.Get(filter);
                var products = productsResult.Result(out int totalCount).ToList();
                int pageSize = filter.PageSize ?? 10;
                int pageCount = (int)Math.Ceiling(totalCount / (double)pageSize);

                var filtersData = await _repository.GetFiltersData(cancellationToken);

                var response = new GetAllProductsResponse
                {
                    Products = products.Select(static p => new ProductListItemDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Price = p.Price,
                        StockQuantity = p.StockQuantity,
                        ImageUrl = p.ImageUrl,
                        IsActive = p.IsActive,
                        Categories = p.ProductCategories?
                            .Select(pc => pc.Category?.Name)
                            .Where(name => !string.IsNullOrWhiteSpace(name))
                            .ToList() ?? new List<string>(),
                        Brands = p.ProductBrands ?
                            .Select(pb => pb.Brand?.Name)
                            .Where(name => !string.IsNullOrWhiteSpace(name))
                            .ToList() ?? new List<string>(),
                        Objectives = p.ProductObjectives?
                            .Select(po => po.Objective?.Name)
                            .Where(name => !string.IsNullOrWhiteSpace(name))
                            .ToList() ?? new List<string>(),
                        Attributes = p.Attributes?
                            .Select(a => new ProductAttributeDto
                            {
                                Key = a.Key,
                                Value = a.Value
                            }).ToList() ?? new List<ProductAttributeDto>()
                    }).ToList(),

                    Pagination = new PaginationDto
                    {
                        CurrentPage = query.Filter.Page ?? 1,
                        PageSize = pageSize,
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
                result.WithError($"Erro ao buscar produtos: {ex.Message}");
                return result;
            }
        }

    }
}
