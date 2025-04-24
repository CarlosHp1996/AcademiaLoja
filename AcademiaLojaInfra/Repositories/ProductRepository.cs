using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Products;
using AcademiaLoja.Application.Models.Responses.Products;
using AcademiaLoja.Domain.Entities;
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
                .Include(x => x.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(x => x.Attributes)
                .AsQueryable();

            // Aplicando os filtros
            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(p => p.Name.Contains(filter.Name));

            if (filter.CategoryIds != null && filter.CategoryIds.Any())
                query = query.Where(p => p.ProductCategories.Any(pc => filter.CategoryIds.Contains(pc.CategoryId)));

            if (filter.Flavors != null && filter.Flavors.Any())
                query = query.Where(p => p.Attributes.Any(a =>
                    a.Key.ToLower() == "sabor" && filter.Flavors.Contains(a.Value)));

            if (filter.Brands != null && filter.Brands.Any())
                query = query.Where(p => p.Attributes.Any(a =>
                    a.Key.ToLower() == "marca" && filter.Brands.Contains(a.Value)));

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
            var availableCategories = await _context.Categories
                .Select(c => new FilterCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductCount = c.ProductCategories.Count
                })
                .ToListAsync(cancellationToken);

            // 2. Sabores disponíveis
            var availableFlavors = await _context.ProductAttributes
                .Where(a => a.Key.ToLower() == "sabor")
                .GroupBy(a => a.Value)
                .Select(g => new FilterAttributeDto
                {
                    Value = g.Key,
                    ProductCount = g.Count()
                })
                .ToListAsync(cancellationToken);

            // 3. Marcas disponíveis
            var availableBrands = await _context.ProductAttributes
                .Where(a => a.Key.ToLower() == "marca")
                .GroupBy(a => a.Value)
                .Select(g => new FilterAttributeDto
                {
                    Value = g.Key,
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
                Categories = availableCategories,
                Flavors = availableFlavors,
                Brands = availableBrands,
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
                product.Name = request.Name;
                product.Description = request.Description;
                product.Price = (decimal)request.Price;
                product.StockQuantity = (int)request.StockQuantity;
                product.ImageUrl = request.ImageUrl;
                product.IsActive = (bool)request.IsActive;
                product.UpdatedAt = DateTime.UtcNow;

                // Atualizar estoque no inventário
                if (product.Inventory != null)
                {
                    product.Inventory.Quantity = (int)request.StockQuantity;
                    product.Inventory.LastUpdated = DateTime.UtcNow;
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

                // Atualizar categorias
                // 1. Remover associações antigas
                _context.ProductCategories.RemoveRange(product.ProductCategories);

                // 2. Adicionar novas associações
                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    foreach (var categoryId in request.CategoryIds)
                    {
                        // Verificar se a categoria existe
                        bool categoryExists = await _context.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken);
                        if (categoryExists)
                        {
                            product.ProductCategories.Add(new ProductCategory
                            {
                                ProductId = product.Id,
                                CategoryId = categoryId
                            });
                        }
                    }
                }

                // Atualizar atributos
                // 1. Remover atributos antigos
                _context.ProductAttributes.RemoveRange(product.Attributes);

                // 2. Adicionar novos atributos
                if (request.Attributes != null && request.Attributes.Any())
                {
                    foreach (var attr in request.Attributes)
                    {
                        product.Attributes.Add(new ProductAttribute
                        {
                            ProductId = product.Id,
                            Key = attr.Key,
                            Value = attr.Value
                        });
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
                    Categories = await _context.ProductCategories
                        .Where(pc => pc.ProductId == product.Id)
                        .Select(pc => new CategoryDto
                        {
                            Id = pc.Category.Id,
                            Name = pc.Category.Name,
                            Description = pc.Category.Description
                        }).ToListAsync(cancellationToken),
                    Attributes = product.Attributes
                        .Select(a => new ProductAttributeDto
                        {
                            Key = a.Key,
                            Value = a.Value
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
                .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
                .Include(p => p.Attributes)
                .Where(x => x.Id == id)
                .AsQueryable();

            return await product.FirstOrDefaultAsync();
        }

        public async Task<CreateProductResponse> CreateProduct(CreateProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Verificar se todas as categorias existem
                var categories = new List<Category>();
                foreach (var categoryId in request.CategoryIds)
                {
                    var category = await _context.Categories.FindAsync(categoryId);
                    if (category == null)
                    {
                        throw new Exception($"Category with ID {categoryId} not found.");
                    }
                    categories.Add(category);
                }

                // Verificar se todas as marcas existem
                var brands = new List<Brand>();
                foreach (var brandId in request.BrandIds)
                {
                    var brand = await _context.Brands.FindAsync(brandId);

                    if (brand == null)
                        throw new Exception($"Brand with ID {brandId} not found.");

                    brands.Add(brand);
                }

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

                // Associar categorias ao produto
                var productCategories = new List<ProductCategory>();
                foreach (var category in categories)
                {
                    var productCategory = new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = category.Id
                    };
                    _context.ProductCategories.Add(productCategory);
                    productCategories.Add(productCategory);
                }

                // Associar marcas ao produto
                var productBrands = new List<ProductBrand>();
                foreach (var brand in brands)
                {
                    var productBrand = new ProductBrand
                    {
                        ProductId = product.Id,
                        BrandId = brand.Id
                    };
                    _context.ProductBrands.Add(productBrand);
                    productBrands.Add(productBrand);
                }

                // Adicionar atributos do produto
                var productAttributes = new List<ProductAttribute>();
                foreach (var attr in request.Attributes)
                {
                    var productAttribute = new ProductAttribute
                    {
                        Id = Guid.NewGuid(),
                        ProductId = product.Id,
                        Key = attr.Key,
                        Value = attr.Value
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
                    Categories = categories.Select(c => new CategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description
                    }).ToList(),
                    Brands = brands.Select(c => new BrandDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description
                    }).ToList(),
                    Attributes = productAttributes.Select(a => new ProductAttributeDto
                    {
                        Id = a.Id,
                        Key = a.Key,
                        Value = a.Value
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
    }
}
