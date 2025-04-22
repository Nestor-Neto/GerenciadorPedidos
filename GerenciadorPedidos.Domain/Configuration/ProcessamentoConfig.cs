using System.ComponentModel.DataAnnotations;

namespace GerenciadorPedidos.Domain.Configuration
{
    /// <summary>
    /// Configurações para o processamento em lote de pedidos
    /// </summary>
    public class ProcessamentoConfig
    {
        /// <summary>
        /// Tamanho padrão do lote de processamento
        /// </summary>
        [Range(100, 5000)]
        public int TamanhoLotePadrao { get; set; } = 1000;

        /// <summary>
        /// Número máximo de processamentos simultâneos
        /// </summary>
        [Range(1, 20)]
        public int MaxProcessamentosSimultaneos { get; set; } = 5;

        /// <summary>
        /// Delay em milissegundos entre o processamento de lotes
        /// </summary>
        [Range(0, 1000)]
        public int DelayEntreLotesMs { get; set; } = 100;

        /// <summary>
        /// Tamanho máximo permitido para um lote
        /// </summary>
        [Range(1000, 10000)]
        public int TamanhoLoteMaximo { get; set; } = 5000;

        /// <summary>
        /// Tamanho mínimo permitido para um lote
        /// </summary>
        [Range(50, 1000)]
        public int TamanhoLoteMinimo { get; set; } = 100;
    }
} 