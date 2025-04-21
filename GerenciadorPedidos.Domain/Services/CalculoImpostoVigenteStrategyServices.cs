using GerenciadorPedidos.Domain.Interfaces;
using GerenciadorPedidos.Domain.Entities.Objects;

namespace GerenciadorPedidos.Domain.Services;

/// <summary>
/// Estratégia de cálculo de imposto vigente.
/// Calcula o imposto com base na alíquota atual.
/// </summary>
public class CalculoImpostoVigenteStrategyServices : ICalculoImpostoStrategyServices
{
    private const decimal ALIQUOTA_IMPOSTO = 0.30m; // 30%

    public string Nome => "Vigente";

    /// <summary>
    /// Calcula o imposto para um pedido baseado na alíquota vigente
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