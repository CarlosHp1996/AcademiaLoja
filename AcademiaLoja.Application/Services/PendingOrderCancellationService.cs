//using AcademiaLoja.Application.Interfaces;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace AcademiaLoja.Application.Services
//{
//    public class PendingOrderCancellationService : IHostedService, IDisposable
//    {
//        private readonly ILogger<PendingOrderCancellationService> _logger;
//        private readonly IServiceScopeFactory _scopeFactory;
//        private Timer _timer;

//        public PendingOrderCancellationService(ILogger<PendingOrderCancellationService> logger, IServiceScopeFactory scopeFactory)
//        {
//            _logger = logger;
//            _scopeFactory = scopeFactory;
//        }

//        public Task StartAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Serviço de cancelamento de pedidos pendentes está iniciando.");

//            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5)); // Executa a cada 5 minutos

//            return Task.CompletedTask;
//        }

//        private void DoWork(object state)
//        {
//            _logger.LogInformation("Verificando pedidos pendentes para cancelamento...");

//            using (var scope = _scopeFactory.CreateScope())
//            {
//                var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
//                var ordersToCancel = orderRepository.GetPendingOrders(TimeSpan.FromMinutes(30)).Result;

//                foreach (var order in ordersToCancel)
//                {
//                    _logger.LogInformation($"Cancelando pedido {order.Id} por falta de pagamento.");
//                    // Aqui você pode adicionar a lógica para cancelar o pedido, 
//                    // como alterar o status para 'Cancelado' e retornar os itens ao estoque.
//                    // Por enquanto, vamos apenas logar a informação.
//                }
//            }
//        }

//        public Task StopAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Serviço de cancelamento de pedidos pendentes está parando.");

//            _timer?.Change(Timeout.Infinite, 0);

//            return Task.CompletedTask;
//        }

//        public void Dispose()
//        {
//            _timer?.Dispose();
//        }
//    }
//}
