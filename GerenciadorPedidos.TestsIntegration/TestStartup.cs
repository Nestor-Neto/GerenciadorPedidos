using GerenciadorPedidos.Domain.Interfaces.IRepositories;
using GerenciadorPedidos.Domain.Interfaces.IServices;
using GerenciadorPedidos.Domain.Services;
using GerenciadorPedidos.Infra.Context;
using GerenciadorPedidos.Infra.CrossCutting.IoC;
using GerenciadorPedidos.Infra.Repositories;
using GerenciadorPedidos.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorPedidos.TestsIntegration;

public class TestStartup
{
    public static WebApplicationFactory<Program> CreateTestFactory()
    {
        return new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices((context, services) =>
                {
                    // Configurar banco de dados in-memory
                    services.AddDbContext<PedidoDbContext>(options =>
                        options.UseInMemoryDatabase($"PedidosDb_{Guid.NewGuid()}"));

                    // Configurar repositórios
                    services.AddScoped<IPedidoRepository, PedidoRepository>();

                    // Configurar serviços
                    services.AddScoped<IPedidoService, PedidoService>();

                    // Configurar AutoMapper
                    services.AddAutoMapper(typeof(TestStartup).Assembly);
                });

                builder.ConfigureAppConfiguration((context, config) =>
                {
                    // Configurar configurações específicas para teste
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"FeatureFlags:ReformaTributaria", "false"}
                    });
                });
            });
    }
} 