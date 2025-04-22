// GerenciadorPedidos.Domain/Entities/DataTransferObjects/ResultadoProcessamentoLoteDTO.cs
using GerenciadorPedidos.Domain.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace GerenciadorPedidos.Domain.Entities.DataTransferObjects
{
    /// <summary>
    /// DTO que representa o resultado do processamento em lote de pedidos
    /// </summary>
    [SwaggerSchema(Title = "Resultado do processamento em lote")]
    public class ResultadoProcessamentoLoteDTO
    {
        /// <summary>
        /// Construtor que inicializa as listas
        /// </summary>
        public ResultadoProcessamentoLoteDTO()
        {
            Sucessos = new List<PedidoCriadoDTO>();
            Erros = new List<ErroProcessamentoDTO>();
        }

        /// <summary>
        /// Lista de pedidos processados com sucesso
        /// </summary>
        [SwaggerSchema(Description = "Pedidos processados com sucesso")]
        public List<PedidoCriadoDTO> Sucessos { get; set; }

        /// <summary>
        /// Lista de erros ocorridos durante o processamento
        /// </summary>
        [SwaggerSchema(Description = "Erros ocorridos durante o processamento")]
        public List<ErroProcessamentoDTO> Erros { get; set; }

        /// <summary>
        /// Total de pedidos processados no lote
        /// </summary>
        [SwaggerSchema(Description = "Total de pedidos processados")]
        public int TotalProcessado { get; set; }

        /// <summary>
        /// Total de pedidos processados com sucesso
        /// </summary>
        [SwaggerSchema(Description = "Total de pedidos com sucesso")]
        public int TotalSucesso { get; set; }

        /// <summary>
        /// Total de pedidos que apresentaram erro
        /// </summary>
        [SwaggerSchema(Description = "Total de pedidos com erro")]
        public int TotalErros { get; set; }
    }

    /// <summary>
    /// DTO que representa um erro ocorrido durante o processamento de um pedido
    /// </summary>
    public class ErroProcessamentoDTO
    {
        /// <summary>
        /// ID do pedido que apresentou erro
        /// </summary>
        [Required(ErrorMessage = "O ID do pedido é obrigatório")]
        [SwaggerSchema(Description = "ID do pedido que apresentou erro")]
        public int PedidoId { get; set; }

        /// <summary>
        /// Mensagem detalhada do erro
        /// </summary>
        [Required(ErrorMessage = "A mensagem de erro é obrigatória")]
        [SwaggerSchema(Description = "Mensagem detalhada do erro")]
        public string Mensagem { get; set; } = string.Empty;
    }
}