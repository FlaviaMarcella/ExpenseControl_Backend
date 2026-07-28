# ExpenseControl.Api

API REST em **ASP.NET Core (.NET 10)** para controle de gastos domésticos: gestão de membros
da família (`People`), lançamentos financeiros (`Transaction`), autenticação via **JWT**, e
documentação interativa via **Swagger**.

Projeto acadêmico/portfólio, desenvolvido como parte do processo seletivo para vaga de estágio
em desenvolvimento backend.

## Stack

- **.NET 10 / ASP.NET Core Web API**
- **Entity Framework Core** (SQLite)
- **JWT Bearer Authentication** + **BCrypt** para hash de senhas
- **Swashbuckle** (Swagger/OpenAPI) para documentação interativa

## Arquitetura

Camadas em estilo similar ao usado em projetos Spring Boot, adaptadas às convenções do
ecossistema .NET:

```
Controllers/    → endpoints HTTP (equivalente a @RestController)
Service/        → regras de orquestração e acesso a dados via EF Core
Model/
  Entity/       → entidades mapeadas pelo EF Core (equivalente a @Entity)
  Enums/        → enums de domínio (Relationship, TypeTransaction)
  Domain/       → regras de negócio puras (TransactionRules, DateUtils)
  Repository/   → interfaces dos serviços (IPeopleService, ITransactionService)
Dto/            → objetos de transferência (records imutáveis)
Mapper/         → conversão manual Entity ↔ DTO
Data/           → AppDbContext (configuração do EF Core)
Middleware/     → tratamento global de exceções
```

**Decisão de design relevante**: diferente do Spring Data JPA, o EF Core já expõe o
`DbContext` como uma espécie de repository pronto — por isso este projeto **não tem** uma
camada de Repository separada; os Services acessam o `AppDbContext` diretamente.

## Autenticação

A API usa **JWT Bearer tokens**. O fluxo é:

1. `POST /api/auth/register` — cria um usuário (`username`, `password`, e opcionalmente um
   `peopleId` para vincular a uma pessoa já cadastrada)
2. `POST /api/auth/login` — retorna `{ "token": "..." }`
3. Toda chamada aos endpoints protegidos (`/api/people`, `/api/transaction`) precisa do header
   `Authorization: Bearer {token}`

Um usuário administrativo padrão é criado automaticamente no primeiro start da aplicação
(seed), definido em `appsettings.json` sob a chave `DefaultAdminUser`. Use essas credenciais
para o primeiro login sem precisar registrar um usuário manualmente.

## Executando o projeto

### Pré-requisitos

- .NET 10 SDK

### Passos

```bash
dotnet restore
dotnet run
```

A aplicação:

- Aplica as migrations do EF Core automaticamente ao iniciar
- Cria o usuário administrativo padrão, se ainda não existir nenhum usuário no banco
- Sobe o Swagger UI na raiz do site (`http://localhost:5241/`, ou a porta exibida no console)

### Testando pelo Swagger

1. Abra a raiz da aplicação no navegador
2. `POST /api/auth/login` com as credenciais padrão (ver `appsettings.json`)
3. Copie o token retornado
4. Clique em **Authorize** (canto superior direito), cole o token (sem o prefixo `Bearer`)
5. Teste os demais endpoints normalmente

## Principais endpoints

| Método | Rota                                 | Descrição                                |
|--------|--------------------------------------|------------------------------------------|
| POST   | `/api/auth/register`                 | Registra um novo usuário                 |
| POST   | `/api/auth/login`                    | Autentica e retorna o token JWT          |
| GET    | `/api/people`                        | Lista todas as pessoas                   |
| GET    | `/api/people/{id}`                   | Busca pessoa por Id                      |
| POST   | `/api/people`                        | Cria uma pessoa                          |
| PUT    | `/api/people/{id}`                   | Atualiza uma pessoa                      |
| DELETE | `/api/people/{id}`                   | Exclui uma pessoa (e suas transações)    |
| GET    | `/api/transaction`                   | Lista todas as transações                |
| GET    | `/api/transaction/people/{peopleId}` | Lista transações de uma pessoa           |
| GET    | `/api/transaction/{id}`              | Busca transação por Id                   |
| POST   | `/api/transaction`                   | Cria uma transação                       |
| PUT    | `/api/transaction/{id}`              | Atualiza uma transação                   |
| DELETE | `/api/transaction/{id}`              | Exclui uma transação                     |
| DELETE | `/api/transaction/people/{peopleId}` | Exclui todas as transações de uma pessoa |

Todos os endpoints acima (exceto `/api/auth/*`) exigem autenticação.

## Regras de negócio implementadas

- **Idade mínima para receitas**: uma `Transaction` do tipo `Receive` só pode ser criada para
  uma `People` com 18 anos ou mais (ver `Model/Domain/TransactionRules.cs`)
- **Idade calculada, não persistida**: `People.Age` não existe como coluna — é sempre
  calculada a partir de `BirthDate` (ver `Model/Domain/DateUtils.cs`), evitando dados
  desatualizados
- **Exclusão em cascata controlada**: excluir uma `People` remove primeiro suas transações
  associadas, evitando violação de integridade referencial
- **Tratamento centralizado de erros**: um middleware global (`ExceptionHandlingMiddleware`)
  traduz exceções de regra de negócio em respostas HTTP padronizadas (400/500), no formato
  `ProblemDetails`

## Testando o fluxo completo

1. Registre ou use o usuário padrão e faça login
2. Crie uma `People` (`POST /api/people`)
3. Crie uma `Transaction` associada a ela (`POST /api/transaction`)
4. Liste, atualize e explore os demais endpoints

## Possíveis melhorias futuras

- Separar DTOs de entrada e saída também para `People`/`Transaction` (hoje o mesmo DTO serve
  para os dois sentidos, o que permite que campos calculados como `Age` sejam enviados pelo
  cliente sem qualquer efeito — não é um bug, mas pode confundir quem consome a API)
- Constraint de unicidade de `Username` no nível do banco (`HasIndex(...).IsUnique()`), hoje
  validada apenas na aplicação
- Paginação nas listagens (`GetAll`) para bases de dados maiores
- Testes automatizados (unitários para `TransactionRules`/`DateUtils`, e de integração para
  os Controllers)

