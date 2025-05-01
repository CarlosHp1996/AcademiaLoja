using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Requests.Categories;
using AcademiaLoja.Application.Models.Responses.Categories;
using AcademiaLoja.Application.Models.Responses.SubCategories;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLoja.Infra.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<CategoryResponse> CreateCategory(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Verificar se todas as subcategorias existem
                var subCategories = new List<SubCategory>();
                foreach (var subCategoryId in request.SubCategoryIds)
                {
                    var subCategory = await _context.SubCategories.FindAsync(subCategoryId);
                    if (subCategory == null)
                    {
                        throw new Exception($"SubCategory with ID {subCategoryId} not found.");
                    }
                    subCategories.Add(subCategory);
                }               

                // Criar a categoria
                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                };

                // Adicionar a categoria ao contexto
                _context.Categories.Add(category);

                // Associar subcategorias a categoria
                var categorySubcategories = new List<CategorySubCategory>();
                foreach (var subcategory in subCategories)
                {
                    var categorySubcategory = new CategorySubCategory
                    {
                        CategoryId = category.Id,
                        SubCategoryId = subcategory.Id
                    };
                    _context.CategorySubCategories.Add(categorySubcategory);
                    categorySubcategories.Add(categorySubcategory);
                }              


                // Salvar todas as alterações de uma vez
                await _context.SaveChangesAsync(cancellationToken);

                // Preparar a resposta
                var response = new CategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    SubCategories = subCategories.Select(c => new SubCategoryResponse
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description
                    }).ToList(),
                    Message = "Category created successfully."
                };                

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating category: {ex.Message}", ex);
            }
        }

        public async Task<CategoryResponse> UpdateCategory(Category category, UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Atualizar propriedades básicas da categoria
                category.Name = request.Name;
                category.Description = request.Description; 

                // Atualizar categorias
                // 1. Remover associações antigas
                _context.CategorySubCategories.RemoveRange(category.CategorySubCategories);

                // 2. Adicionar novas associações
                if (request.SubCategoryIds != null && request.SubCategoryIds.Any())
                {
                    foreach (var subCategoryId in request.SubCategoryIds)
                    {
                        // Verificar se a subCategoria existe
                        bool subCategoryExists = await _context.SubCategories.AnyAsync(c => c.Id == subCategoryId, cancellationToken);
                        if (subCategoryExists)
                        {
                            category.CategorySubCategories.Add(new CategorySubCategory
                            {
                                CategoryId = category.Id,
                                SubCategoryId = subCategoryId
                            });                          
                        }
                    }
                }                

                // Salvar alterações
                await _context.SaveChangesAsync(cancellationToken);

                // Criar resposta
                var response = new CategoryResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,                    
                    SubCategories = await _context.CategorySubCategories
                        .Where(pc => pc.CategoryId == category.Id)
                        .Select(pc => new SubCategoryResponse
                        {
                            Id = pc.SubCategory.Id,
                            Name = pc.SubCategory.Name,
                            Description = pc.SubCategory.Description
                        }).ToListAsync(cancellationToken)                  
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating category: {ex.Message}", ex);
            }
        }
    }
}
