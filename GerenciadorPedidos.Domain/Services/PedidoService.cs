using AutoMapper;
using GerenciadorPedidos.Domain.Entities.DataTransferObjects;
using GerenciadorPedidos.Domain.Entities.Objects;
using GerenciadorPedidos.Domain.Interfaces.IRepositories;
using GerenciadorPedidos.Domain.Interfaces.IServices;
using GerenciadorPedidos.Domain.Exceptions;
using GerenciadorPedidos.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace GerenciadorPedidos.Domain.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly ICalculadoraImpostoService _calculadoraImposto;
        private readonly ISistemaBService _sistemaBService;
        private readonly ILogger<PedidoService> _logger;
        private readonly IMapper _mapper;

        public PedidoService(
            IPedidoRepository pedidoRepository,
            ICalculadoraImpostoService calculadoraImposto,
            ISistemaBService sistemaBService,
            ILogger<PedidoService> logger,
            IMapper mapper)
        {
            _pedidoRepository = pedidoRepository;
            _calculadoraImposto = calculadoraImposto;
            _sistemaBService = sistemaBService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PedidoResponseDTO> CriarPedidoAsync(NovoPedidoDTO pedidoDto)
        {
            try
            {
                if (pedidoDto == null)
                    throw new DomainException("Pedido inválido!");

                var pedidoExistente = await _pedidoRepository.ObterPorPedidoIdAsync(pedidoDto.PedidoId);
                if (pedidoExistente != null)
                    throw new DomainException($"Já existe um pedido com o PedidoId {pedidoDto.PedidoId}.");

                var pedido = _mapper.Map<Pedido>(pedidoDto);
                await _pedidoRepository.AdicionarAsync(pedido);
                await _pedidoRepository.UnitOfWork.CommitAsync();

                return _mapper.Map<PedidoResponseDTO>(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<PedidoResponseDTO> ObterPedidoPorIdAsync(int id)
        {
            try
            {
                var pedido = await _pedidoRepository.ObterPorIdAsync(id);
                if (pedido == null)
                {
                    throw new DomainException($"Pedido com ID {id} não encontrado");
                }

                return _mapper.Map<PedidoResponseDTO>(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedido {Id}: {Message}", id, ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<PedidoResponseDTO>> ListarPedidosPorStatusAsync(EnumStatus status)
        {
            try
            {
                var pedidos = await _pedidoRepository.ObterPorStatusAsync(status);
                return _mapper.Map<IEnumerable<PedidoResponseDTO>>(pedidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar pedidos por status {Status}: {Message}", status, ex.Message);
                throw;
            }
        }
    }
} 