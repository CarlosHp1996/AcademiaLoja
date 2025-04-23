using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Infra.Data;

namespace AcademiaLoja.Infra.Repositories
{
    public class SubCategoryRepository : BaseRepository<SubCategory>, ISubCategoryRepository
    {
        private readonly AppDbContext _context;

        public SubCategoryRepository(AppDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
    }
}
