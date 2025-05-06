using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Filters;
using AcademiaLoja.Application.Models.Requests.Accessoriess;
using AcademiaLoja.Application.Models.Responses.Acessory;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Domain.Helpers;
using AcademiaLoja.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Brand = AcademiaLoja.Domain.Entities.Brand;

namespace AcademiaLoja.Infra.Repositories
{
    public class AccessoryRepository : BaseRepository<Accessory>, IAccessoryRepository
    {
        private readonly AppDbContext _context;

        public AccessoryRepository(AppDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<AccessoryResponse> CreateAccessory(CreateAccessoryRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Verificar se todas as marcas existem
                var brands = new List<Brand>();
                foreach (var brandId in request.BrandIds)
                {
                    var brand = await _context.Brands.FindAsync(brandId);

                    if (brand == null)
                        throw new Exception($"Brand with ID {brandId} not found.");

                    brands.Add(brand);
                }

                // Criar o acessório
                var accessorie = new Accessory
                {
                    Id = Guid.NewGuid(),
                    Name = request.Name,
                    Description = request.Description,
                    Color = request.Color,
                    Model = request.Model,
                    Size = request.Size
                };

                // Adicionar o acessório ao contexto
                _context.Accessories.Add(accessorie);                

                // Salvar todas as alterações de uma vez
                await _context.SaveChangesAsync(cancellationToken);

                // Preparar a resposta
                var response = new AccessoryResponse
                {
                    Id = accessorie.Id,
                    Name = accessorie.Name,
                    Description = accessorie.Description,
                    Color = accessorie.Color,
                    Model = accessorie.Model,
                    Size = accessorie.Size,                    
                    Message = "Accessory created successfully."
                };               

                return response;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<AsyncOutResult<IEnumerable<Accessory>, int>> Get(GetAccessoriessRequestFilter filter)
        {
            // Extrair parâmetros de paginação e ordenação do filtro
            int page = filter.Page ?? 1;
            int pageSize = filter.PageSize ?? 10;
            int offset = (page - 1) * pageSize;
            string sortBy = filter.SortBy ?? "Name";
            bool ascending = filter.SortDirection?.ToLower() != "desc";

            // Inicializando a consulta
            var query = _context.Accessories
                .AsQueryable();

            // Aplicando os filtros
            if (!string.IsNullOrEmpty(filter.Name))
                query = query.Where(p => p.Name.Contains(filter.Name));

            if (!string.IsNullOrEmpty(filter.Description))
                query = query.Where(p => p.Name.Contains(filter.Description));

            if (filter.Color != null)
                query = query.Where(p => p.Color == filter.Color);

            if (filter.Model != null)
                query = query.Where(p => p.Model == filter.Model);

            if (filter.Size != null)
                query = query.Where(p => p.Size == filter.Size);           

            // Ordenação dinâmica
            if (DataHelpers.CheckExistingProperty<Accessory>(sortBy))
                query = query.OrderByDynamic(sortBy, ascending);
            else
                query = ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);

            var totalCount = await query.CountAsync();
            var accessories = await query.Skip(offset).Take(pageSize).ToListAsync();

            return new AsyncOutResult<IEnumerable<Accessory>, int>(accessories, totalCount);
        }
    }
}
