using GerenciadorPedidos.Domain.Interfaces;
using GerenciadorPedidos.Domain.Entities.Objects;

namespace GerenciadorPedidos.Domain.Services;

/// <summary>
/// Estratégia de cálculo de imposto da reforma.
/// Calcula o imposto com base na nova alíquota da reforma.
/// </summary>
public class CalculoImpostoReformaStrategyServices : ICalculoImpostoStrategyServices
{
    private const decimal ALIQUOTA_IMPOSTO = 0.20m; // 20%

    /// <summary>
    /// Calcula o imposto para um pedido baseado na alíquota da reforma
    /// </summary>
    /// <param name="pedido">Pedido para cálculo do imposto</param>
    /// <returns>Valor do imposto calculado</returns>
    public decimal CalcularImposto(Pedido pedido)
    {
        if (pedido?.Itens == null || !pedido.Itens.Any())
            return 0;

        var valorTotal = pedido.Itens.Sum(item => item.Valor * item.Quantidade);
        return valorTotal * ALIQUOTA_IMPOSTO;
    }
} 