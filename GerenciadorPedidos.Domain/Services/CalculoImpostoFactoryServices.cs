using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Intrinsics.X86;
using GerenciadorPedidos.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorPedidos.Domain.Services
{
    public class CalculoImpostoFactoryServices : ICalculoImpostoFactoryServices
    {
        /// <summary>
        /// Container de injeção de dependência que permite criar instâncias de serviços registrados.
        /// Usado para criar dinamicamente as estratégias de cálculo de imposto em tempo de execução.
        /// O IServiceProvider mantém o ciclo de vida dos objetos e suas dependências.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Dicionário que mapeia os nomes das estratégias para seus respectivos tipos.
        /// - Chave (string): Nome identificador da estratégia (ex: "Vigente", "Reforma")
        /// - Valor (Type): Tipo da classe que implementa a estratégia
        /// 
        /// Este dicionário funciona como um registro de estratégias disponíveis,
        /// permitindo a criação dinâmica da estratégia correta com base no nome.
        /// 
        /// Importante: Todas as estratégias devem implementar ICalculoImpostoStrategyServices.
        /// </summary>
        private readonly Dictionary<string, Type> _strategies;

        public CalculoImpostoFactoryServices(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _strategies = new Dictionary<string, Type>
            {
                // Mapeia a estratégia vigente (30% de imposto)
                { "Vigente", typeof(CalculoImpostoVigenteStrategyServices) },
                
                // Mapeia a estratégia da reforma tributária (20% de imposto)
                { "Reforma", typeof(CalculoImpostoReformaStrategyServices) }
            };
        }

        public ICalculoImpostoStrategyServices CriarEstrategia(string nome)
        {
            //O método TryGetValue é uma forma segura de buscar um valor em um dicionário
            //retornando false se a chave não existir, em vez de lançar uma exceção
            //O parâmetro out var strategyType armazena o valor encontrado se a chave existir
            if (!_strategies.TryGetValue(nome, out var strategyType))
            {
                throw new ArgumentException($"Estratégia '{nome}' não encontrada.");
            }

            return (ICalculoImpostoStrategyServices)_serviceProvider.GetRequiredService(strategyType);
        }
    }
} 