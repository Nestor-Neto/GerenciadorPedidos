using GerenciadorPedidos.Domain.Entities.Objects;

namespace GerenciadorPedidos.Domain.Interfaces
{
    /// <summary>
    /// Interface que define o valor do imposto a ser calculado
    /// </summary>
    public interface ICalculoImpostoStrategyServices
    {
        /// <summary>
        /// Calcula o valor do imposto para um pedido
        /// </summary>
        /// <param name="pedido">Pedido para cálculo do imposto</param>
        /// <returns>Valor do imposto calculado</returns>
        decimal CalcularImposto(Pedido pedido);
    }
} 