# EcoColeta API

## Descrição do Projeto

O **EcoColeta API** é um WebService RESTful desenvolvido em **ASP.NET Core 8** com foco no tema **ESG** de gestão de resíduos e reciclagem. A aplicação simula um sistema de apoio à coleta seletiva urbana, permitindo o cadastro de pontos de coleta, o registro de descartes e o monitoramento da capacidade de cada ponto.

### Objetivo da aplicação

* Gerenciar pontos de coleta seletiva.
* Registrar descartes de resíduos.
* Monitorar ocupação dos pontos de coleta.
* Gerar alertas automáticos quando a capacidade estiver próxima do limite.
* Consultar orientações de destinação correta de resíduos.
* Gerar relatório de impacto ambiental.

A solução está organizada no arquivo `EcoColeta.sln`, com os projetos `EcoColeta.Api` (API) e `EcoColeta.Tests` (testes automatizados).

---

## Tema ESG

**Tema escolhido:** Gestão de resíduos e reciclagem.

O projeto contribui para práticas ESG ao:

* Auxiliar no descarte correto de resíduos, com orientações por tipo de material.
* Apoiar a coleta seletiva por meio do cadastro e consulta de pontos de coleta.
* Ajudar a reduzir descarte irregular, direcionando resíduos aos pontos adequados.
* Permitir acompanhamento da capacidade dos pontos de coleta em tempo real.
* Fornecer dados consolidados para análise de impacto ambiental positivo.

---

## Tecnologias Utilizadas

Principais tecnologias e pacotes encontrados no projeto:

| Tecnologia / Pacote | Uso no projeto |
|---|---|
| C# | Linguagem principal |
| ASP.NET Core 8 | Framework Web API |
| Entity Framework Core 8.0.11 | ORM e acesso a dados |
| Microsoft.EntityFrameworkCore.SqlServer | Provider SQL Server |
| Microsoft.EntityFrameworkCore.Design | Ferramentas de migration |
| SQL Server | Banco de dados principal (local ou Docker) |
| Migrations do Entity Framework | Versionamento do banco |
| JWT Bearer Authentication | Autenticação de endpoints administrativos |
| BCrypt.Net-Next | Hash de senha do usuário administrador |
| Swashbuckle.AspNetCore (Swagger / OpenAPI) | Documentação da API |
| xUnit 2.5.3 | Testes automatizados |
| Microsoft.AspNetCore.Mvc.Testing | Testes de integração da API |
| Microsoft.EntityFrameworkCore.InMemory | Banco em memória nos testes |
| Docker | Containerização da API |
| Postman | Testes manuais via collection exportada |

---

## Arquitetura do Projeto

O projeto foi organizado com separação de responsabilidades, seguindo uma estrutura inspirada em **MVVM** e em camadas de aplicação.

### Estrutura da solução

```text
EcoColeta/
├── EcoColeta.sln
├── EcoColeta.Api/
│   ├── Controllers/
│   ├── Models/
│   ├── ViewModels/
│   │   ├── Requests/
│   │   └── Responses/
│   ├── Services/
│   ├── Repositories/
│   ├── Data/
│   ├── Middlewares/
│   ├── Configurations/
│   ├── Migrations/
│   ├── Exceptions/
│   ├── Program.cs
│   ├── appsettings.json
│   └── Dockerfile
├── EcoColeta.Tests/
│   ├── Controllers/
│   └── Factories/
├── postman/
│   └── EcoColeta.postman_collection.json
└── docker-compose.yml
```

### Pastas principais (`EcoColeta.Api/`)

| Pasta | Responsabilidade |
|---|---|
| `Controllers/` | Endpoints REST da API |
| `Models/` | Entidades persistidas no banco de dados |
| `ViewModels/` | Objetos de entrada (`Requests/`) e saída (`Responses/`) |
| `Services/` | Regras de negócio da aplicação |
| `Repositories/` | Acesso e consulta ao banco de dados |
| `Data/` | `AppDbContext`, seed inicial (`DbInitializer`) |
| `Middlewares/` | Tratamento global de exceções |
| `Configurations/` | Injeção de dependência, JWT e Swagger |
| `Migrations/` | Arquivos de versionamento do banco de dados |
| `Exceptions/` | Exceções de domínio (`NotFound`, `Business`, etc.) |

---

## Funcionalidades Principais

