# GerenciadorPedidos

## 📋 Descrição
Sistema de gerenciamento de pedidos desenvolvido em .NET Core, seguindo boas práticas de desenvolvimento e arquitetura limpa. O projeto implementa um sistema RESTful para gerenciamento de pedidos, com suporte a testes automatizados e logging estruturado.

## 🚀 Tecnologias e Frameworks

### Framework Principal
- .NET 8.0

### Bibliotecas Principais
- **AutoMapper**: Para mapeamento objeto-objeto
- **FluentValidation**: Para validação de dados
- **Serilog**: Para logging estruturado
- **Entity Framework Core**: Para acesso a dados
- **Microsoft.EntityFrameworkCore.InMemory**: Para testes com banco em memória

### Bibliotecas de Teste
- **xUnit**: Framework de testes
- **FluentAssertions**: Para assertions mais legíveis
- **Bogus**: Para geração de dados fake
- **NSubstitute**: Para mocking
- **Testcontainers**: Para testes de integração com containers

## 🏗️ Arquitetura

O projeto segue a arquitetura em camadas, com separação clara de responsabilidades:

### Camadas

1. **API (GerenciadorPedidos.API)**
- **Controllers**
     - `PedidosController`: Endpoints REST para gerenciamento de pedidos
       - **Endpoints**:
         - `POST /api/pedidos`: Criação de pedido
         - `GET /api/pedidos/{id}`: Obtenção de pedido por ID
         - `GET /api/pedidos/por-status`: Listagem de pedidos por status
       - **Características**:
         - Validação de entrada via FluentValidation
         - Tratamento de exceções via filtros globais
         - Respostas HTTP padronizadas
         - Documentação via Swagger
- **Logging**
- **Configuração Serilog**:
       - Formato estruturado (JSON)
       - Níveis: Information, Warning, Error
       - Sinks: Console, File
- **Middleware de Logging**:
       - Log de requisições HTTP
       - Log de respostas
       - Log de exceções   
- **Properties**
     - **appsettings.json**:
       - Configurações de conexão
       - Configurações de logging
     - **appsettings.Development.json**:
       - Configurações específicas para desenvolvimento
       - Logging detalhado
       - Swagger habilitado
- **Swagger/OpenAPI**
     - **Configuração**:
       - Documentação automática dos endpoints
       - Exemplos de requisição/resposta

