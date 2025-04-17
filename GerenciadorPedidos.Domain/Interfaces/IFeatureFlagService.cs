namespace GerenciadorPedidos.Domain.Interfaces;

/// <summary>
/// Interface para gerenciamento de feature flags
/// </summary>
public interface IFeatureFlagService
{
    /// <summary>
    /// Verifica se uma feature flag está ativa
    /// </summary>
    /// <param name="featureName">Nome da feature flag</param>
    /// <returns>True se a feature flag estiver ativa, false caso contrário</returns>
    bool IsEnabled(string featureName);
} 