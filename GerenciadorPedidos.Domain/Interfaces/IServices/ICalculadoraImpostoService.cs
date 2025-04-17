using GerenciadorPedidos.Domain.Entities.Objects;

namespace GerenciadorPedidos.Domain.Interfaces.IServices;

/// <summary>
/// Interface para o serviço de cálculo de impostos
/// </summary>
public interface ICalculadoraImpostoService
{
    /// <summary>
    /// Calcula o imposto para um pedido
    /// </summary>
    /// <param name="pedido">Pedido para cálculo do imposto</param>
    /// <returns>Valor do imposto calculado</returns>
    decimal CalcularImposto(Pedido pedido);
}