2. **Domain (GerenciadorPedidos.Domain)**
   - **Entidades de Domínio**
     - `Pedido`: Entidade principal com validações de PedidoId único e ClienteId obrigatório
     - `PedidoItem`: Entidade de item com validações de quantidade e valor

   - **DataTransferObjects**
     - `NovoPedidoDTO`: Dados para criação
     - `PedidoResponseDTO`: Dados para resposta
     - `PedidoItemDTO`: Dados de item

   - **Objects**
     - `Pedido`: Representa um pedido no sistema
     - `PedidoItem`: Representa um item de um pedido

   - **Enums**
     - `EnumStatus`: Estados do pedido (Criado, EmAndamento, Concluido, Cancelado)

   - **Exceptions**
     - `DomainException`: Base para exceções de domínio, pode ser usada quando houver, violações de regras de negócio

   - **Interfaces**
     - `IPedidoRepository`: Operações CRUD de pedidos
     - `IPedidoItemRepository`: Operações CRUD de itens
     - `IPedidoService`: Operações de negócio de pedidos
     - `IPedidoItemService`: Operações de negócio de itens
     - `ICalculoImpostoFactory`: Operações de negócio, Estratégia de cálculo de imposto
     - `ICalculoImpostoStrategy`: Operações de negócio, valor do imposto a ser calculado
     - `IUnitOfWork`: Gerenciamento de transações
     - `IFeatureFlagService`: Operações de negócio, Verifica se uma feature flag, qual o status.

   - **AutoMapperSetup**
     - Mapeamento entre entidades e DTOs
     - Configuração de profiles

   - **Serviços e Regras de Negócio**
     - `CalculadoraImpostoService`
       - Serviço principal responsável pelo cálculo de impostos
       - Implementa o padrão Strategy para alternar entre diferentes formas de cálculo.
       - Utiliza Feature Flags para determinar qual estratégia de cálculo aplicar.
     - `CalculoImpostoFactoryServices`
       - Serviço responsável por criar instâncias de estratégias de cálculo de imposto.
       - Implementa o padrão Factory Method para criar diferentes estratégias de cálculo.
       - Seleciona estratégia baseada na configuração (dicionário com as estratégias disponíveis), Este dicionário funciona como um registro de estratégias disponíveis, permitindo a adição de novas estratégias sem modificar o código existente.
     - `CalculoImpostoReformaStrategyServices`
       - Serviço responsável por estratégia de cálculo de imposto da reforma, Calcula o imposto com base na nova alíquota da reforma.
       - Implementa cálculo de 20% de imposto
       - Regra da reforma tributária
     - `CalculoImpostoVigenteStrategyServices`
       - Serviço responsável por estratégia de cálculo de imposto da reforma, Calcula o imposto com base na alíquota atual.
       - Implementa cálculo de 30% de imposto
       - Regra do Cálculo em vigor(Vigente)
     -`FeatureFlagService`
       - Serviço responsável por gerenciar as Feature Flags do sistema.
       - Implementa o padrão Feature Toggle para controlar funcionalidades em tempo de execução.
       - Este serviço permite: Ativar/desativar funcionalidades sem deploy. 
       - Lançamento gradual de features
      - `PedidoService`
       - Serviço principal responsável pelo gerenciamento de pedidos no sistema.
       - Implementa as regras de negócio relacionadas a pedidos.
       - Criação de pedidos com validação de duplicidade
       - Cálculo de impostos usando diferentes estratégias
       - Integração com Sistema B
       - Gerenciamento do ciclo de vida do pedido
       - `CriarPedido`: Cria um novo pedido no sistema, aplicando todas as regras de negócio
       - `ObterPedidoPorId`: Obtém um pedido pelo seu ID interno.
       - `ListarPedidosPorStatus`: Lista todos os pedidos com um determinado status.
      - `ProcessamentoPedidoService`
       - Serviço responsável pelo processamento em lote de pedidos, Implementa estratégias para lidar com alto volume de dados.
       - `SemaphoreSlim`: Semáforo para controlar o número máximo de processamentos simultâneos, Evita sobrecarga do sistema e do banco de dados.
       - `ObterLotePedidos`: Obtém um lote de pedidos para processamento e aplica filtros conforme configuração (lote de pedidos com status {Status} TamanhoPadrao={TamanhoPadrao}, Min={Min}, Max={Max})
       - `ProcessarPedidos`: Processa uma lista de pedidos em lotes. Implementa controle automático de processamento baseado na configuração

       ##### Factory (Fábrica de Estratégias)
       **Explicação da Factory:**
      - `_serviceProvider`: Container de injeção de dependência que permite criar instâncias de serviços registrados.
         Responsável por criar instâncias das estratégias com suas dependências. 
      - `_strategies`: Mapeia nomes de estratégias para suas implementações
      - Uso do padrão Factory Method para criar estratégias dinamicamente
      - Validação de estratégias inexistentes para evitar erros em runtime


     - **CalculoImpostoReformaStrategyServices**
       

     - **CalculoImpostoVigenteStrategyServices**
       - Implementa cálculo de 30% de imposto
       - Regra atual vigente

     - **FeatureFlagService**
       - Gerencia flags de feature
       - Controla qual estratégia de cálculo usar
       - Controle de cálculo de impostos, O sistema utiliza feature flags para controlar qual estratégia de cálculo será utilizada: 
       via appsettings.json `ReformaTributaria`
       - Cálculo padrão (30% quando `false`)
       - Cálculo reforma (20% quando `true`)
       - Implementação via padrão Strategy
       **Como funciona:**
       - 1. Sistema verifica a configuração da feature flag: `bool reformaAtiva = _featureFlagService.IsEnabled("ReformaTributaria")`
       - 2. Seleciona a estratégia apropriada : `string estrategiaNome = reformaAtiva ? "Reforma" : "Vigente"`
       - 3. Factory cria a instância da estratégia: `var estrategia = _calculoImpostoFactoryServices.CriarEstrategia(estrategiaNome)`
       - 4. Retorna a estratégia calcula o imposto : `return estrategia.CalcularImposto(pedido)`

     - **GerenciadorPedidos**
       - Orquestra operações de pedido
       - Aplica validações de negócio: Verifica existência do `PedidoId` antes da criação
       - Retorna erro HTTP 409 (Conflict) se duplicado
        - Índice único na coluna `PedidoId` no banco de dados

     - **SistemaBService**
       - Integração com sistema externo
       - Comunicação assíncrona: `EnviarPedidoAsync(Pedido pedido)`
       - Tratamento de falhas
       - Logging detalhado de operações


3. **Infrastructure (GerenciadorPedidos.Infra)**
   - Implementação de repositórios
   - Contexto do Entity Framework
   - Configurações do AutoMapper
   - Implementações de serviço

4. **CrossCutting (GerenciadorPedidos.Infra.CrossCutting)**
   - Injeção de dependência
   - Configurações globais
   - Extensões


4. **Testes
1. **Testes Unitários (GerenciadorPedidos.TestsUnit)**
   - Testes de controllers
   - Testes de serviços
   - Testes de repositórios

2. **Testes de Integração (GerenciadorPedidos.TestsIntegration)**
   - Testes de API
   - Testes com banco em memória
   - Testes de fluxos completos

