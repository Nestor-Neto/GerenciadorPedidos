using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorPedidos.Domain.Entities.DataTransferObjects
{
    [SwaggerSchema(Title = "Item do pedido")]
    public class PedidoItemDTO
    {
        /// <summary>
        /// ID do produto
        /// </summary>
        [Required(ErrorMessage = "O ID do produto é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "O ID do produto deve ser maior que zero")]
        [DefaultValue(0)]
        [SwaggerSchema(Description = "ID do produto", Format = "int32")]
        [FromQuery(Name = "produtoId")]
        public int ProdutoId { get; set; }

        /// <summary>
        /// Quantidade do produto
        /// </summary>
        [Required(ErrorMessage = "A quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero")]
        [DefaultValue(0)]
        [SwaggerSchema(Description = "Quantidade do produto", Format = "int32")]
        [FromQuery(Name = "quantidade")]
        public int Quantidade { get; set; }

        /// <summary>
        /// Valor unitário do produto
        /// </summary>
        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser maior que zero")]
        [DefaultValue(0)]
        [SwaggerSchema(Description = "Valor unitário do produto", Format = "decimal")]
        [FromQuery(Name = "valor")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Inicializa uma nova instância do DTO de item de pedido com os valores especificados.
        /// </summary>
        /// <param name="produtoId">ID do produto</param>
        /// <param name="quantidade">Quantidade do produto</param>
        /// <param name="valor">Valor unitário do produto</param>
        public PedidoItemDTO(int produtoId, int quantidade, decimal valor)
        {
            ProdutoId = produtoId;
            Quantidade = quantidade;
            Valor = valor;
        }

        /// <summary>
        /// Inicializa uma nova instância vazia do DTO de item de pedido.
        /// Este construtor é necessário para serialização/desserialização e para o funcionamento do AutoMapper.
        /// </summary>
        public PedidoItemDTO() { }
    }
}
