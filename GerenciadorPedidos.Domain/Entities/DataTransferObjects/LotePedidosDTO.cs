// GerenciadorPedidos.Domain/Entities/DataTransferObjects/LotePedidosDTO.cs
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace GerenciadorPedidos.Domain.Entities.DataTransferObjects
{
    /// <summary>
    /// DTO para recebimento de lote de pedidos
    /// </summary>
    [SwaggerSchema(Title = "Dados do lote de pedidos")]
    public class LotePedidosDTO
    {
        public LotePedidosDTO()
        {
            Pedidos = new List<NovoPedidoDTO>();
        }

        /// <summary>
        /// Lista de pedidos a serem processados em lote
        /// </summary>
        [Required(ErrorMessage = "A lista de pedidos é obrigatória")]
        [MinLength(1, ErrorMessage = "O lote deve conter pelo menos um pedido")]
        [SwaggerSchema(Description = "Lista de pedidos a serem processados")]
        public List<NovoPedidoDTO> Pedidos { get; set; }
    }
}