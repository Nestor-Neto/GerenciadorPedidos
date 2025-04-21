using GerenciadorPedidos.Domain.Entities.Objects;
using GerenciadorPedidos.Domain.Interfaces;
using GerenciadorPedidos.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace GerenciadorPedidos.Domain.Services
{
    /// <summary>
    /// Serviço principal responsável pelo cálculo de impostos.
    /// Implementa o padrão Strategy para alternar entre diferentes formas de cálculo.
    /// Utiliza Feature Flags para determinar qual estratégia de cálculo aplicar.
    /// </summary>
    public class CalculadoraImpostoService : ICalculadoraImpostoService
    {
        /// <summary>
        /// Factory responsável por criar as estratégias de cálculo de imposto.
        /// Utiliza injeção de dependência para instanciar as implementações corretas.
        /// </summary>
        private readonly ICalculoImpostoFactoryServices _calculoImpostoFactoryServices;

        /// <summary>
        /// Serviço que gerencia as feature flags do sistema.
        /// Permite alternar entre diferentes comportamentos em tempo de execução.
        /// </summary>
        private readonly IFeatureFlagService _featureFlagService;

        /// <summary>
        /// Logger para registro de operações e diagnóstico do sistema.
        /// </summary>
        private readonly ILogger<CalculadoraImpostoService> _mockLogger;

        /// <summary>
        /// Nome da feature flag que controla a estratégia de cálculo.
        /// Quando true: usa reforma tributária (20%)
        /// Quando false: usa regra vigente (30%)
        /// </summary>
        private const string FeatureFlagReformaTributaria = "ReformaTributaria";

        /// <summary>
        /// Construtor que inicializa o serviço com suas dependências.
        /// </summary>
        /// <param name="calculoImpostoFactory">Factory para criar estratégias de cálculo</param>
        /// <param name="featureFlagService">Serviço de feature flags</param>
        /// <param name="logger">Serviço de logging</param>
        /// <exception cref="ArgumentNullException">Lançada se alguma dependência for nula</exception>
        public CalculadoraImpostoService(
            ICalculoImpostoFactoryServices calculoImpostoFactory,
            IFeatureFlagService featureFlagService,
            ILogger<CalculadoraImpostoService> logger)
        {
            _calculoImpostoFactoryServices = calculoImpostoFactory ?? 
                throw new ArgumentNullException(nameof(calculoImpostoFactory));
            
            _featureFlagService = featureFlagService ?? 
                throw new ArgumentNullException(nameof(featureFlagService));
            
            _mockLogger = logger ?? 
                throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Calcula o imposto para um pedido baseado na configuração atual do sistema.
        /// O cálculo é determinado pela feature flag "ReformaTributaria":
        /// - Se ativa: usa alíquota de 20% (reforma tributária)
        /// - Se inativa: usa alíquota de 30% (regra vigente)
        /// </summary>
        /// <param name="pedido">Pedido para cálculo do imposto</param>
        /// <returns>Valor do imposto calculado</returns>
        /// <exception cref="ArgumentException">
        /// Lançada se a estratégia de cálculo não for encontrada
        /// </exception>
        public decimal CalcularImposto(Pedido pedido)
        {
            // Verifica se a reforma tributária está ativa
            bool reformaTributariaAtiva = _featureFlagService
                .IsEnabled(FeatureFlagReformaTributaria);
            
            // Define qual estratégia usar com base na feature flag
            string estrategiaNome = reformaTributariaAtiva ? "Reforma" : "Vigente";
            
            // Registra log da estratégia selecionada
            _mockLogger.LogInformation(
                "Calculando imposto para o pedido {PedidoId} usando a estratégia {Estrategia} " + 
                "(Feature Flag: {FeatureFlag})", 
                pedido.PedidoId, 
                estrategiaNome, 
                reformaTributariaAtiva);
            
            try
            {
                // Obtém a estratégia de cálculo através da factory
                var estrategia = _calculoImpostoFactoryServices
                    .CriarEstrategia(estrategiaNome);

                // Executa o cálculo do imposto
                var imposto = estrategia.CalcularImposto(pedido);
                
                // Registra log do resultado
                _mockLogger.LogInformation(
                    "Imposto calculado para o pedido {PedidoId}: {Imposto}", 
                    pedido.PedidoId, 
                    imposto);
                
                return imposto;
            }
            catch (Exception ex)
            {
                // Registra log de erro em caso de falha
                _mockLogger.LogError(
                    ex,
                    "Erro ao calcular imposto para o pedido {PedidoId} usando estratégia {Estrategia}",
                    pedido.PedidoId,
                    estrategiaNome);
                
                throw;
            }
        }
    }
}
