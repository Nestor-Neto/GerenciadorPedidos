using GerenciadorPedidos.Domain.Entities.Objects;
using GerenciadorPedidos.Domain.Interfaces;
using GerenciadorPedidos.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace GerenciadorPedidos.Domain.Services
{
    /// <summary>
    /// Serviço principal de cálculo de impostos.
    /// Utiliza Feature Flags para determinar qual estratégia de cálculo aplicar.
    /// Exemplo de configuração: "FeatureFlags": { "ReformaTributaria": false }
    /// </summary>
    public class CalculadoraImpostoService : ICalculadoraImpostoService
    {
        private readonly ICalculoImpostoFactory _calculoImpostoFactory;
        private readonly IFeatureFlagService _featureFlagService;
        private readonly ILogger<CalculadoraImpostoService> _mockLogger;
        private const string FeatureFlagReformaTributaria = "ReformaTributaria";

        public CalculadoraImpostoService(
            ICalculoImpostoFactory calculoImpostoFactory,
            IFeatureFlagService featureFlagService,
            ILogger<CalculadoraImpostoService> logger)
        {
            _calculoImpostoFactory = calculoImpostoFactory ?? throw new ArgumentNullException(nameof(calculoImpostoFactory));
            _featureFlagService = featureFlagService ?? throw new ArgumentNullException(nameof(featureFlagService));
            _mockLogger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Calcula o imposto para um pedido
        /// </summary>
        /// <param name="pedido">Pedido para cálculo do imposto</param>
        /// <returns>Valor do imposto calculado</returns>
        public decimal CalcularImposto(Pedido pedido)
        {
            // O sistema verifica a feature flag "ReformaTributaria" no appsettings.json
            bool reformaTributariaAtiva = _featureFlagService.IsEnabled(FeatureFlagReformaTributaria);
            
            string estrategiaNome = reformaTributariaAtiva ? "Reforma" : "Vigente";
            
            _mockLogger.LogInformation("Calculando imposto para o pedido {PedidoId} usando a estratégia {Estrategia} (Feature Flag: {FeatureFlag})", 
                pedido.PedidoId, estrategiaNome, reformaTributariaAtiva);
            
            var estrategia = _calculoImpostoFactory.CriarEstrategia(estrategiaNome);
            var imposto = estrategia.CalcularImposto(pedido);
            
            _mockLogger.LogInformation("Imposto calculado para o pedido {PedidoId}: {Imposto}", 
                pedido.PedidoId, imposto);
            
            return imposto;
        }
    }
}
