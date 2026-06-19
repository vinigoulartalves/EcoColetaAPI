# EcoColeta API

API RESTful em ASP.NET Core 8 para gestão de resíduos e reciclagem (tema ESG).

## Estrutura

```
EcoColeta.Api/
├── Controllers/
├── Data/              → AppDbContext, seed
├── Models/
├── ViewModels/
│   ├── Requests/
│   └── Responses/     → PagedResponse<T>
├── Services/
├── Repositories/
├── Middlewares/
├── Configurations/
└── Migrations/

EcoColeta.Tests/
├── Factories/
└── Controllers/
```

## Pré-requisitos

- .NET 8 SDK
- SQL Server local (ou Docker)

## Executar

```bash
docker compose up sqlserver -d
cd EcoColeta.Api
dotnet ef database update
dotnet run
```

Swagger: `http://localhost:5xxx/` (Development)

### Credenciais seed

| Campo  | Valor                 |
|--------|-----------------------|
| E-mail | `admin@ecocoleta.com` |
| Senha  | `Admin@123`           |

## Endpoints

| Método | Rota | Auth |
|--------|------|------|
| GET | `/api/pontos-coleta` | Público |
| GET | `/api/pontos-coleta/{id}` | Público |
| POST | `/api/pontos-coleta` | Admin |
| PUT | `/api/pontos-coleta/{id}` | Admin |
| GET | `/api/registros-residuos` | Público |
| POST | `/api/registros-residuos` | Admin |
| GET | `/api/alertas-coleta` | Público |
| PATCH | `/api/alertas-coleta/{id}/resolver` | Admin |
| POST | `/api/alertas-coleta/recalcular` | Admin |
| GET | `/api/destinacoes-residuos` | Público |
| GET | `/api/destinacoes-residuos/{tipoResiduo}` | Público |
| POST | `/api/destinacoes-residuos` | Admin |
| GET | `/api/relatorios-impacto/resumo` | Público |
| POST | `/api/auth/login` | Público |

## Paginação

Parâmetros: `pagina` (padrão 1), `tamanhoPagina` (padrão 10, máx. 50).

## Testes

```bash
dotnet test
```

## Docker

```bash
docker compose up --build
```

API em `http://localhost:8080`.

Coleção Postman: `postman/EcoColeta.postman_collection.json`
