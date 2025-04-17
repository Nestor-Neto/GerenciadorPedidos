using System.Text.Json.Serialization;

namespace GerenciadorPedidos.Domain.Enums
{
    /// <summary>
    /// Enum que representa os possíveis status de um pedido
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnumStatus
    {
        /// <summary>
        /// Pedido criado
        /// </summary>
        Criado = 1,

        /// <summary>
        /// Pedido em processamento
        /// </summary>
        EmProcessamento = 2,

        /// <summary>
        /// Pedido enviado
        /// </summary>
        Enviado = 3,

        /// <summary>
        /// Pedido entregue
        /// </summary>
        Entregue = 4,

        /// <summary>
        /// Pedido cancelado
        /// </summary>
        Cancelado = 5
    }
} 