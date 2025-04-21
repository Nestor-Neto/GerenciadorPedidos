using GerenciadorPedidos.Domain.Exceptions;
using GerenciadorPedidos.Domain.Enums;

namespace GerenciadorPedidos.Domain.Entities.Objects
{
    /// <summary>
    /// Representa um pedido no sistema
    /// </summary>
    public class Pedido
    {
        public int Id { get; private set; }
        public int PedidoId { get; private set; }
        public int ClienteId { get; private set; }
        public decimal Imposto { get; private set; }
        public EnumStatus Status { get; private set; }
        
        /// <summary>
        /// Data e hora de criação do pedido em UTC
        /// Usado para controle e filtros de período
        /// </summary>
        public DateTime DataCriacao { get; private set; }


        private readonly List<PedidoItem> _itens;
        public IReadOnlyCollection<PedidoItem> Itens => _itens.AsReadOnly();

        protected Pedido()
        {
            _itens = new List<PedidoItem>();
            Status = EnumStatus.Criado;
            DataCriacao = DateTime.UtcNow;
            
        }

        public Pedido(int pedidoId, int clienteId) : this()
        {
            if (pedidoId <= 0)
                throw new DomainException("O ID do pedido deve ser maior que zero");

            if (clienteId <= 0)
                throw new DomainException("O ID do cliente deve ser maior que zero");

            PedidoId = pedidoId;
            ClienteId = clienteId;
        }

        public void AdicionarItem(PedidoItem item)
        {
            if (item == null)
                throw new DomainException("O item não pode ser nulo");

            _itens.Add(item);
             
        }

        /// <summary>
        /// Atualiza o valor do imposto do pedido
        /// </summary>
        /// <param name="valorImposto">Valor do imposto calculado</param>
        public void AtualizarImposto(decimal valorImposto)
        {
            Imposto = valorImposto;
             
        }

    }
}