## 📝 Logging

O projeto utiliza Serilog para logging estruturado, com as seguintes características:
- Logging em diferentes níveis (Information, Warning, Error)
- Formato estruturado (JSON)
- Rotações de arquivo de log
- Logging de exceções detalhado

## 🗄️ Banco de Dados

- **Produção**: SQL Server
- **Testes**: In-memory database (Microsoft.EntityFrameworkCore.InMemory)
- **Migrations**: Entity Framework Core Migrations

## 🔄 Controle de Versão

### Git Flow
- `main`: Código em produção
- `develop`: Desenvolvimento
- `feature/*`: Novas funcionalidades
- `hotfix/*`: Correções urgentes
- `release/*`: Preparação para release

### Commits Semânticos
- `feat`: Nova funcionalidade
- `fix`: Correção de bug
- `docs`: Documentação
- `style`: Formatação
- `refactor`: Refatoração
- `test`: Testes
- `chore`: Manutenção

## ✨ Boas Práticas

### Clean Code
- Nomes descritivos
- Funções pequenas e focadas
- DRY (Don't Repeat Yourself)
- Comentários significativos

### SOLID
- **S**: Single Responsibility Principle (Princípio da Responsabilidade Única)
  - `PedidosController`: Responsável apenas por endpoints REST
  - `PedidoService`: Responsável apenas por regras de negócio de pedidos
  - `PedidoRepository`: Responsável apenas por persistência de dados
  - `CalculadoraImpostoService`: Responsável apenas por cálculos de impostos
  - Cada estratégia de cálculo (`CalculoImpostoVigenteStrategy`, `CalculoImpostoReformaStrategy`) tem uma única responsabilidade

- **O**: Open/Closed Principle (Princípio Aberto/Fechado)
  - Sistema de cálculo de impostos extensível sem modificação
  - Novas estratégias de cálculo podem ser adicionadas implementando `ICalculoImpostoStrategy`
  - `CalculoImpostoFactory` permite adicionar novas estratégias sem modificar código existente
  - Feature flags permitem alternar entre implementações sem modificar código

- **L**: Liskov Substitution Principle (Princípio da Substituição de Liskov)
  - Todas as estratégias de cálculo (`CalculoImpostoVigenteStrategy`, `CalculoImpostoReformaStrategy`) podem ser usadas onde `ICalculoImpostoStrategy` é esperado
  - `PedidoRepository` e `PedidoItemRepository` são intercambiáveis através de suas interfaces
  - DTOs mantêm contrato consistente com suas entidades correspondentes

- **I**: Interface Segregation Principle (Princípio da Segregação de Interface)
  - `IPedidoRepository`: Interface específica para operações de pedido
  - `IPedidoItemRepository`: Interface específica para operações de item
  - `ICalculoImpostoStrategy`: Interface específica para cálculo de impostos
  - `IUnitOfWork`: Interface específica para transações
  - Interfaces pequenas e coesas

- **D**: Dependency Inversion Principle (Princípio da Inversão de Dependência)
  - Controllers dependem de interfaces de serviço (`IPedidoService`)
  - Serviços dependem de interfaces de repositório (`IPedidoRepository`)
  - `CalculadoraImpostoService` depende da interface `ICalculoImpostoStrategy`
  - Injeção de dependência via construtor em todas as classes
  - Configuração de DI no `CrossCutting/IoC/DependencyInjection.cs`

## 🧪 Testes

### Testes Unitários
- **xUnit**: Framework de testes
- **FluentAssertions**: Para assertions mais legíveis e expressivas
- **Bogus**: Para geração de dados fake realistas
- **NSubstitute**: Para mocking de dependências

### Testes de Integração
- **Testcontainers**: Para testes com containers Docker
- Banco de dados em memória para testes isolados
- Testes de API com WebApplicationFactory

## 🌐 APIs REST

### Endpoints
- `POST /api/pedidos`: Criar pedido
- `GET /api/pedidos/{id}`: Obter pedido por ID
- `GET /api/pedidos/por-status`: Listar pedidos por status

### Respostas HTTP
- 200: OK
- 201: Created
- 400: Bad Request
- 404: Not Found
- 409: Conflict
- 500: Internal Server Error

## 🛠️ Como Executar

### Pré-requisitos
- .NET 8.0 SDK
- Visual Studio 2022 ou VS Code
- Git

### Instalação
```bash
# Clone o repositório
git clone https://github.com/seu-usuario/GerenciadorPedidos.git

# Entre no diretório
cd GerenciadorPedidos

# Restaure os pacotes
dotnet restore

# Execute as migrations
dotnet ef database update
```

### Executando a Aplicação
```bash
# Execute a API
dotnet run --project GerenciadorPedidos.API
```

### Executando os Testes
```