using GerenciadorPedidos.Domain.Entities.Objects;

namespace GerenciadorPedidos.Domain.Interfaces;

/// <summary>
/// Interface que define a estratégia de cálculo de impostos
/// </summary>
public interface ICalculoImpostoStrategy
{
    /// <summary>
    /// Nome da estratégia de cálculo
    /// </summary>
    string Nome { get; }

    /// <summary>
    /// Calcula o valor do imposto para um pedido
    /// </summary>
    /// <param name="pedido">Pedido para cálculo do imposto</param>
    /// <returns>Valor do imposto calculado</returns>
    decimal CalcularImposto(Pedido pedido);
} 