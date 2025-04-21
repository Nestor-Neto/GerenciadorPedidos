using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using GerenciadorPedidos.Domain.Entities.DataTransferObjects;
using GerenciadorPedidos.Domain.Enums;
using GerenciadorPedidos.API;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace GerenciadorPedidos.TestsIntegration.Controllers
{
    public class PedidosControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;

        public PedidosControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = TestStartup.CreateTestFactory();
            _client = _factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Fact]
        public async Task CriarPedido_ComDadosValidos_RetornaCreated()
        {
            // Arrange
            var pedidoDto = new NovoPedidoDTO
            {
                PedidoId = 1,
                ClienteId = 1,
                Itens = new List<PedidoItemDTO>
                {
                    new PedidoItemDTO
                    {
                        ProdutoId = 1,
                        Quantidade = 2,
                        Valor = 100
                    }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(pedidoDto),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/pedidos", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var responseContent = await response.Content.ReadAsStringAsync();
            var pedidoCriado = JsonSerializer.Deserialize<PedidoCriadoDTO>(responseContent, _jsonOptions);
            pedidoCriado.Should().NotBeNull();
            pedidoCriado!.Status.Should().Be(EnumStatus.Criado);
        }

        [Fact]
        public async Task CriarPedido_ComPedidoDuplicado_RetornaConflict()
        {
            // Arrange
            var pedidoDto = new NovoPedidoDTO
            {
                PedidoId = 1,
                ClienteId = 1,
                Itens = new List<PedidoItemDTO>
                {
                    new PedidoItemDTO
                    {
                        ProdutoId = 1,
                        Quantidade = 2,
                        Valor = 100
                    }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(pedidoDto),
                Encoding.UTF8,
                "application/json");

            // Act
            await _client.PostAsync("/api/pedidos", content); // Primeira chamada
            var response = await _client.PostAsync("/api/pedidos", content); // Segunda chamada com mesmo PedidoId

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task ObterPedidoPorId_ComIdValido_RetornaOk()
        {
            // Arrange
            var pedidoDto = new NovoPedidoDTO
            {
                PedidoId = 2,
                ClienteId = 1,
                Itens = new List<PedidoItemDTO>
                {
                    new PedidoItemDTO
                    {
                        ProdutoId = 1,
                        Quantidade = 2,
                        Valor = 100
                    }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(pedidoDto),
                Encoding.UTF8,
                "application/json");

            var createResponse = await _client.PostAsync("/api/pedidos", content);
            var pedidoCriado = JsonSerializer.Deserialize<PedidoCriadoDTO>(
                await createResponse.Content.ReadAsStringAsync(),
                _jsonOptions);

            // Act
            var response = await _client.GetAsync($"/api/pedidos/{pedidoCriado!.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            var pedido = JsonSerializer.Deserialize<PedidoResponseDTO>(responseContent, _jsonOptions);
            pedido.Should().NotBeNull();
            pedido!.Id.Should().Be(pedidoCriado.Id);
            pedido.PedidoId.Should().Be(pedidoDto.PedidoId);
            pedido.ClienteId.Should().Be(pedidoDto.ClienteId);
            pedido.Status.Should().Be(EnumStatus.Criado);
        }

        [Fact]
        public async Task ObterPedidoPorId_ComIdInvalido_RetornaNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/pedidos/999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ListarPedidosPorStatus_ComStatusValido_RetornaOk()
        {
            // Arrange
            var pedidoDto = new NovoPedidoDTO
            {
                PedidoId = 3,
                ClienteId = 1,
                Itens = new List<PedidoItemDTO>
                {
                    new PedidoItemDTO
                    {
                        ProdutoId = 1,
                        Quantidade = 2,
                        Valor = 100
                    }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(pedidoDto),
                Encoding.UTF8,
                "application/json");

            await _client.PostAsync("/api/pedidos", content);

            // Act
            var response = await _client.GetAsync("/api/pedidos/por-status?status=Criado");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseContent = await response.Content.ReadAsStringAsync();
            var pedidos = JsonSerializer.Deserialize<List<PedidoResponseDTO>>(responseContent, _jsonOptions);
            pedidos.Should().NotBeNull();
            pedidos!.Should().NotBeEmpty();
            pedidos.Should().Contain(p => p.PedidoId == pedidoDto.PedidoId);
        }
    }
} 