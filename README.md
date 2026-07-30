# SeniorCrud

API REST desenvolvida em **.NET 9** para gerenciamento de usuários e endereços, construída com foco em boas práticas de arquitetura, separação de responsabilidades e alta manutenibilidade.

O projeto foi estruturado utilizando **Clean Architecture**, **DDD**, **CQRS** e **Vertical Slice Architecture**, servindo como uma base sólida para aplicações corporativas.

---

# Visão Geral

A aplicação disponibiliza uma API para gerenciamento de usuários e seus respectivos endereços, contemplando autenticação baseada em JWT, integração com o ViaCEP, exportação de dados em CSV e mecanismos de observabilidade.

Entre as funcionalidades implementadas destacam-se:

- Autenticação via JWT
- CRUD completo de Usuários
- CRUD completo de Endereços
- Consulta de CEP utilizando ViaCEP
- Exportação de usuários em CSV
- Cache para consultas
- Resiliência HTTP com Polly
- Logging estruturado
- OpenTelemetry
- Health Checks
- Testes unitários e de integração

---

# Arquitetura

O projeto foi estruturado utilizando um conjunto de padrões arquiteturais amplamente adotados em aplicações corporativas.

## Clean Architecture

A solução está organizada em camadas bem definidas, garantindo baixo acoplamento entre regras de negócio, infraestrutura e API.

Cada camada possui responsabilidades específicas, facilitando manutenção, evolução e testes.

---

## Domain-Driven Design (DDD)

A camada de domínio concentra:

- Entidades
- Value Objects
- Exceptions
- Enums
- Interfaces de domínio

As regras de negócio permanecem isoladas de tecnologias externas como Entity Framework ou ASP.NET Core.

---

## CQRS

As operações de leitura e escrita são separadas através de:

- Commands
- Queries
- Handlers

Essa separação torna os casos de uso mais simples, organizados e independentes.

---

## Vertical Slice Architecture

A camada Application é organizada por funcionalidades (features), agrupando os artefatos relacionados ao mesmo caso de uso.

Exemplo:

- Users
- Addresses
- Authentication
- ViaCEP
- Export

Essa abordagem reduz acoplamento entre funcionalidades e melhora a escalabilidade do projeto.

---

## Result Pattern

Todos os casos de uso retornam objetos padronizados de resultado, permitindo representar cenários como:

- Success
- Validation Failure
- Not Found
- Conflict
- Unauthorized
- Forbidden

Sem utilizar exceções como fluxo normal da aplicação.

---

# Estrutura da Solução

## SeniorCrud.Api

Camada responsável pela exposição da API.

Contém:

- Controllers
- Configuração da aplicação
- Swagger
- Middleware
- Dependency Injection
- Health Checks
- Configuração de autenticação

---

## SeniorCrud.Application

Responsável pelos casos de uso da aplicação.

Contém:

- Commands
- Queries
- Handlers
- DTOs
- Validators
- Behaviors do MediatR
- Result Pattern
- Mapeamentos
- Abstrações

---

## SeniorCrud.Domain

Núcleo da aplicação.

Contém:

- Entidades
- Value Objects
- Exceções
- Enums
- Regras de domínio

Sem dependência de frameworks externos.

---

## SeniorCrud.Infrastructure

Responsável pelas integrações técnicas.

Contém:

- JWT
- Hash de senha
- ViaCEP Client
- Cache
- CSV
- DateTime Provider
- Current User
- Configuração de HttpClient
- Polly

---

## SeniorCrud.Persistence

Camada de persistência utilizando Entity Framework Core.

Contém:

- DbContext
- Configurações Fluent API
- Repositórios
- Unit of Work
- Interceptors
- Migrations
- Seed

---

## SeniorCrud.UnitTests

Projeto destinado aos testes unitários das regras da aplicação.

Abrange domínio, handlers, validators, serviços e comportamentos do pipeline.

---

## SeniorCrud.IntegrationTests

Projeto destinado aos testes de integração utilizando `WebApplicationFactory`.

Valida o comportamento dos principais endpoints da API.

---

# Tecnologias Utilizadas

- .NET 9
- ASP.NET Core Web API
- C#
- Entity Framework Core
- SQL Server
- MediatR
- FluentValidation
- AutoMapper
- Serilog
- OpenTelemetry
- Polly
- JWT Bearer Authentication
- BCrypt
- CsvHelper
- Memory Cache
- HttpClientFactory
- Swagger (Swashbuckle)
- Health Checks
- xUnit

---

# Funcionalidades Implementadas

## Autenticação

- Login utilizando JWT
- Validação de senha com BCrypt
- Geração de Access Token

---

## Usuários

- Cadastro
- Consulta por ID
- Listagem
- Atualização
- Remoção

---

## Endereços

