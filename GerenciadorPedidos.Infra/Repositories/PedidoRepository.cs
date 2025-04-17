using GerenciadorPedidos.Domain.Entities.Objects;
using GerenciadorPedidos.Domain.Interfaces.IRepositories;
using GerenciadorPedidos.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GerenciadorPedidos.Domain.Enums;
using GerenciadorPedidos.Domain.Interfaces.IUnitOfWork;
using GerenciadorPedidos.Infra.UnitOfWork;

namespace GerenciadorPedidos.Infra.Repositories
{
    public class PedidoRepository : IPedidoRepository
    {
        private readonly PedidoDbContext _context;
        private readonly ILogger<PedidoRepository> _logger;
        private readonly IUnitOfWork<Pedido> _unitOfWork;

        public IUnitOfWork<Pedido> UnitOfWork => _unitOfWork;

        public PedidoRepository(PedidoDbContext context, ILogger<PedidoRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = new UnitOfWork<Pedido>(context);
        }
        public async Task<Pedido> AdicionarAsync(Pedido pedido)
        {
            if (pedido == null)
                throw new ArgumentNullException(nameof(pedido));

            _logger.LogInformation("Adicionando pedido {PedidoId} para o cliente {ClienteId}", pedido.PedidoId, pedido.ClienteId);
            await _context.Pedidos.AddAsync(pedido);
            await _unitOfWork.CommitAsync();
            return pedido;
        }

        public async Task AtualizarAsync(Pedido pedido)
        {
            if (pedido == null)
                throw new ArgumentNullException(nameof(pedido));

            _logger.LogInformation("Atualizando pedido {PedidoId}", pedido.PedidoId);
            _context.Pedidos.Update(pedido);
            await _unitOfWork.CommitAsync();
        }

        public async Task<bool> ExistePedidoComIdAsync(int pedidoId)
        {
            return await _context.Pedidos.AnyAsync(p => p.PedidoId == pedidoId);
        }

        public async Task<Pedido?> ObterPorIdAsync(int id)
        {
            _logger.LogInformation("Buscando pedido por ID {Id}", id);
            return await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Pedido?> ObterPorPedidoIdAsync(int pedidoId)
        {
            _logger.LogInformation("Buscando pedido por PedidoId {PedidoId}", pedidoId);
            return await _context.Pedidos
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.PedidoId == pedidoId);
        }

        public async Task<IEnumerable<Pedido>> ObterPorStatusAsync(EnumStatus status)
        {
            _logger.LogInformation("Buscando pedidos com status {Status}", status);
            return await _context.Pedidos
                .Include(p => p.Itens)
                .Where(p => p.Status == status)
                .ToListAsync();
        }
    }
}
