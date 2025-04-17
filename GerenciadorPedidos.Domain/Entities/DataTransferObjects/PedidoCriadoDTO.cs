using GerenciadorPedidos.Domain.Enums;

namespace GerenciadorPedidos.Domain.Entities.DataTransferObjects;

/// <summary>
/// DTO que representa o retorno simplificado de um pedido criado
/// </summary>
public class PedidoCriadoDTO
{
    /// <summary>
    /// Identificador único do pedido
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Status atual do pedido
    /// </summary>
    public EnumStatus Status { get; set; }
} 