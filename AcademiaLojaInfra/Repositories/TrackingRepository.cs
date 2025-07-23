using AcademiaLoja.Application.Interfaces;
using AcademiaLoja.Application.Models.Dtos;
using AcademiaLoja.Application.Models.Requests.Trackings;
using AcademiaLoja.Application.Models.Responses.Trackings;
using AcademiaLoja.Application.Services.Interfaces;
using AcademiaLoja.Domain.Entities;
using AcademiaLoja.Domain.Helpers;
using AcademiaLoja.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace AcademiaLoja.Infra.Repositories
{
    public class TrackingRepository : BaseRepository<Tracking>, ITrackingRepository
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public TrackingRepository(AppDbContext dbContext, IEmailService emailService) : base(dbContext)
        {
            _context = dbContext;
            _emailService = emailService;
        }

        public async Task<TrackingResponse> CreateTrackingEventAsync(CreateTrackingRequest request)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.User)
                    .Where(o => o.Id == request.OrderId)
                .FirstOrDefaultAsync();

                if (order == null)
                    throw new KeyNotFoundException("Pedido não encontrado.");

                var trackingEvent = new Tracking
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    TrackingNumber = request.TrackingNumber,
                    Status = request.Status,
                    Description = request.Description,
                    Location = request.Location,
                    EventDate = request.EventDate,
                    CreatedAt = DateTime.UtcNow
                };

                var email = _emailService.SendEmailConfirmationTrackingAsync(order.User.Email);

                if (email.Exception != null)
                    throw new KeyNotFoundException("Erro ao enviar o email de rastreio.");

                _context.Trackings.Add(trackingEvent);
                await _context.SaveChangesAsync();

                // Preparar a resposta
                var response = new TrackingResponse
                {
                    Id = trackingEvent.Id,
                    OrderId = order.Id,
                    Status = trackingEvent.Status,
                    Description = trackingEvent.Description,
                    Location = trackingEvent.Location,
                    EventDate = trackingEvent.EventDate,
                    CreatedAt = trackingEvent.CreatedAt,
                    TrackingNumber = trackingEvent.TrackingNumber
                };

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }       

        public async Task<TrackingResponse> UpdateTrackingAsync(Tracking tracking, UpdateTrackingRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar o evento mais recente
                var existingTracking = await _context.Trackings
                    .FirstOrDefaultAsync(t => t.Id == tracking.Id, cancellationToken);

                if (existingTracking == null)
                    throw new KeyNotFoundException("Evento de rastreamento não encontrado.");

                // Atualizar as propriedades
                tracking.Status = request.Status;
                tracking.Description = request.Description;
                tracking.Location = request.Location;
                tracking.EventDate = request.EventDate;
                tracking.TrackingNumber = request.TrackingNumber;

                // Salvar alterações
                await _context.SaveChangesAsync(cancellationToken);

                // Criar resposta
                var response = new TrackingResponse
                {
                    Id = tracking.Id,
                    OrderId = tracking.OrderId,
                    Status = tracking.Status,
                    Description = tracking.Description,
                    Location = tracking.Location,
                    EventDate = tracking.EventDate,
                    CreatedAt = tracking.CreatedAt,
                    TrackingNumber = tracking.TrackingNumber
                };

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }        

        // Novo método que implementa toda a lógica que estava no handler
        public async Task<(List<TrackingResponse> Trackings, int TotalCount)> GetTrackingsFilteredAsync(
            int? page,
            int? pageSize,
            string sortingProp,
            bool ascending,
            string status = null,
            Guid? orderId = null,
            string trackingNumber = null,
            string description = null,
            string location = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            CancellationToken cancellationToken = default)
        {
            // Calcular paginação
            var skip = (page - 1) * pageSize;
            var take = pageSize;

            // Criar uma consulta inicial
            var query = _context.Trackings.AsQueryable();

            // Aplicar filtros se necessário
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(t => t.Status.Contains(status, StringComparison.OrdinalIgnoreCase));
            }

            if (orderId.HasValue)
            {
                query = query.Where(t => t.OrderId == orderId.Value);
            }

            if (startDate.HasValue)
            {
                query = query.Where(t => t.EventDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(t => t.EventDate <= endDate.Value);
            }

            // Calcular total antes de aplicar paginação
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplicar ordenação
            if (!string.IsNullOrEmpty(sortingProp))
            {
                // Implementar ordenação dinâmica com base na propriedade
                switch (sortingProp.ToLowerInvariant())
                {
                    case "eventdate":
                        query = ascending
                            ? query.OrderBy(t => t.EventDate)
                            : query.OrderByDescending(t => t.EventDate);
                        break;
                    case "status":
                        query = ascending
                            ? query.OrderBy(t => t.Status)
                            : query.OrderByDescending(t => t.Status);
                        break;
                    case "location":
                        query = ascending
                            ? query.OrderBy(t => t.Location)
                            : query.OrderByDescending(t => t.Location);
                        break;
                    default:
                        // Ordenação padrão
                        query = query.OrderByDescending(t => t.EventDate);
                        break;
                }
            }
            else
            {
                // Ordenação padrão
                query = query.OrderByDescending(t => t.EventDate);
            }

            // Aplicar paginação
            var trackings = await query
                .Skip((int)skip) 
                .Take((int)take)
                .ToListAsync(cancellationToken);

            // Mapear para o modelo de resposta
            var trackingResponses = trackings.Select(t => new TrackingResponse
            {
                Id = t.Id,
                OrderId = t.OrderId,
                Status = t.Status,
                Description = t.Description,
                Location = t.Location,
                TrackingNumber = t.TrackingNumber,
                EventDate = t.EventDate,
                CreatedAt = t.CreatedAt
            }).ToList();

            return (trackingResponses, totalCount);
        }
    }
}