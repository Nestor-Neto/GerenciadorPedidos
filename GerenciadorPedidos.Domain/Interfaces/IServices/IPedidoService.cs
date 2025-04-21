using GerenciadorPedidos.Domain.Entities.DataTransferObjects;
using GerenciadorPedidos.Domain.Enums;

namespace GerenciadorPedidos.Domain.Interfaces.IServices
{
    public interface IPedidoService
    {
        Task<PedidoResponseDTO> CriarPedidoAsync(NovoPedidoDTO pedidoDto);
        Task<PedidoResponseDTO> ObterPedidoPorIdAsync(int id);
        Task<IEnumerable<PedidoResponseDTO>> ListarPedidosPorStatusAsync(EnumStatus status);
    }
} 