* Cadastro e consulta de pontos de coleta.
* Registro de descarte de resíduos.
* Atualização automática da ocupação dos pontos de coleta.
* Geração de alertas quando a ocupação atingir **80% ou mais** da capacidade (nível **Atencao**).
* Alerta crítico quando a ocupação atingir **100% ou mais** (nível **Critico**).
* Consulta de destinação correta por tipo de resíduo.
* Relatório consolidado de impacto ambiental.
* Autenticação via JWT.
* Autorização com role `Admin` para endpoints administrativos.
* Paginação nos endpoints de listagem.

---

## Controllers e Endpoints

| Controller | Método | Rota | Descrição | Autenticação |
|---|---|---|---|---|
| `AuthController` | POST | `/api/auth/login` | Realiza login e retorna token JWT | Não |
| `PontosColetaController` | GET | `/api/pontos-coleta` | Lista pontos de coleta com paginação e filtros | Não |
| `PontosColetaController` | GET | `/api/pontos-coleta/{id}` | Busca ponto de coleta por ID | Não |
| `PontosColetaController` | POST | `/api/pontos-coleta` | Cadastra novo ponto de coleta | Admin (JWT) |
| `PontosColetaController` | PUT | `/api/pontos-coleta/{id}` | Atualiza ponto de coleta existente | Admin (JWT) |
| `RegistrosResiduosController` | GET | `/api/registros-residuos` | Lista registros de descarte com paginação | Não |
| `RegistrosResiduosController` | POST | `/api/registros-residuos` | Registra descarte e atualiza ocupação | Admin (JWT) |
| `AlertasColetaController` | GET | `/api/alertas-coleta` | Lista alertas com paginação e filtros | Não |
| `AlertasColetaController` | PATCH | `/api/alertas-coleta/{id}/resolver` | Marca alerta como resolvido | Admin (JWT) |
| `AlertasColetaController` | POST | `/api/alertas-coleta/recalcular` | Recalcula alertas de todos os pontos ativos | Admin (JWT) |
| `DestinacoesResiduosController` | GET | `/api/destinacoes-residuos` | Lista destinações de resíduos | Não |
| `DestinacoesResiduosController` | GET | `/api/destinacoes-residuos/{tipoResiduo}` | Consulta destinação por tipo de resíduo | Não |
| `DestinacoesResiduosController` | POST | `/api/destinacoes-residuos` | Cadastra nova destinação de resíduo | Admin (JWT) |
| `RelatoriosImpactoController` | GET | `/api/relatorios-impacto/resumo` | Retorna resumo de impacto ambiental | Não |

### Filtros disponíveis em listagens

| Endpoint | Filtros opcionais |
|---|---|
| `GET /api/pontos-coleta` | `cidade`, `bairro`, `tipoResiduo`, `ativo` |
| `GET /api/registros-residuos` | `pontoColetaId`, `tipoResiduo` |
| `GET /api/alertas-coleta` | `resolvido`, `nivel`, `pontoColetaId` |

---

## Pré-requisitos

Para executar o projeto localmente, é necessário:

