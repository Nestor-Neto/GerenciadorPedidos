using Bogus;
using FluentAssertions;
using GerenciadorPedidos.API.Controller;
using GerenciadorPedidos.Domain.Entities.DataTransferObjects;
using GerenciadorPedidos.Domain.Entities.Objects;
using GerenciadorPedidos.Domain.Exceptions;
using GerenciadorPedidos.Domain.Interfaces.IServices;
using GerenciadorPedidos.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using Newtonsoft.Json;

namespace GerenciadorPedidos.TestsUnit.Controllers
{
    public class PedidosControllerTests
    {
        private readonly IPedidoService _pedidoService;
        private readonly ILogger<PedidosController> _logger;
        private readonly PedidosController _controller;
        private readonly Faker _faker;

        public PedidosControllerTests()
        {
            _pedidoService = Substitute.For<IPedidoService>();
            _logger = Substitute.For<ILogger<PedidosController>>();
            _controller = new PedidosController(_pedidoService, _logger);
            _faker = new Faker();
        }

        // Classe para representar a resposta de erro
        private class ErrorResponse
        {
            public string Message { get; set; } = string.Empty;
            public string? Details { get; set; }
        }

        [Fact]
        // Teste de sucesso: Verifica se a criação de pedido com dados válidos retorna CreatedAtAction
        public async Task CriarPedido_ComDadosValidos_RetornaCreatedAtAction()
        {
            // Arrange
            var pedidoDto = new NovoPedidoDTO
            {
                PedidoId = _faker.Random.Int(1, 1000),
                ClienteId = _faker.Random.Int(1, 1000),
                Itens = new List<PedidoItemDTO>
                {
                    new PedidoItemDTO
                    {
                        ProdutoId = _faker.Random.Int(1, 1000),
                        Quantidade = _faker.Random.Int(1, 10),
                        Valor = _faker.Random.Decimal(10, 1000)
                    }
                }
            };

            var pedidoResponse = new PedidoResponseDTO
            {
                Id = _faker.Random.Int(1, 1000),
                Status = EnumStatus.Criado
            };

            _pedidoService.CriarPedidoAsync(pedidoDto).Returns(pedidoResponse);

            // Act
            var result = await _controller.CriarPedido(pedidoDto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdAtResult = result as CreatedAtActionResult;
            createdAtResult.Should().NotBeNull();
            createdAtResult!.StatusCode.Should().Be(201);
            createdAtResult.ActionName.Should().Be(nameof(_controller.ObterPedidoPorId));
            createdAtResult.RouteValues.Should().NotBeNull();
            createdAtResult.RouteValues!["id"].Should().Be(pedidoResponse.Id);
            
            var response = createdAtResult.Value as PedidoCriadoDTO;
            response.Should().NotBeNull();
            response!.Id.Should().Be(pedidoResponse.Id);
            response.Status.Should().Be(pedidoResponse.Status);
        }

        [Fact]
        // Teste de erro: Verifica se a criação de pedido duplicado retorna Conflict
        public async Task CriarPedido_ComPedidoDuplicado_RetornaConflict()
        {
            // Arrange
            var pedidoDto = new NovoPedidoDTO
            {
                PedidoId = _faker.Random.Int(1, 1000),
                ClienteId = _faker.Random.Int(1, 1000),
                Itens = new List<PedidoItemDTO>
                {
                    new PedidoItemDTO
                    {
                        ProdutoId = _faker.Random.Int(1, 1000),
                        Quantidade = _faker.Random.Int(1, 10),
                        Valor = _faker.Random.Decimal(10, 1000)
                    }
                }
            };

            var mensagemErro = $"Já existe um pedido com o PedidoId {pedidoDto.PedidoId}.";
            _pedidoService.CriarPedidoAsync(pedidoDto).Returns(Task.FromException<PedidoResponseDTO>(
                new DomainException(mensagemErro)));

            // Act
            var result = await _controller.CriarPedido(pedidoDto);

            // Assert
            result.Should().BeOfType<ConflictObjectResult>();
            var conflictResult = result as ConflictObjectResult;
            conflictResult.Should().NotBeNull();
            conflictResult!.StatusCode.Should().Be(409);
            var response = conflictResult.Value as ErrorResponse;
            response.Should().NotBeNull();
            response!.Message.Should().Be(mensagemErro);
        }

        [Fact]
        // Teste de erro: Verifica se a criação de pedido com dados inválidos retorna BadRequest
        public async Task CriarPedido_ComDadosInvalidos_RetornaBadRequest()
        {
            // Arrange
            var pedidoDto = new NovoPedidoDTO
            {
                PedidoId = _faker.Random.Int(1, 1000),
                ClienteId = _faker.Random.Int(1, 1000),
                Itens = new List<PedidoItemDTO>
                {
                    new PedidoItemDTO
                    {
                        ProdutoId = _faker.Random.Int(1, 1000),
                        Quantidade = _faker.Random.Int(1, 10),
                        Valor = _faker.Random.Decimal(10, 1000)
                    }
                }
            };

            var mensagemErro = "Pedido inválido!";
            _pedidoService.CriarPedidoAsync(pedidoDto).Returns(Task.FromException<PedidoResponseDTO>(
                new DomainException(mensagemErro)));

            // Act
            var result = await _controller.CriarPedido(pedidoDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
            var response = badRequestResult.Value as ErrorResponse;
            response.Should().NotBeNull();
            response!.Message.Should().Be(mensagemErro);
        }

        [Fact]
        // Teste de erro: Verifica se a criação de pedido com erro interno retorna InternalServerError
        public async Task CriarPedido_ComErroInterno_RetornaInternalServerError()
        {
            // Arrange
            var pedidoDto = new NovoPedidoDTO
            {
                PedidoId = _faker.Random.Int(1, 1000),
                ClienteId = _faker.Random.Int(1, 1000),
                Itens = new List<PedidoItemDTO>
                {
                    new PedidoItemDTO
                    {
                        ProdutoId = _faker.Random.Int(1, 1000),
                        Quantidade = _faker.Random.Int(1, 10),
                        Valor = _faker.Random.Decimal(10, 1000)
                    }
                }
            };

            var mensagemErro = "Erro interno";
            _pedidoService.CriarPedidoAsync(pedidoDto).Returns(Task.FromException<PedidoResponseDTO>(
                new Exception(mensagemErro)));

            // Act
            var result = await _controller.CriarPedido(pedidoDto);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult!.StatusCode.Should().Be(500);
            var response = objectResult.Value as ErrorResponse;
            response.Should().NotBeNull();
            response!.Message.Should().Be("Erro interno ao processar o pedido");
            response.Details.Should().Be(mensagemErro);
        }

        [Fact]
        // Teste de sucesso: Verifica se a obtenção de pedido por ID válido retorna Ok
        public async Task ObterPedidoPorId_ComIdValido_RetornaOk()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var pedidoResponse = new PedidoResponseDTO
            {
                Id = id,
                PedidoId = _faker.Random.Int(1, 1000),
                ClienteId = _faker.Random.Int(1, 1000),
                Status = EnumStatus.Criado,
                Imposto = _faker.Random.Decimal(10, 1000),
                Itens = new List<PedidoItemDTO>
                {
                    new PedidoItemDTO
                    {
                        ProdutoId = _faker.Random.Int(1, 1000),
                        Quantidade = _faker.Random.Int(1, 10),
                        Valor = _faker.Random.Decimal(10, 1000)
                    }
                }
            };

            _pedidoService.ObterPedidoPorIdAsync(id).Returns(pedidoResponse);

            // Act
            var result = await _controller.ObterPedidoPorId(id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            var response = okResult.Value as PedidoResponseDTO;
            response.Should().NotBeNull();
            response!.Id.Should().Be(pedidoResponse.Id);
            response.PedidoId.Should().Be(pedidoResponse.PedidoId);
            response.ClienteId.Should().Be(pedidoResponse.ClienteId);
            response.Status.Should().Be(pedidoResponse.Status);
            response.Imposto.Should().Be(pedidoResponse.Imposto);
            response.Itens.Should().BeEquivalentTo(pedidoResponse.Itens);
        }

        [Fact]
        // Teste de erro: Verifica se a obtenção de pedido por ID inválido retorna NotFound
        public async Task ObterPedidoPorId_ComIdInvalido_RetornaNotFound()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            _pedidoService.ObterPedidoPorIdAsync(id).Returns((PedidoResponseDTO)null!);

            // Act
            var result = await _controller.ObterPedidoPorId(id);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(404);
            var response = notFoundResult.Value as ErrorResponse;
            response.Should().NotBeNull();
            response!.Message.Should().Be($"Pedido com ID {id} não encontrado");
        }

        [Fact]
        // Teste de erro: Verifica se a obtenção de pedido com erro interno retorna InternalServerError
        public async Task ObterPedidoPorId_ComErroInterno_RetornaInternalServerError()
        {
            // Arrange
            var id = _faker.Random.Int(1, 1000);
            var mensagemErro = "Erro interno";
            _pedidoService.ObterPedidoPorIdAsync(id).Returns(Task.FromException<PedidoResponseDTO>(
                new Exception(mensagemErro)));

            // Act
            var result = await _controller.ObterPedidoPorId(id);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult!.StatusCode.Should().Be(500);
            var response = objectResult.Value as ErrorResponse;
            response.Should().NotBeNull();
            response!.Message.Should().Be("Erro interno ao processar a requisição");
            response.Details.Should().Be(mensagemErro);
        }

        [Fact]
        // Teste de sucesso: Verifica se a listagem de pedidos por status retorna Ok
        public async Task ListarPedidosPorStatus_ComStatusValido_RetornaOk()
        {
            // Arrange
            var status = EnumStatus.Criado;
            var pedidosResponse = new List<PedidoResponseDTO>
            {
                new PedidoResponseDTO
                {
                    Id = _faker.Random.Int(1, 1000),
                    PedidoId = _faker.Random.Int(1, 1000),
                    ClienteId = _faker.Random.Int(1, 1000),
                    Status = status,
                    Imposto = _faker.Random.Decimal(10, 1000),
                    Itens = new List<PedidoItemDTO>
                    {
                        new PedidoItemDTO
                        {
                            ProdutoId = _faker.Random.Int(1, 1000),
                            Quantidade = _faker.Random.Int(1, 10),
                            Valor = _faker.Random.Decimal(10, 1000)
                        }
                    }
                }
            };

            _pedidoService.ListarPedidosPorStatusAsync(status).Returns(pedidosResponse);

            // Act
            var result = await _controller.ListarPedidosPorStatus(status);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            var response = okResult.Value as IEnumerable<PedidoResponseDTO>;
            response.Should().NotBeNull();
            response!.Should().BeEquivalentTo(pedidosResponse);
        }

        [Fact]
        // Teste de erro: Verifica se a listagem de pedidos por status com erro interno retorna InternalServerError
        public async Task ListarPedidosPorStatus_ComErroInterno_RetornaInternalServerError()
        {
            // Arrange
            var status = EnumStatus.Criado;
            var mensagemErro = "Erro interno";
            _pedidoService.ListarPedidosPorStatusAsync(status).Returns(Task.FromException<IEnumerable<PedidoResponseDTO>>(
                new Exception(mensagemErro)));

            // Act
            var result = await _controller.ListarPedidosPorStatus(status);

            // Assert
            result.Should().BeOfType<ObjectResult>();
            var objectResult = result as ObjectResult;
            objectResult.Should().NotBeNull();
            objectResult!.StatusCode.Should().Be(500);
            var response = objectResult.Value as ErrorResponse;
            response.Should().NotBeNull();
            response!.Message.Should().Be("Erro interno ao processar a requisição");
            response.Details.Should().Be(mensagemErro);
        }
    }
} 