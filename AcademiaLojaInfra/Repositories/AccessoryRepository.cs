using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Infra.Data;

namespace AcademiaLoja.Infra.Repositories
{
    public class AccessoryRepository : BaseRepository<Accessory>, IAccessoryRepository
    {
        private readonly AppDbContext _context;

        public AccessoryRepository(AppDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
    }
}