- Cadastro
- Consulta por ID
- Consulta por usuário
- Atualização
- Remoção

---

## ViaCEP

- Consulta de CEP
- Integração via HttpClient
- Cache das consultas
- Resiliência com Polly

---

## Exportação

- Exportação de usuários em CSV

---

## Cache

Aplicado às consultas de:

- Usuário por ID
- Listagem de usuários
- Endereços
- Consulta ViaCEP

Com invalidação automática após operações de escrita.

---

## Resiliência

O cliente HTTP do ViaCEP utiliza Polly com:

- Retry
- Timeout
- Circuit Breaker

---

## Observabilidade

- Logging estruturado
- Correlation ID
- OpenTelemetry
- Instrumentação HTTP
- Instrumentação EF Core
- ActivitySource da aplicação

---

## Health Checks

Endpoints disponíveis:

- `/health`
- `/liveness`
- `/readiness`

Incluindo verificações para:

- SQL Server
- ViaCEP
- Memory Cache

---

# Fluxo da Aplicação

Uma requisição percorre o seguinte fluxo:

```text
Controller
        │
        ▼
MediatR
        │
        ▼
Pipeline Behaviors
(Validation, Logging, Performance, Exception)
        │
        ▼
Handler
        │
        ▼
Repositories
        │
        ▼
Persistence
        │
        ▼
SQL Server
```

Durante esse fluxo também podem atuar:

- Cache
- Logging
- OpenTelemetry
- Polly (quando há chamadas HTTP externas)

---

# Principais Decisões Arquiteturais

## MediatR

Toda comunicação entre Controllers e casos de uso ocorre através do MediatR, reduzindo acoplamento entre camadas.

---

## FluentValidation

As validações são executadas automaticamente pelo pipeline do MediatR, mantendo os handlers focados apenas na regra de negócio.

---

## Pipeline Behaviors

Foram implementados behaviors para:

- Validação
- Logging
- Tratamento de exceções
- Medição de desempenho

---

## Result Pattern

Os casos de uso retornam resultados padronizados, facilitando o tratamento de erros e aumentando a consistência da API.

---

## Dependency Injection

Todos os serviços são registrados através de extensões específicas de cada camada, mantendo a configuração organizada e modular.

---

## Repositórios e Unit of Work

O acesso aos dados é abstraído por interfaces, enquanto a persistência é centralizada através do Unit of Work.

---

## Separação de Responsabilidades

Cada projeto possui uma responsabilidade única, reduzindo dependências entre camadas e facilitando evolução futura.

---

## Observabilidade

A aplicação possui suporte nativo para:

- Logs estruturados
- Correlation ID
- OpenTelemetry
- Health Checks

permitindo melhor diagnóstico e monitoramento.

---

# Como Executar

## Pré-requisitos

- .NET SDK 9
- SQL Server
- Git

---

## Restaurar dependências

```bash
dotnet restore
```

---

## Configurar banco de dados

Atualize a Connection String em:

```
appsettings.json
```

ou

```
appsettings.Development.json
```

conforme seu ambiente.

---

## Aplicar as Migrations

```bash
dotnet ef database update --project src/SeniorCrud.Persistence --startup-project src/SeniorCrud.Api
```

---

## Executar a aplicação

```bash
dotnet run --project src/SeniorCrud.Api
```

---

## Swagger

Após iniciar a aplicação, acesse:

```
/swagger
```

---

# Testes

A solução possui dois projetos de testes:

- SeniorCrud.UnitTests
- SeniorCrud.IntegrationTests

Para executar todos os testes:

```bash
dotnet test
```

Os testes cobrem principalmente:

- Value Objects
- Entidades
- Result Pattern
- Validators
- Behaviors
- Handlers
- Serviços de infraestrutura
- Fluxos principais da API
- Integração dos endpoints

---

# Melhorias Futuras

Embora a solução esteja funcional, algumas evoluções podem ser consideradas:

- Refresh Token
- Docker e Docker Compose
- Pipeline de CI/CD
- Redis como cache distribuído
- Versionamento da API
- Paginação com metadados
- Rate Limiting
- Documentação OpenAPI mais detalhada
- Monitoramento via OTLP (Jaeger, Grafana, Zipkin ou similar)
- Atualização do pacote AutoMapper para eliminar o aviso de vulnerabilidade (NU1903)

---

# Considerações Finais

O projeto foi desenvolvido priorizando organização, separação de responsabilidades e aderência a padrões amplamente utilizados no ecossistema .NET.

A combinação de **Clean Architecture**, **DDD**, **CQRS**, **Vertical Slice Architecture** e **Result Pattern** proporciona uma base consistente para manutenção, evolução e escalabilidade, enquanto recursos como observabilidade, cache, resiliência HTTP e testes automatizados contribuem para a robustez da solução.