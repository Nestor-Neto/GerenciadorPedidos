using GerenciadorPedidos.Domain.Interfaces;
using GerenciadorPedidos.Domain.Interfaces.IRepositories;
using GerenciadorPedidos.Domain.Interfaces.IServices;
using GerenciadorPedidos.Domain.Services;
using GerenciadorPedidos.Infra.Context;
using GerenciadorPedidos.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GerenciadorPedidos.Infra.CrossCutting.IoC
{
    public static class NativeInjector
    {
        /// <summary>
        /// Dependency Injection
        /// </summary>
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            #region Constantes


            #endregion
            // Configuração do Entity Framework
            services.AddDbContext<PedidoDbContext>(options =>
                options.UseInMemoryDatabase("PedidosDb"));
            #region Banco 
            // Registro das dependências
            services.AddScoped<ICalculadoraImpostoService, CalculadoraImpostoService>();
            services.AddScoped<ISistemaBService, SistemaBService>();
            services.AddScoped<IPedidoRepository, PedidoRepository>();
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<ICalculoImpostoFactoryServices, CalculoImpostoFactoryServices>();
            services.AddScoped<ICalculoImpostoStrategyServices, CalculoImpostoVigenteStrategyServices>();
            services.AddScoped<ICalculoImpostoStrategyServices, CalculoImpostoReformaStrategyServices>();
            services.AddScoped<IFeatureFlagService, FeatureFlagService>();

            #endregion

            #region External Integrations 

            #endregion
            // Configuração do Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            services.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(dispose: true));
        }
    }
}