* [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
* SQL Server Local, SQL Server Express ou SQL Server em Docker
* [Visual Studio 2022](https://visualstudio.microsoft.com/), [Visual Studio Code](https://code.visualstudio.com/) ou Cursor
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) (opcional, para execução via container)
* [Postman](https://www.postman.com/) (para importar e testar a collection)
* [dotnet-ef](https://www.nuget.org/packages/dotnet-ef) (ferramenta global para migrations)

Instalação da ferramenta EF Core:

```bash
dotnet tool install --global dotnet-ef
```

---

## Configuração do Banco de Dados

O projeto utiliza **SQL Server** como banco principal. A connection string está configurada em `EcoColeta.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=EcoColetaDb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
}
```

> **Observação:** ajuste servidor, usuário e senha conforme seu ambiente local. Se utilizar **Windows Authentication**, adapte a string de conexão para o padrão do seu SQL Server.

O banco é criado e atualizado automaticamente pelas **migrations** do Entity Framework Core, localizadas em `EcoColeta.Api/Migrations/`.

### Dados iniciais (seed)

Ao iniciar a API, o `DbInitializer` popula o banco com:

* 1 usuário administrador
* 3 pontos de coleta de exemplo
* 9 destinações de resíduos (um registro por tipo)

Credenciais do administrador inicial:

| Campo | Valor |
|---|---|
| E-mail | `admin@ecocoleta.com` |
| Senha | `Admin@123` |
| Role | `Admin` |

---

## Como Rodar o Projeto Localmente

### Opção 1 — SQL Server via Docker + API local

```bash
# 1. Abrir a pasta da solução
cd EcoColeta

# 2. Subir o SQL Server (na raiz da solução)
docker compose up sqlserver -d

# 3. Restaurar os pacotes
dotnet restore

# 4. Aplicar as migrations no banco
dotnet ef database update --project EcoColeta.Api

# 5. Executar a API
dotnet run --project EcoColeta.Api
```

### Opção 2 — Apenas a API (com SQL Server já instalado)

```bash
cd EcoColeta
dotnet restore
dotnet ef database update --project EcoColeta.Api
dotnet run --project EcoColeta.Api
```

### Acesso ao Swagger

Com o perfil **Development** ativo, o Swagger é exibido na **raiz da aplicação**, conforme configurado em `EcoColeta.Api/Program.cs`.

URLs configuradas em `EcoColeta.Api/Properties/launchSettings.json`:

| Perfil | URL |
|---|---|
| HTTP | `http://localhost:5261` |
| HTTPS | `https://localhost:7164` |

Exemplos de acesso:

```text
http://localhost:5261
https://localhost:7164
```

O JSON da especificação OpenAPI fica disponível em:

```text
http://localhost:5261/swagger/v1/swagger.json
```

---

## Como Rodar com Docker

O Dockerfile está em `EcoColeta.Api/Dockerfile`. Na raiz da solução também existe o arquivo `docker-compose.yml`, que sobe **SQL Server + API** juntos.

### Apenas a imagem da API

```bash
# Na raiz da solução (EcoColeta/)
docker build -t ecocoleta-api -f EcoColeta.Api/Dockerfile .

docker run -p 8080:8080 ecocoleta-api
```

### API + SQL Server com Docker Compose

```bash
# Na raiz da solução
docker compose up --build
```

A API ficará disponível em `http://localhost:8080`.

> **Importante:** ao executar via Docker, a connection string precisa apontar para um SQL Server acessível pelo container. O `docker-compose.yml` já configura `Server=sqlserver,1433` para o serviço da API.

---

## Autenticação

Alguns endpoints são **públicos** (principalmente operações de leitura) e outros exigem **autenticação JWT** com a role `Admin`.

### Login

```http
POST /api/auth/login
Content-Type: application/json
```

```json
{
  "email": "admin@ecocoleta.com",
  "senha": "Admin@123"
}
```

### Uso do token

Copie o valor do campo `token` retornado e envie no header das requisições protegidas:

```http
Authorization: Bearer SEU_TOKEN_AQUI
```

No Swagger, clique em **Authorize** e informe: `Bearer {seu_token}`.

---

## Paginação

Os endpoints de listagem aceitam os parâmetros de query `pagina` e `tamanhoPagina`.

Exemplo:

```http
GET /api/pontos-coleta?pagina=1&tamanhoPagina=10
```

O tamanho máximo de página é **50** (definido em `EcoColeta.Api/ViewModels/Requests/ParametrosPaginacao.cs`).

A resposta utiliza o ViewModel `PagedResponse<T>` com os campos:

| Campo JSON | Descrição |
|---|---|
| `itens` | Lista de registros da página atual |
| `paginaAtual` | Número da página retornada |
| `tamanhoPagina` | Quantidade de itens por página |
| `totalItens` | Total de registros encontrados |
| `totalPaginas` | Total de páginas calculadas |

---

## Testes Automatizados

Os testes estão no projeto `EcoColeta.Tests/`, utilizando **xUnit** e **Microsoft.AspNetCore.Mvc.Testing**.

A factory `CustomWebApplicationFactory` (`EcoColeta.Tests/Factories/CustomWebApplicationFactory.cs`) sobe a API em ambiente de teste com **EF Core InMemory**, sem depender do SQL Server local.

### Executar os testes

```bash
dotnet test
```

### Testes implementados

| Arquivo | Cenário |
|---|---|
| `PontosColetaControllerTests` | `GET /api/pontos-coleta` retorna HTTP 200 |
| `RegistrosResiduosControllerTests` | `GET /api/registros-residuos` retorna HTTP 200 |
| `AlertasColetaControllerTests` | `GET /api/alertas-coleta` retorna HTTP 200 |
| `DestinacoesResiduosControllerTests` | `GET /api/destinacoes-residuos` retorna HTTP 200 |
| `RelatoriosImpactoControllerTests` | `GET /api/relatorios-impacto/resumo` retorna HTTP 200 |
| `AuthControllerTests` | Login válido retorna token; endpoint protegido sem token retorna 401 |
| `RegistrosResiduosAlertaTests` | Descarte acima de 80% gera alerta automaticamente |

---

## Collection do Postman

A collection está localizada em:

```text
postman/EcoColeta.postman_collection.json
```

### Pendência

Não há arquivo de environment separado no repositório. As variáveis estão definidas dentro da própria collection:

| Variável | Valor padrão |
|---|---|
| `baseUrl` | `http://localhost:5261` |
| `token` | (preenchido após o login) |

Se desejar criar um environment, o local recomendado é:

```text
postman/EcoColeta.postman_environment.json
```

### Como importar

1. Abra o Postman.
2. Clique em **Import**.
3. Selecione o arquivo `postman/EcoColeta.postman_collection.json`.
4. Ajuste a variável `baseUrl` conforme a porta em que a API estiver rodando.
5. Execute a requisição **POST Login**.
6. O script de teste da collection salva automaticamente o `token` retornado na variável `token`.

---

## Ordem Recomendada para Testar no Postman

1. **Login** — `POST /api/auth/login`
2. **Listar pontos de coleta** — `GET /api/pontos-coleta?pagina=1&tamanhoPagina=10`
3. **Buscar ponto por ID** — `GET /api/pontos-coleta/1`
4. **Registrar descarte** — `POST /api/registros-residuos` (com Bearer Token)
5. **Listar alertas** — `GET /api/alertas-coleta`
6. **Resolver alerta** — `PATCH /api/alertas-coleta/{id}/resolver` (com Bearer Token)
7. **Consultar destinação** — `GET /api/destinacoes-residuos/Plastico`
8. **Gerar relatório** — `GET /api/relatorios-impacto/resumo`

---

## Migrations

Os arquivos de migration estão em:

```text
EcoColeta.Api/Migrations/
```

Arquivos principais:

* `20260619155705_InitialCreate.cs`
* `AppDbContextModelSnapshot.cs`

### Aplicar migrations

```bash
dotnet ef database update --project EcoColeta.Api
```

### Criar nova migration (se necessário)

```bash
dotnet ef migrations add NomeDaMigration --project EcoColeta.Api --output-dir Migrations
```

---

## Estrutura de Entrega

A entrega da atividade deve conter **dois arquivos ZIP**:

### 1. ZIP com o código-fonte

Deve incluir:

* `EcoColeta.sln`
* `EcoColeta.Api/` (projeto da API)
* `EcoColeta.Tests/` (projeto de testes)
* `EcoColeta.Api/Dockerfile`
* `EcoColeta.Api/Migrations/`
* `docker-compose.yml`
* `README.md`
* `appsettings.json` e demais arquivos de configuração necessários

**Não incluir:**

* `bin/`
* `obj/`
* `.vs/`
* arquivos temporários de build

Comando sugerido para gerar o ZIP a partir do Git:

```bash
git archive --format=zip --output=EcoColeta-codigo.zip HEAD
```

### 2. ZIP com a collection do Postman

Incluir o arquivo:

```text
postman/EcoColeta.postman_collection.json
```

---

## Observações Importantes

* Verifique a **connection string** em `EcoColeta.Api/appsettings.json` antes de executar a API.
* Aplique as **migrations** (`dotnet ef database update`) antes de testar os endpoints.
* Use o **token JWT** nos endpoints protegidos com role `Admin`.
* Confirme se o **SQL Server** está ativo (local ou via `docker compose up sqlserver -d`).
* Ajuste a variável **`baseUrl`** no Postman para a mesma porta usada pela API (`5261` local ou `8080` no Docker).
* O tipo de resíduo nos endpoints pode ser informado pelo nome do enum (ex.: `Plastico`) ou pelo valor numérico (ex.: `1`).
* Ao registrar descarte, o **tipo de resíduo deve ser compatível** com o tipo aceito pelo ponto de coleta.

---

## Autor

**Autor:** Vinícius Goulart Alves  
**Atividade:** DESAFIO: WEBSERVICE COM ASP.NET CORE 8  
**Instituição:** FIAP
