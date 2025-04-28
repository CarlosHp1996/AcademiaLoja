using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Infra.Data;

namespace AcademiaLoja.Infra.Repositories
{
    public class ObjectiveRepository : BaseRepository<Objective>, IObjectiveRepository
    {
        private readonly AppDbContext _context;

        public ObjectiveRepository(AppDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
    }
}
