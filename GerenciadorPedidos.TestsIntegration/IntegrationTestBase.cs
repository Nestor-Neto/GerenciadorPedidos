using GerenciadorPedidos.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GerenciadorPedidos.TestsIntegration;

/// <summary>
/// Classe base para testes de integração que fornece um ambiente isolado com banco de dados.
/// 
/// Principais funcionalidades:
/// 1. Configuração do Ambiente de Teste:
///    - Configura um banco de dados In-memory para cada teste
///    - Inicializa o contexto do Entity Framework (PedidoDbContext)
/// 
/// 2. Isolamento dos Testes:
///    - Cada teste roda em seu próprio banco de dados In-memory
///    - O banco é criado no início do teste e destruído ao final
///    - Garante que os testes não interfiram uns nos outros
/// 
/// 3. Gerenciamento do Ciclo de Vida:
///    - Implementa IAsyncLifetime para controlar o ciclo de vida dos testes
///    - InitializeAsync: Cria o banco
///    - DisposeAsync: Limpa o banco
/// 
/// 4. Injeção de Dependências:
///    - Configura o ServiceProvider com todas as dependências necessárias
///    - Permite que os testes acessem os serviços através do container DI
/// 
/// Benefícios:
/// - Testes Rápidos: Usa banco de dados In-memory para execução rápida
/// - Isolamento: Cada teste tem seu próprio ambiente limpo
/// - Reutilização: A configuração comum é feita uma única vez
/// - Manutenibilidade: Mudanças na configuração são feitas em um único lugar
/// - Confiabilidade: Os testes refletem o comportamento real do sistema
/// </summary>
public abstract class IntegrationTestBase : IAsyncLifetime
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly PedidoDbContext DbContext;
    protected readonly IConfiguration Configuration;

    /// <summary>
    /// Inicializa uma nova instância da classe base de testes de integração.
    /// Configura o banco de dados In-memory e o ServiceProvider com as dependências necessárias.
    /// </summary>
    protected IntegrationTestBase()
    {
        Configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"FeatureFlags:ReformaTributaria", "false"}
            })
            .Build();

        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();
        DbContext = ServiceProvider.GetRequiredService<PedidoDbContext>();
    }

    /// <summary>
    /// Configura os serviços necessários para os testes de integração.
    /// Sobrescreva este método para adicionar configurações específicas em classes derivadas.
    /// </summary>
    /// <param name="services">Coleção de serviços para configuração</param>
    protected virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IConfiguration>(Configuration);
        services.AddDbContext<PedidoDbContext>(options =>
            options.UseInMemoryDatabase($"PedidosDb_{Guid.NewGuid()}"));
    }

    /// <summary>
    /// Inicializa o ambiente de teste:
    /// 1. Cria o banco de dados
    /// </summary>
    public async Task InitializeAsync()
    {
        await DbContext.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Limpa o ambiente de teste:
    /// 1. Remove o banco de dados
    /// </summary>
    public async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
    }
} 