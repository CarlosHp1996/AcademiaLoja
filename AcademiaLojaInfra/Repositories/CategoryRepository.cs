using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Infra.Data;

namespace AcademiaLoja.Infra.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
    }
}
