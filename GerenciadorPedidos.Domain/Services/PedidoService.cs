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
    /// <summary>
    /// Serviço principal responsável pelo gerenciamento de pedidos no sistema.
    /// Implementa as regras de negócio relacionadas a pedidos, incluindo:
    /// - Criação de pedidos com validação de duplicidade
    /// - Cálculo de impostos usando diferentes estratégias
    /// - Integração com Sistema B
    /// - Gerenciamento do ciclo de vida do pedido
    /// </summary>
    public class PedidoService : IPedidoService
    {
        /// <summary>
        /// Repositório para operações de persistência de pedidos.
        /// </summary>
        private readonly IPedidoRepository _pedidoRepository;

        /// <summary>
        /// Serviço responsável pelo cálculo de impostos.
        /// Utiliza diferentes estratégias baseadas em feature flags.
        /// </summary>
        private readonly ICalculadoraImpostoService _calculadoraImposto;

        /// <summary>
        /// Serviço de integração com o Sistema B.
        /// Responsável por enviar pedidos processados.
        /// </summary>
        private readonly ISistemaBService _sistemaBService;

        /// <summary>
        /// Serviço de logging para rastreamento de operações.
        /// Utiliza logging estruturado para melhor diagnóstico.
        /// </summary>
        private readonly ILogger<PedidoService> _logger;

        /// <summary>
        /// Serviço de mapeamento entre DTOs e entidades.
        /// Configurado via AutoMapper profiles.
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// Construtor que inicializa o serviço com todas as dependências necessárias.
        /// </summary>
        /// <param name="pedidoRepository">Repositório de pedidos</param>
        /// <param name="calculadoraImposto">Serviço de cálculo de impostos</param>
        /// <param name="sistemaBService">Serviço de integração com Sistema B</param>
        /// <param name="logger">Serviço de logging</param>
        /// <param name="mapper">Serviço de mapeamento</param>
        /// <exception cref="ArgumentNullException">
        /// Lançada se qualquer dependência for nula
        /// </exception>
        public PedidoService(
            IPedidoRepository pedidoRepository,
            ICalculadoraImpostoService calculadoraImposto,
            ISistemaBService sistemaBService,
            ILogger<PedidoService> logger,
            IMapper mapper)
        {
            _pedidoRepository = pedidoRepository ?? 
                throw new ArgumentNullException(nameof(pedidoRepository));
            _calculadoraImposto = calculadoraImposto ?? 
                throw new ArgumentNullException(nameof(calculadoraImposto));
            _sistemaBService = sistemaBService ?? 
                throw new ArgumentNullException(nameof(sistemaBService));
            _logger = logger ?? 
                throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? 
                throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Cria um novo pedido no sistema, aplicando todas as regras de negócio.
        /// </summary>
        /// <param name="pedidoDto">Dados do novo pedido</param>
        /// <returns>Pedido criado com dados completos</returns>
        /// <exception cref="DomainException">
        /// Lançada em caso de violação de regras de negócio:
        /// </exception>
        public async Task<PedidoResponseDTO> CriarPedidoAsync(NovoPedidoDTO pedidoDto)
        {
            try
            {
                // Validação inicial
                if (pedidoDto == null)
                {
                    //Lançada em caso de violação de regras de negócio
                    _logger.LogWarning("Tentativa de criar pedido com dados nulos");
                    throw new DomainException("Pedido inválido!");
                }

                // Verificação de duplicidade
                var pedidoExistente = await _pedidoRepository
                    .ObterPorPedidoIdAsync(pedidoDto.PedidoId);
                
                if (pedidoExistente != null)
                {
                    _logger.LogWarning(
                        "Tentativa de criar pedido duplicado. PedidoId: {PedidoId}", 
                        pedidoDto.PedidoId);
                    //Lançada em caso de violação de regras de negócio
                    throw new DomainException(
                        $"Já existe um pedido com o PedidoId {pedidoDto.PedidoId}.");
                }

                // Mapeamento para entidade
                var pedido = _mapper.Map<Pedido>(pedidoDto);
                
                // Cálculo de impostos
                decimal imposto = _calculadoraImposto.CalcularImposto(pedido);
                pedido.AtualizarImposto(imposto);

                _logger.LogInformation(
                    "Imposto calculado para o pedido {PedidoId}: {Imposto}", 
                    pedido.PedidoId, 
                    imposto);

                // Persistência no banco
                await _pedidoRepository.AdicionarAsync(pedido);
                await _pedidoRepository.UnitOfWork.CommitAsync();

                // Integração com Sistema B
                try 
                {
                    await _sistemaBService.EnviarPedidoAsync(pedido);
                    
                    _logger.LogInformation(
                        "Pedido {PedidoId} enviado com sucesso para o Sistema B", 
                        pedido.PedidoId);
                }
                catch (Exception ex)
                {
                    // Loga o erro mas não impede a criação do pedido
                    _logger.LogError(
                        ex,
                        "Erro ao enviar pedido {PedidoId} para o Sistema B: {Message}", 
                        pedido.PedidoId,
                        ex.Message);
                }

                // Retorno do pedido criado
                return _mapper.Map<PedidoResponseDTO>(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar pedido: {Message}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Obtém um pedido pelo seu ID interno.
        /// </summary>
        /// <param name="id">ID interno do pedido</param>
        /// <returns>Dados do pedido encontrado</returns>
        /// <exception cref="DomainException">
        /// Lançada quando o pedido não é encontrado
        /// </exception>
        public async Task<PedidoResponseDTO> ObterPedidoPorIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Buscando pedido por ID {Id}", id);
                
                var pedido = await _pedidoRepository.ObterPorIdAsync(id);
                if (pedido == null)
                {
                    _logger.LogWarning("Pedido não encontrado. ID: {Id}", id);
                    //Lançada em caso de violação de regras de negócio
                    throw new DomainException($"Pedido com ID {id} não encontrado");
                }

                return _mapper.Map<PedidoResponseDTO>(pedido);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Erro ao obter pedido {Id}: {Message}", 
                    id, 
                    ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Lista pedidos por status
        /// </summary>
        /// <param name="status">Status dos pedidos a serem listados</param>
        /// <returns>Lista de pedidos no status especificado</returns>
        public async Task<IEnumerable<PedidoResponseDTO>> ListarPedidosPorStatusAsync(EnumStatus status)
        {
            try
            {
                _logger.LogInformation("Listando pedidos com status {Status}", status);

                var pedidos = await _pedidoRepository.ObterPorStatusAsync(status);
                var pedidosDto = _mapper.Map<IEnumerable<PedidoResponseDTO>>(pedidos);

                _logger.LogInformation(
                    "Encontrados {Quantidade} pedidos com status {Status}", 
                    pedidosDto.Count(), 
                    status);

                return pedidosDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex, 
                    "Erro ao listar pedidos por status {Status}: {Message}", 
                    status, 
                    ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Valida os dados básicos de um pedido.
        /// </summary>
        /// <param name="pedido">Pedido a ser validado</param>
        /// <exception cref="DomainException">
        /// Lançada quando há dados inválidos
        /// </exception>
        private void ValidarPedido(Pedido pedido)
        {
            if (pedido.PedidoId <= 0)
                throw new DomainException("ID do pedido inválido");

            if (pedido.ClienteId <= 0)
                throw new DomainException("ID do cliente inválido");

            if (!pedido.Itens.Any())
                throw new DomainException("Pedido deve ter pelo menos um item");
        }

        /// <summary>
        /// Valida se um pedido pode ser modificado com base em seu status.
        /// </summary>
        /// <param name="status">Status atual do pedido</param>
        /// <returns>true se o pedido pode ser modificado</returns>
        private bool PodeModificarPedido(EnumStatus status)
        {
            return status == EnumStatus.Criado || 
                   status == EnumStatus.EmProcessamento;
        }
    }
} 