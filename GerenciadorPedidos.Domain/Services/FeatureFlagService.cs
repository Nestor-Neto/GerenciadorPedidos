using GerenciadorPedidos.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GerenciadorPedidos.Domain.Services;

/// <summary>
/// Serviço responsável por gerenciar as Feature Flags do sistema.
/// Controla a ativação/desativação de funcionalidades como a Reforma Tributária.
/// Exemplo de configuração: "FeatureFlags": { "ReformaTributaria": false }
/// </summary>
public class FeatureFlagService : IFeatureFlagService
{
    private readonly IConfiguration _configuration;
    private const string FeatureFlagsSection = "FeatureFlags";

    public FeatureFlagService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public bool IsEnabled(string featureName)
    {
        return _configuration.GetValue<bool>($"{FeatureFlagsSection}:{featureName}");
    }
} 