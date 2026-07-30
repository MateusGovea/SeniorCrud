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

# Executando com Docker

**Pré-requisito:** apenas Docker Desktop instalado.

> ⚠️ **Importante:** antes de executar, certifique-se de que as portas `1433` (SQL Server) e `80` (frontend) não estão em uso. A API será exposta na porta `5000`.

## Passo único

```bash
docker compose up --build
```

## URLs

| Serviço   | URL                                 |
|-----------|-------------------------------------|
| Frontend  | http://localhost                    |
| API       | http://localhost:5000               |
| Swagger   | http://localhost:5000/swagger       |
| Health    | http://localhost:5000/health        |
| SQL Server| `localhost,1433` (sa / Your_password123) |

## Login

Não há usuário seed. Crie um usuário via Swagger (`POST /api/users`) e faça login normalmente.

## Parar os containers

```bash
docker compose down
```

Para remover também o volume do banco de dados:

```bash
docker compose down -v
```

---

# Executando sem Docker

## Pré-requisitos

- .NET SDK 9
- SQL Server (local ou container)
- Node.js 20+
- Git

---

## Backend

### Restaurar dependências

```bash
dotnet restore
```

### Configurar banco de dados

Atualize a Connection String em `appsettings.json` conforme seu ambiente.

### Aplicar as Migrations

```bash
dotnet ef database update --project src/SeniorCrud.Persistence --startup-project src/SeniorCrud.Api
```

### Executar a API

```bash
dotnet run --project src/SeniorCrud.Api
```

A API será iniciada em `http://localhost:5029` (padrão do `launchSettings.json`).

### Swagger

Após iniciar a aplicação, acesse:

```
http://localhost:5029/swagger
```

---

## Frontend

### Configurar URL da API

Edite `web/.env` e ajuste a variável `VITE_API_URL` para a URL da sua API:

```
VITE_API_URL=http://localhost:5029
```

### Instalar dependências

```bash
cd web
npm install
```

### Executar em desenvolvimento

```bash
npm run dev
```

O frontend será iniciado em `http://localhost:5173`.

### Build de produção

```bash
npm run build
```

Os arquivos estáticos serão gerados em `web/dist/`.

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