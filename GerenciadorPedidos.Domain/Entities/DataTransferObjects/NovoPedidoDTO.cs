using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Swashbuckle.AspNetCore.Annotations;

namespace GerenciadorPedidos.Domain.Entities.DataTransferObjects
{
    [SwaggerSchema(Title = "Dados do novo pedido")]
    public class NovoPedidoDTO
    {
        public NovoPedidoDTO()
        {
            Itens = new List<PedidoItemDTO>();
        }
        /// <summary>
        /// ID do pedido no sistema externo
        /// </summary>
        [Required(ErrorMessage = "O ID do pedido é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID do pedido deve ser maior que zero")]
        [DefaultValue(0)]
        [SwaggerSchema(Description = "ID do pedido no sistema externo", Format = "int32")]
        public int PedidoId { get; set; }

        /// <summary>
        /// ID do cliente
        /// </summary>
        [Required(ErrorMessage = "O ID do cliente é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID do cliente deve ser maior que zero")]
        [DefaultValue(0)]
        [SwaggerSchema(Description = "ID do cliente", Format = "int32")]
        public int ClienteId { get; set; }

        /// <summary>
        /// Lista de itens do pedido
        /// </summary>
        [Required(ErrorMessage = "O pedido deve conter pelo menos um item")]
        [MinLength(1, ErrorMessage = "O pedido deve conter pelo menos um item")]
        [SwaggerSchema(Description = "Lista de itens do pedido")]
        public List<PedidoItemDTO> Itens { get; set; }
    }
}
