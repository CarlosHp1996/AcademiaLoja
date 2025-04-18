﻿using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Responses.Products;
using AcademiaLoja.Domain.Helpers;
using MediatR;

namespace AcademiaLoja.Application.Queries.Products.Handlers
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<GetProductByIdResponse>>
    {
        private readonly IProductRepository _repository;

        public GetProductByIdQueryHandler(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<GetProductByIdResponse>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            var result = new Result<GetProductByIdResponse>();

            try
            {
                // Buscar o produto pelo ID incluindo suas relações
                var product = await _repository.GetById(query.Id);                

                if (product == null)
                {
                    result.WithError("Product not found");
                    return result;
                }

                // Mapear para o DTO de resposta
                var response = new GetProductByIdResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    ImageUrl = product.ImageUrl,
                    IsActive = product.IsActive,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    Categories = product.ProductCategories
                        .Select(pc => new CategoryDto
                        {
                            Id = pc.Category.Id,
                            Name = pc.Category.Name,
                            Description = pc.Category.Description
                        }).ToList(),
                    Attributes = product.Attributes
                        .Select(a => new ProductAttributeDto
                        {
                            Key = a.Key,
                            Value = a.Value
                        }).ToList()
                };

                result.Value = response;
                result.HasSuccess = true;
                return result;
            }
            catch (Exception ex)
            {
                result.WithError($"Error retrieving product: {ex.Message}");
                return result;
            }
        }
    }
}
