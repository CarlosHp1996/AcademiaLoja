using AcademiaLoja.Application.Services;
using AcademiaLoja.Application.Services.Interfaces;

namespace AcademiaLoja.Web.Configuration
{
    public static class StripeConfiguration
    {
        public static IServiceCollection AddStripeServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar o serviço de pagamento
            services.AddScoped<IPaymentService, StripePaymentService>();

            // Configurar a chave secreta do Stripe
            Stripe.StripeConfiguration.ApiKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY_ACADEMIA") ??
                         configuration["Stripe:STRIPE_SECRET_KEY_ACADEMIA"];

            return services;
        }

        public static IApplicationBuilder UseStripeConfiguration(this IApplicationBuilder app)
        {
            // Qualquer configuração adicional do Stripe que precise ser feita no pipeline
            return app;
        }
    }
}