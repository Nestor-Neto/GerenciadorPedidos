using GerenciadorPedidos.Domain.Enums;

namespace GerenciadorPedidos.Domain.Entities.DataTransferObjects
{
    public class PedidoResponseDTO
    {
        public PedidoResponseDTO()
        {
            Itens = new List<PedidoItemDTO>();
        }
        /// <summary>
        /// ID interno do pedido
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID do pedido no sistema externo
        /// </summary>
        public int PedidoId { get; set; }

        /// <summary>
        /// ID do cliente
        /// </summary>
        public int ClienteId { get; set; }

        /// <summary>
        /// Valor do imposto calculado
        /// </summary>
        public decimal Imposto { get; set; }


        /// <summary>
        /// Data e hora de criação do pedido
        /// </summary>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Lista de itens do pedido
        /// </summary>
        public List<PedidoItemDTO> Itens { get; set; }

        /// <summary>
        /// Status atual do pedido
        /// </summary>
        public EnumStatus Status { get; set; }
    }
}
