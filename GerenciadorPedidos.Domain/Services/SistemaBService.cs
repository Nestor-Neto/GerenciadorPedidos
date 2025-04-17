
using GerenciadorPedidos.Domain.Entities.Objects;
using GerenciadorPedidos.Domain.Interfaces.IServices;
using Microsoft.Extensions.Logging;

namespace GerenciadorPedidos.Domain.Services
{
    public class SistemaBService : ISistemaBService
    {

        private readonly ILogger<SistemaBService> _logger;

        public SistemaBService(ILogger<SistemaBService> logger)
        {
            _logger = logger;
        }

        public async Task EnviarPedidoAsync(Pedido pedido)
        {
            try
            {
                _logger.LogInformation("Iniciando envio do pedido {PedidoId} para o Sistema B", pedido.PedidoId);

                // Simula o envio para o Sistema B
                await Task.Delay(1000); // Simula uma chamada HTTP

                _logger.LogInformation("Pedido {PedidoId} enviado com sucesso para o Sistema B", pedido.PedidoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar pedido {PedidoId} para o Sistema B", pedido.PedidoId);
                throw;
            }
        }
    }
}
