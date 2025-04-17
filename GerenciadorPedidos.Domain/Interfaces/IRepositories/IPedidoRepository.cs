using GerenciadorPedidos.Domain.Entities.Objects;
using GerenciadorPedidos.Domain.Enums;
using GerenciadorPedidos.Domain.Interfaces.IUnitOfWork;

namespace GerenciadorPedidos.Domain.Interfaces.IRepositories
{
    public interface IPedidoRepository
    {
        IUnitOfWork<Pedido> UnitOfWork { get; }
        Task<Pedido?> ObterPorIdAsync(int id);
        Task<Pedido?> ObterPorPedidoIdAsync(int pedidoId);
        Task<IEnumerable<Pedido>> ObterPorStatusAsync(EnumStatus status);
        Task<Pedido> AdicionarAsync(Pedido pedido);
        Task<bool> ExistePedidoComIdAsync(int pedidoId);
        Task AtualizarAsync(Pedido pedido);
    }
}
