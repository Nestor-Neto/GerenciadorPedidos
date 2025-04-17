using GerenciadorPedidos.Domain.Exceptions;

namespace GerenciadorPedidos.Domain.Entities.Objects
{
    /// <summary>
    /// Representa um item de um pedido
    /// </summary>
    public class PedidoItem
    {
        public int Id { get; private set; }
        public int ProdutoId { get; private set; }
        public int Quantidade { get; private set; }
        public decimal Valor { get; private set; }
        public int PedidoId { get; private set; }
        public Pedido? Pedido { get; private set; }

        protected PedidoItem() { }

        /// <summary>
        /// Validação de dados do pedido item
        /// </summary>
        public PedidoItem(int produtoId, int quantidade, decimal valor)
        {
            if (quantidade <= 0)
                throw new DomainException("A quantidade deve ser maior que zero");

            if (valor <= 0)
                throw new DomainException("O valor deve ser maior que zero");

            ProdutoId = produtoId;
            Quantidade = quantidade;
            Valor = valor;
        }

        public decimal CalcularSubtotal()
        {
            return Quantidade * Valor;
        }
    }
}
