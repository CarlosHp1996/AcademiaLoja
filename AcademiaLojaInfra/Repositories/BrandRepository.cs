using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Infra.Data;

namespace AcademiaLoja.Infra.Repositories
{
    public class BrandRepository : BaseRepository<Brand>, IBrandRepository
    {
        private readonly AppDbContext _context;

        public BrandRepository(AppDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
    }
}
