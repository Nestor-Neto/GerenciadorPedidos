using GerenciadorPedidos.Domain.Entities.Objects;

namespace GerenciadorPedidos.Domain.Interfaces.IServices
{
    public interface ISistemaBService
    {
        Task EnviarPedidoAsync(Pedido pedido);
    }
}
