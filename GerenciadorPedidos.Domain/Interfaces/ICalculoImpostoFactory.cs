namespace GerenciadorPedidos.Domain.Interfaces;

/// <summary>
/// Interface para a fábrica de cálculo de impostos
/// </summary>
public interface ICalculoImpostoFactory
{
    /// <summary>
    /// Cria uma estratégia de cálculo de imposto
    /// </summary>
    /// <param name="nome">Nome da estratégia</param>
    /// <returns>Estratégia de cálculo de imposto</returns>
    ICalculoImpostoStrategy CriarEstrategia(string nome);
} 