using GerenciadorPedidos.Domain.Entities.DataTransferObjects;
using GerenciadorPedidos.Domain.Enums;

namespace GerenciadorPedidos.Domain.Interfaces.IServices
{
    public interface IPedidoService
    {
        Task<PedidoResponseDTO> CriarPedidoAsync(NovoPedidoDTO pedidoDto);
        Task<PedidoResponseDTO> ObterPedidoPorIdAsync(int id);
        Task<IEnumerable<PedidoResponseDTO>> ListarPedidosPorStatusAsync(EnumStatus status);
        /// <summary>
        /// Processa uma lista de pedidos em lote
        /// </summary>
        /// <param name="pedidos">Lista de pedidos a serem processados</param>
        /// <returns>Resultado do processamento com sucessos e falhas</returns>
        Task<ResultadoProcessamentoLoteDTO> ProcessarPedidosAsync(List<NovoPedidoDTO> pedidos);
    }
} 