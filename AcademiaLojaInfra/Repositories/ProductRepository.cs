using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Products;
using AcademiaLoja.Application.Models.Responses.Products;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Domain.Enums;
using AcademiaLoja.Domain.Helpers;
using AcademiaLoja.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLoja.Infra.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<AsyncOutResult<IEnumerable<Product>, int>> Get(GetProductsRequestFilter filter)
        {
            // Extrair parâmetros de paginação e ordenação do filtro
            int page = filter.Page ?? 1;
            int pageSize = filter.PageSize ?? 10;
            int offset = (page - 1) * pageSize;
            string sortBy = filter.SortBy ?? "Name";
            bool ascending = filter.SortDirection?.ToLower() != "desc";

            // Inicializando a consulta
            var query = _context.Products
                .Include(x => x.Attributes)                
                .AsQueryable();

            // Aplicando os filtros
            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(p => p.Name.Contains(filter.Name));           

            // Filtro por sabor usando o novo campo Flavor do enum
            if (filter.Flavors != null && filter.Flavors.Any())
            {
                var flavorValues = filter.Flavors.Select(f => Enum.Parse<EnumFlavor>(f)).ToList();
                query = query.Where(p => p.Attributes.Any(a => a.Flavor != null && flavorValues.Contains(a.Flavor.Value)));
            }

            // Filtro por marca usando o novo campo Brand do enum
            if (filter.BrandIds != null && filter.BrandIds.Any())
            {
                var brandValues = filter.BrandIds.Select(b => Enum.Parse<EnumBrand>(b)).ToList();
                query = query.Where(p => p.Attributes.Any(a => a.Brand != null && brandValues.Contains(a.Brand.Value)));
            }

            if (filter.QuantityRanges != null && filter.QuantityRanges.Any())
            {
                var maxRange = filter.QuantityRanges.Max();
                query = query.Where(p => p.StockQuantity <= maxRange);
            }

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            if (filter.IsActive.HasValue)
                query = query.Where(p => p.IsActive == filter.IsActive.Value);

            // Ordenação dinâmica
            if (DataHelpers.CheckExistingProperty<Product>(sortBy))
                query = query.OrderByDynamic(sortBy, ascending);
            else
                query = ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);

            var totalCount = await query.CountAsync();
            var products = await query.Skip(offset).Take(pageSize).ToListAsync();

            return new AsyncOutResult<IEnumerable<Product>, int>(products, totalCount);
        }

        public async Task<FiltersDto> GetFiltersData(CancellationToken cancellationToken)
        {
            // 1. Categorias disponíveis

            // 2. Sabores disponíveis usando o Enum
            var availableFlavors = await _context.ProductAttributes
                .Where(a => a.Flavor != null)
                .GroupBy(a => a.Flavor)
                .Select(g => new FilterAttributeDto
                {
                    Value = g.Key.ToString(),
                    ProductCount = g.Count()
                })
                .ToListAsync(cancellationToken);

            // 3. Marcas disponíveis usando o Enum
            var availableBrands = await _context.ProductAttributes
                .Where(a => a.Brand != null)
                .GroupBy(a => a.Brand)
                .Select(g => new FilterAttributeDto
                {
                    Value = g.Key.ToString(),
                    ProductCount = g.Count()
                })
                .ToListAsync(cancellationToken);

            // 4. Faixas de quantidade
            var quantityRanges = new List<FilterQuantityRangeDto>
            {
                new FilterQuantityRangeDto { MinQuantity = 0, MaxQuantity = 10, ProductCount = 0 },
                new FilterQuantityRangeDto { MinQuantity = 11, MaxQuantity = 50, ProductCount = 0 },
                new FilterQuantityRangeDto { MinQuantity = 51, MaxQuantity = 100, ProductCount = 0 },
                new FilterQuantityRangeDto { MinQuantity = 101, MaxQuantity = int.MaxValue, ProductCount = 0 }
            };

            // Contar produtos em cada faixa de quantidade
            foreach (var range in quantityRanges)
            {
                range.ProductCount = await _context.Products
                    .CountAsync(p => p.StockQuantity >= range.MinQuantity && p.StockQuantity <= range.MaxQuantity, cancellationToken);
            }

            // 5. Faixa de preço
            var minPrice = await _context.Products.MinAsync(p => p.Price, cancellationToken);
            var maxPrice = await _context.Products.MaxAsync(p => p.Price, cancellationToken);

            return new FiltersDto
            {
                QuantityRanges = quantityRanges,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };
        }

        public async Task<UpdateProductResponse> UpdateProduct(Product product, UpdateProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Atualizar propriedades básicas do produto
                var existingProduct = await _context.Products
                    .Include(p => p.Attributes)
                    .Include(p => p.Inventory)
                    .FirstOrDefaultAsync(p => p.Id == product.Id, cancellationToken) ?? throw new Exception($"Product with ID {product.Id} not found.");

                // Atualizar propriedades do produto


                existingProduct.Name = request.Name;
                existingProduct.Description = request.Description;
                existingProduct.Price = (decimal)request.Price;
                existingProduct.StockQuantity = (int)request.StockQuantity;
                existingProduct.ImageUrl = request.ImageUrl;
                existingProduct.IsActive = (bool)request.IsActive;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                // Atualizar estoque no inventário
                if (request.InventoryId is not null)
                {
                    existingProduct.Inventory.Quantity = (int)request.StockQuantity;
                    existingProduct.Inventory.LastUpdated = DateTime.UtcNow;
                }
                else
                {
                    // Criar inventário se não existir
                    product.Inventory = new Inventory
                    {
                        ProductId = product.Id,
                        Quantity = (int)request.StockQuantity,
                        LastUpdated = DateTime.UtcNow
                    };
                }                

                // Atualizar atributos
                // 1. Remover atributos antigos
                _context.ProductAttributes.RemoveRange(product.Attributes);

                // 2. Adicionar novos atributos
                if (request.Attributes != null && request.Attributes.Any())
                {
                    foreach (var attr in request.Attributes)
                    {
                        var productAttribute = new ProductAttribute
                        {
                            ProductId = product.Id,
                            Key = attr.Key,
                            Value = attr.Value,
                            Flavor = attr.Flavor,
                            Brand = attr.Brand,
                            Category = attr.Category,
                            Objective = attr.Objective,
                            Accessory = attr.Accessory
                        };                                                

                        product.Attributes.Add(productAttribute);
                    }
                }

                // Salvar alterações
                await _context.SaveChangesAsync(cancellationToken);

                // Criar resposta
                var response = new UpdateProductResponse
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    ImageUrl = product.ImageUrl,
                    IsActive = product.IsActive,
                    UpdatedAt = product.UpdatedAt,
                    InventoryId = product.Inventory.Id,                   
                    Attributes = product.Attributes
                        .Select(a => new ProductAttributeDto
                        {
                            Key = GetAttributeTypeName(a),
                            Value = GetAttributeValue(a),
                            Flavor = a.Flavor,
                            Brand = a.Brand,
                            Category = a.Category,
                            Objective = a.Objective,
                            Accessory = a.Accessory
                        }).ToList()
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating product: {ex.Message}", ex);
            }
        }

        public async Task<Product> GetProductById(Guid id)
        {
            var product = _context.Products
                .Where(x => x.Id == id)
                .Include(p => p.Attributes)                
                .Include(p => p.Inventory)
                .Include(p => p.OrderItems)
                .AsQueryable();

            return await product.FirstOrDefaultAsync();
        }

        public async Task<CreateProductResponse> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken)
        {
            try
            {               
                // Criar o produto
                var now = DateTime.UtcNow;
                var product = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    ImageUrl = request.ImageUrl,
                    IsActive = request.IsActive,
                    CreatedAt = now,
                    UpdatedAt = now
                };

                // Adicionar o produto ao contexto
                _context.Products.Add(product);

                // Criar o inventário do produto
                var inventory = new Inventory
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Quantity = request.StockQuantity,
                    LastUpdated = now
                };
                _context.Inventories.Add(inventory);               

                // Adicionar atributos do produto
                var productAttributes = new List<ProductAttribute>();
                foreach (var attr in request.Attributes)
                {
                    var productAttribute = new ProductAttribute
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Key = attr.Key,
                        Value = attr.Value,
                        Flavor = attr.Flavor,
                        Brand = attr.Brand,
                        Category = attr.Category,
                        Objective = attr.Objective,
                        Accessory = attr.Accessory
                    };

                    _context.ProductAttributes.Add(productAttribute);
                    productAttributes.Add(productAttribute);
                }

                // Salvar todas as alterações de uma vez
                await _context.SaveChangesAsync(cancellationToken);

                // Preparar a resposta
                var response = new CreateProductResponse
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
                    Attributes = productAttributes.Select(a => new ProductAttributeDto
                    {
                        Id = a.Id,
                        Key = GetAttributeTypeName(a),
                        Value = GetAttributeValue(a),
                        Flavor = a.Flavor,
                        Brand = a.Brand,
                        Category = a.Category,
                        Objective = a.Objective,
                        Accessory = a.Accessory
                    }).ToList(),                    
                    Message = "Product created successfully."
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating product: {ex.Message}", ex);
            }
        }

        // Métodos auxiliares para lidar com os atributos
        private string GetAttributeTypeName(ProductAttribute attribute)
        {
            if (attribute.Flavor.HasValue) return "Flavor";
            if (attribute.Brand.HasValue) return "Brand";
            if (attribute.Category.HasValue) return "Category";
            if (attribute.Objective.HasValue) return "Objective";
            if (attribute.Accessory.HasValue) return "Accessory";
            return attribute.Key; // Fallback para atributos customizados
        }

        private string GetAttributeValue(ProductAttribute attribute)
        {
            if (attribute.Flavor.HasValue) return attribute.Flavor.Value.ToString();
            if (attribute.Brand.HasValue) return attribute.Brand.Value.ToString();
            if (attribute.Category.HasValue) return attribute.Category.Value.ToString();
            if (attribute.Objective.HasValue) return attribute.Objective.Value.ToString();
            if (attribute.Accessory.HasValue) return attribute.Accessory.Value.ToString();
            return attribute.Value; // Fallback para atributos customizados
        }
    }
}