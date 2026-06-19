# EcoColeta API

API RESTful em ASP.NET Core 8 para gestão de resíduos e reciclagem (tema ESG).

## Estrutura do Projeto

```
EcoColeta.Api/
├── Controllers/          # Endpoints REST
├── Data/                 # DbContext e seed
├── Models/               # Entidades do banco
├── ViewModels/
│   ├── Requests/         # DTOs de entrada (com validações)
│   └── Responses/        # DTOs de saída
├── Services/             # Regras de negócio
├── Repositories/         # Acesso a dados
├── Middlewares/          # Tratamento global de exceções
├── Configurations/       # DI, JWT, Swagger
└── Migrations/           # Migrations EF Core

EcoColeta.Tests/
├── Factories/            # WebApplicationFactory para testes
└── Controllers/          # Testes de integração por controller
```

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server local (ou Docker)
- Postman / Insomnia (opcional)

## Executar Localmente

### 1. Subir o SQL Server (Docker)

```bash
docker compose up sqlserver -d
```

### 2. Aplicar migrations e rodar a API

```bash
cd EcoColeta.Api
dotnet ef database update
dotnet run
```

A API estará em `https://localhost:7xxx` ou `http://localhost:5xxx`. O Swagger fica na raiz (`/`).

### 3. Credenciais padrão (seed)

| Campo  | Valor                 |
|--------|-----------------------|
| E-mail | `admin@ecocoleta.com` |
| Senha  | `Admin@123`           |
| Role   | `Admin`               |

## Autenticação JWT

1. `POST /api/Auth/login` com e-mail e senha
2. Copie o `token` da resposta
3. Nos endpoints protegidos, envie o header: `Authorization: Bearer {token}`

## Controllers

| Controller | Leitura (público) | Escrita (Admin) |
|---|---|---|
| `PontosColeta` | GET listar, GET por id | POST, PUT |
| `RegistrosResiduos` | GET listar, GET por id | POST registrar |
| `AlertasColeta` | GET listar, GET por id | POST recalcular |
| `DestinacoesResiduos` | GET listar, GET por id/tipo | POST, PUT, DELETE |
| `RelatoriosImpacto` | GET relatório | POST gerar consolidado |
| `Auth` | POST login | — |

## Paginação

Endpoints de listagem aceitam:

- `pagina` (padrão: 1)
- `tamanhoPagina` (padrão: 10, máximo: 50)

Resposta paginada:

```json
{
  "itens": [],
  "paginaAtual": 1,
  "tamanhoPagina": 10,
  "totalItens": 0,
  "totalPaginas": 0
}
```

## Regras de Negócio

- Ao registrar descarte, a ocupação do ponto aumenta automaticamente
- Ocupação ≥ 80% → alerta de **Atenção**
- Ocupação ≥ 100% → alerta **Crítico**
- Tipo de resíduo deve ser compatível com o ponto de coleta

## Docker (API + SQL Server)

```bash
docker compose up --build
```

API disponível em `http://localhost:8080`.

## Testes

```bash
dotnet test
```

## Exemplos de Requisição

### Login

```http
POST /api/Auth/login
Content-Type: application/json

{
  "email": "admin@ecocoleta.com",
  "senha": "Admin@123"
}
```

### Criar ponto de coleta (Admin)

```http
POST /api/PontosColeta
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "EcoPonto Vila",
  "endereco": "Rua Verde, 50",
  "bairro": "Vila Nova",
  "cidade": "São Paulo",
  "latitude": -23.55,
  "longitude": -46.63,
  "tipoResiduoAceito": 1,
  "capacidadeMaximaKg": 400
}
```

### Registrar descarte (Admin)

```http
POST /api/RegistrosResiduos
Authorization: Bearer {token}
Content-Type: application/json

{
  "pontoColetaId": 1,
  "tipoResiduo": 1,
  "pesoKg": 25.5,
  "origem": "Escola Municipal",
  "observacao": "Coleta semanal"
}
```

### Relatório de impacto

```http
GET /api/RelatoriosImpacto
```
