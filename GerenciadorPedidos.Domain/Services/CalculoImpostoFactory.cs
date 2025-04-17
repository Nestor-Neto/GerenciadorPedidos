using System;
using System.Collections.Generic;
using GerenciadorPedidos.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorPedidos.Domain.Services;

/// <summary>
/// Factory responsável pela criação das estratégias de cálculo de imposto.
/// Cria a estratégia apropriada baseada na Feature Flag "ReformaTributaria".
/// Exemplo de configuração: "FeatureFlags": { "ReformaTributaria": false }
/// </summary>
public class CalculoImpostoFactory : ICalculoImpostoFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, Type> _strategies;

    public CalculoImpostoFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _strategies = new Dictionary<string, Type>
        {
            { "VigenteStrategy", typeof(CalculoImpostoVigenteStrategyServices) },
            { "ReformaStrategy", typeof(CalculoImpostoReformaStrategyServices) }
        };
    }

    /// <summary>
    /// Cria uma estratégia de cálculo de imposto
    /// </summary>
    /// <param name="nome">Nome da estratégia</param>
    /// <returns>Estratégia de cálculo de imposto</returns>
    /// <exception cref="ArgumentException">Se a estratégia não existir</exception>
    public ICalculoImpostoStrategy CriarEstrategia(string nome)
    {
        if (!_strategies.TryGetValue(nome, out var strategyType))
        {
            throw new ArgumentException($"Estratégia '{nome}' não encontrada.");
        }

        return (ICalculoImpostoStrategy)_serviceProvider.GetRequiredService(strategyType);
    }
} 