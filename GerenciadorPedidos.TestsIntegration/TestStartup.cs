using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GerenciadorPedidos.TestsIntegration;

public class TestStartup
{
    public static WebApplicationFactory<TestStartup> CreateTestFactory()
    {
        return new WebApplicationFactory<TestStartup>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices((context, services) =>
                {
                    // Configurar serviços específicos para teste aqui
                });

                builder.ConfigureAppConfiguration((context, config) =>
                {
                    // Configurar configurações específicas para teste aqui
                });
            });
    }
} 