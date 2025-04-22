using GerenciadorPedidos.Domain.Entities.DataTransferObjects;
using GerenciadorPedidos.Domain.Interfaces.IServices;
using GerenciadorPedidos.Domain.Exceptions;
using GerenciadorPedidos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GerenciadorPedidos.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PedidosController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly ILogger<PedidosController> _logger;
       
        public PedidosController(
            IPedidoService pedidoService,
            ILogger<PedidosController> logger)
        {
            _pedidoService = pedidoService;
            _logger = logger;
        }

        /// <summary>
        /// Cria um novo pedido
        /// </summary>
        /// <param name="pedido">Dados do novo pedido</param>
        /// <returns>Pedido criado</returns>
        /// <response code="201">Pedido criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="409">Pedido já existe</response>
        [HttpPost]
        [ProducesResponseType(typeof(PedidoCriadoDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CriarPedido([FromBody] NovoPedidoDTO pedido)
        {
            if (pedido == null)
            {
                _logger.LogWarning("Tentativa de criar pedido com dados nulos");
                return BadRequest(new { message = "Os dados do pedido são obrigatórios" });
            }

            try
            {
                _logger.LogInformation("Iniciando criação do pedido {PedidoId}", pedido.PedidoId);

                var pedidoCriado = await _pedidoService.CriarPedidoAsync(pedido);
                var response = new PedidoCriadoDTO
                {
                    Id = pedidoCriado.Id,
                    Status = pedidoCriado.Status
                };

                return CreatedAtAction(nameof(ObterPedidoPorId), new { id = response.Id }, response);
            }
            catch (DomainException ex) when (ex.Message.Contains("já existe", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning(ex, "Pedido duplicado: {Message}", ex.Message);
                return Conflict(new { message = ex.Message });
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Erro de validação: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro interno ao criar pedido: {Message}", ex.Message);
                return StatusCode(500, new { message = "Erro interno ao processar o pedido", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém um pedido pelo ID
        /// </summary>
        /// <param name="id">ID do pedido</param>
        /// <returns>Dados do pedido</returns>
        /// <response code="200">Pedido encontrado</response>
        /// <response code="404">Pedido não encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PedidoResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPedidoPorId(int id)
        {
            try
            {
                _logger.LogInformation("Buscando pedido por ID {Id}", id);
                var pedido = await _pedidoService.ObterPedidoPorIdAsync(id);
                
                if (pedido == null)
                {
                    return NotFound(new { message = $"Pedido com ID {id} não encontrado" });
                }

                return Ok(pedido);
            }
            catch (DomainException ex)
            {
                _logger.LogWarning(ex, "Pedido não encontrado: {Message}", ex.Message);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter pedido {Id}: {Message}", id, ex.Message);
                return StatusCode(500, new { message = "Erro interno ao processar a requisição", details = ex.Message });
            }
        }

        /// <summary>
        /// Lista pedidos por status
        /// </summary>
        /// <param name="status">Status dos pedidos</param>
        /// <returns>Lista de pedidos</returns>
        /// <response code="200">Lista de pedidos</response>
        [HttpGet("por-status")]
        [ProducesResponseType(typeof(IEnumerable<PedidoResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarPedidosPorStatus([FromQuery] EnumStatus status)
        {
            try
            {
                _logger.LogInformation("Listando pedidos com status {Status}", status);
                var pedidos = await _pedidoService.ListarPedidosPorStatusAsync(status);
                return Ok(pedidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar pedidos por status {Status}: {Message}", status, ex.Message);
                return StatusCode(500, new { message = "Erro interno ao processar a requisição", details = ex.Message });
            }
        }

        /// <summary>
        /// Cria um lote de pedidos
        /// </summary>
        /// <param name="lotePedidos">Lista de pedidos a serem criados em lote</param>
        /// <returns>Resultado do processamento em lote com sucessos e falhas</returns>
        /// <response code="200">Lote processado com sucesso</response>
        /// <response code="400">Dados inválidos no lote</response>
        /// <response code="500">Erro interno no processamento</response>
        [HttpPost("lote")]
        [ProducesResponseType(typeof(ResultadoProcessamentoLoteDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarLotePedidos([FromBody] LotePedidosDTO lotePedidos)
        {
            if (lotePedidos?.Pedidos == null || !lotePedidos.Pedidos.Any())
            {
                _logger.LogWarning("Tentativa de criar lote de pedidos com dados nulos ou vazios");
                return BadRequest(new { message = "A lista de pedidos é obrigatória e não pode estar vazia" });
            }

            try
            {
                _logger.LogInformation("Iniciando criação de lote de pedidos");
                var resultado = await _pedidoService.ProcessarPedidosAsync(lotePedidos.Pedidos);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar lote de pedidos: {Message}", ex.Message);
                return StatusCode(500, new { message = "Erro interno ao processar o lote de pedidos", details = ex.Message });
            }
        }
    }
}
