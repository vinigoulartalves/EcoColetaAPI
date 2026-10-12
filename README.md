# Projeto - Cidades ESG Inteligentes

## EcoColeta API | Navegando pelo mundo DevOps

**Integrante:** Vinícius Goulart Alves · **Instituição:** FIAP · **Entrega:** outubro/2026.

API REST de gestão de resíduos e reciclagem em ASP.NET Core 8. Permite cadastrar pontos de coleta, registrar descartes, emitir alertas de ocupação e consultar indicadores ambientais. Esta entrega integra build, testes, imagem Docker e deploy automatizado em staging e produção acadêmica.

> **Escopo dos ambientes:** staging e production são implantados de verdade com API + SQL Server nos runners do GitHub Actions. São ambientes acadêmicos temporários, removidos depois das verificações e capturas. Não existe hospedagem pública permanente nesta configuração. `Production` identifica a configuração ASP.NET do segundo ambiente; não representa uma operação comercial. SQL Server Developer é usado exclusivamente para desenvolvimento/teste/demonstração.

## Como executar localmente com Docker

Requisitos: Docker Desktop com contêineres Linux e Docker Compose v2 atualizado; máquina x86-64. Reserve pelo menos 4 GB de RAM para uma stack (preferencialmente 8 GB para duas). O SQL Server usa imagem Linux AMD64.

```bash
git clone --branch codex/devops-atividade https://github.com/vinigoulartalves/EcoColetaAPI.git
cd EcoColetaAPI
cp .env.example .env
# Edite .env e substitua as senhas e JWT_SECRET por valores próprios.
docker compose config --quiet
docker compose up -d --build --wait --wait-timeout 240
```

No PowerShell, use `Copy-Item .env.example .env` no lugar de `cp`. Não precisa instalar .NET para rodar a stack Docker. As migrations criam o banco na primeira inicialização. O seed cria um administrador, três pontos de exemplo e nove destinações.

| Acesso local | Endereço |
|---|---|
| Painel operacional | http://localhost:8080/ |
| Swagger (habilitado em `.env.example`) | http://localhost:8080/swagger |
| Prontidão da API + banco | http://localhost:8080/health/ready |
| Liveness da API | http://localhost:8080/health/live |
| Pontos de coleta | http://localhost:8080/api/pontos-coleta |
| Impacto ambiental | http://localhost:8080/api/relatorios-impacto/resumo |

```bash
docker compose ps
docker compose logs --tail=100 api
docker compose logs --tail=100 sqlserver
docker compose stop
docker compose up -d --wait --wait-timeout 240
# Desliga e remove contêineres/rede, preservando os dados:
docker compose down
```

`docker compose down --volumes` apaga os dados; use somente para descartar intencionalmente um ambiente de teste. Alterar a senha no `.env` não altera a senha de um SQL Server já inicializado no volume. O seed também não redefine a senha de um administrador existente.

### Variáveis de ambiente

| Variável | Finalidade |
|---|---|
| `COMPOSE_PROJECT_NAME` | Prefixa contêineres, rede e volume para isolar a stack |
| `API_IMAGE` | Imagem/tag utilizada pela API |
| `APP_ENVIRONMENT` | `Development`, `Staging` ou `Production` |
| `API_PORT` | Porta HTTP publicada somente em `127.0.0.1` |
| `MSSQL_SA_PASSWORD` | Senha forte do SQL Server; não use `;` na connection string |
| `JWT_SECRET` | Chave com pelo menos 32 bytes para assinatura dos tokens |
| `ADMIN_EMAIL` / `ADMIN_PASSWORD` | Credenciais do administrador inicial; senha mínima de 12 caracteres |
| `SWAGGER_ENABLED` | Habilita explicitamente Swagger; `false` em produção acadêmica |

`.env` é ignorado pelo Git e pelo Docker. `.env.example` contém apenas valores fictícios para desenvolvimento. No CI, cada job gera credenciais aleatórias, mascara os valores nos logs e remove o arquivo ao encerrar. Nenhum secret precisa ser cadastrado para os ambientes temporários.

### Executar staging e produção simultaneamente na sua máquina

Copie `.env.example` para `.env.staging` e `.env.production`, defina senhas/chaves diferentes e ajuste:

| Arquivo | COMPOSE_PROJECT_NAME | APP_ENVIRONMENT | API_PORT | SWAGGER_ENABLED |
|---|---|---|---|---|
| `.env.staging` | ecocoleta-staging | Staging | 8081 | true |
| `.env.production` | ecocoleta-production | Production | 8082 | false |

```bash
docker compose --env-file .env.staging up -d --build --wait --wait-timeout 240
docker compose --env-file .env.production up -d --build --wait --wait-timeout 240
```

Cada projeto Compose possui seu próprio SQL Server, rede `backend` e volume `sqldata`. URLs: `http://localhost:8081` e `http://localhost:8082`. Esses ambientes locais permanecem ativos até você pará-los. Nos comandos seguintes, sempre repita `--env-file` para operar na stack correta.

## Pipeline CI/CD

Ferramenta: **GitHub Actions**. Arquivos: [ci-cd.yml](.github/workflows/ci-cd.yml) e [deploy-environment.yml](.github/workflows/deploy-environment.yml).

1. **Restore e build:** restaura o NuGet e compila a solução em Release.
2. **Testes:** executa a suíte xUnit, exportando TRX e cobertura Cobertura.
3. **Imagem:** faz build multi-stage com versão igual ao SHA do commit; exporta a imagem e checksum SHA-256 como artefato.
4. **Staging:** verifica o checksum, carrega a imagem, gera credenciais, inicia API e SQL Server e espera os healthchecks.
5. **Validação de staging:** executa smoke HTTP, reinicia os serviços e confirma persistência dos dados; captura tela real e logs.
6. **Produção acadêmica:** executa apenas se staging passar e usa o mesmo artefato de imagem, sem recompilar. Repete os testes, com `ASPNETCORE_ENVIRONMENT=Production` e Swagger desabilitado.
7. **Evidências e limpeza:** publica arquivos por ambiente e remove a stack temporária, inclusive volumes, mesmo após falha.

Um erro de build/teste impede o deploy. Um erro em staging impede produção. O workflow reutilizável declara os GitHub Environments `staging` e `production`, criando histórico de implantação. Nenhuma URL `localhost` é apresentada como site público no GitHub.

**Gatilhos:** push em `main` e `codex/devops-atividade`; pull request para `main`; execução manual. PR executa build/testes/imagem, sem deploy. Alterações apenas em `docs/` e Markdown não disparam o pipeline de push. Depois do merge, o botão manual fica disponível em Actions. A concorrência serializa execuções por branch.

**Permissões:** `contents: read`. A imagem circula como artefato do próprio workflow, dispensando Docker Hub/GHCR e credenciais de registry. Retenção: imagem por 7 dias; evidências por 30 dias. As evidências relevantes da entrega são também versionadas em `docs/evidencias/`.

### Testes e critérios de aprovação

```bash
# Com .NET SDK 8 instalado:
dotnet restore EcoColeta.sln
dotnet build EcoColeta.sln -c Release --no-restore
dotnet test EcoColeta.sln -c Release --no-build --logger trx --collect:"XPlat Code Coverage"
```

A suíte preserva os cenários anteriores de consulta, login, autorização e alerta por ocupação. Acrescenta prontidão com banco/ambiente, liveness e ausência de Swagger quando desabilitado. A factory usa EF Core InMemory, isolado por instância; os smoke tests do deploy validam o provider SQL Server real.

`scripts/smoke.py` verifica HTTP e conteúdo: ambiente/versão corretos, banco conectado, 401 sem token, login válido, cadastro autorizado, descarte de 81 kg em capacidade de 100 kg, alerta automático, relatório e configuração do Swagger. Após reiniciar API + SQL Server, confirma o mesmo ponto com ocupação de 81 kg. Senhas e tokens não são gravados nos relatórios.

Para repetir o smoke em uma stack acadêmica, exporte `ADMIN_EMAIL` e `ADMIN_PASSWORD` correspondentes e execute (o teste cria registros):

```bash
python3 scripts/smoke.py --url http://localhost:8081 --environment Staging --version local --output artifacts/staging
```

## Containerização

O Dockerfile completo está na raiz:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source
COPY src/EcoColeta.Api/EcoColeta.Api.csproj src/EcoColeta.Api/
RUN dotnet restore src/EcoColeta.Api/EcoColeta.Api.csproj
COPY src/ src/
RUN dotnet publish src/EcoColeta.Api/EcoColeta.Api.csproj --no-restore -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
USER root
RUN apt-get update && apt-get install -y --no-install-recommends curl && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
ARG APP_VERSION=local
ENV APP_VERSION=$APP_VERSION ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
HEALTHCHECK --interval=10s --timeout=5s --start-period=60s --retries=12 CMD curl --fail --silent http://localhost:8080/health/ready || exit 1
ENTRYPOINT ["dotnet", "EcoColeta.Api.dll"]
```

O estágio SDK restaura os pacotes antes do código para aproveitar cache e publica somente a API. A imagem final usa ASP.NET Runtime, sem SDK, com usuário `APP_UID` não root e porta 8080. `curl` suporta o healthcheck. O `.dockerignore` exclui Git, testes, documentação, resultados e arquivos de segredo.

```bash
docker build --build-arg APP_VERSION=local -t ecocoleta-api:local .
```

No pipeline, a imagem recebe a tag `ecocoleta-api:<SHA-do-commit>`; a versão também é embutida em `APP_VERSION`. O checksum do arquivo transportado é verificado nos dois ambientes e o ID da imagem é registrado. As bases .NET/SQL usam tags móveis de manutenção; para reprodutibilidade entre builds futuros, uma evolução é fixá-las por digest e atualizá-las periodicamente.

### Orquestração e arquitetura

| Componente | Configuração |
|---|---|
| API | ASP.NET Core 8; porta interna 8080; usuário sem privilégios; healthcheck HTTP |
| Banco | SQL Server 2022 Developer; porta 1433 somente na rede interna |
| Dependência | API aguarda `service_healthy` do banco (`SELECT 1` via sqlcmd) |
| Persistência | Volume nomeado `sqldata` em `/var/opt/mssql` |
| Rede | Bridge `backend`, isolada pelo nome do projeto Compose |
| Configuração | Ambiente, JWT, conexão e seed injetados por variáveis |
| Inicialização | Migrations EF Core e seed; somente uma réplica da API nesta arquitetura |

A readiness testa conexão com o banco e retorna 503 quando indisponível; a liveness responde independentemente do banco. A página inicial consulta a readiness de verdade e exibe ambiente, saúde e versão, permitindo capturas verificáveis. A API só inicia após suas migrations.

## Prints do funcionamento

As evidências da execução validada, links e capturas estão em [docs/evidencias/README.md](docs/evidencias/README.md). O PDF técnico acompanha o ZIP da entrega.

No GitHub, abra **Actions → EcoColeta CI/CD → execução → Artifacts**. Os artefatos por ambiente contêm `ambiente.png`, `health.json`, `smoke.json`, `persistence.json`, ID de imagem, estado dos contêineres e logs. O artefato de testes contém TRX e cobertura. Os links históricos dos jobs permitem conferir build, testes e deploy; os runners são encerrados ao final, portanto suas portas não ficam acessíveis pela internet.

## Tecnologias utilizadas

C#; ASP.NET Core 8; Entity Framework Core 8.0.11; SQL Server 2022; JWT Bearer; BCrypt; Swagger/Swashbuckle; xUnit; Microsoft.AspNetCore.Mvc.Testing; EF Core InMemory; Coverlet; Docker; Docker Compose v2; GitHub Actions; Python 3 para smoke e configuração; Bash e Chrome Headless para captura das telas.

## Estrutura do projeto

| Caminho | Conteúdo |
|---|---|
| `src/EcoColeta.Api/` | Controllers, regras de negócio, repositories, modelos, migrations, configuração e painel |
| `tests/EcoColeta.Tests/` | Testes automatizados e factory |
| `.github/workflows/` | Pipeline principal e deploy reutilizável |
| `Dockerfile` / `docker-compose.yml` | Build da imagem e orquestração |
| `.env.example` | Modelo de configuração sem segredos reais |
| `scripts/` | Smoke, credenciais de CI, captura e empacotamento |
| `docs/evidencias/` | Evidências reais e rastreabilidade |
| `postman/` | Collection de endpoints da API |

O README da etapa anterior foi preservado em `docs/README-WebService-original.md` como histórico. Seus caminhos e credenciais são antigos; use as instruções deste README.

## Desafios encontrados e soluções

- **Corrida de inicialização:** `depends_on` simples iniciava a API antes de o SQL aceitar conexões. Solução: healthcheck SQL, `service_healthy` e `up --wait`.
- **Credenciais no código:** removidas de appsettings e seed. Injeção de configuração e geração aleatória por job.
- **Prova de funcionamento:** substituição de uma verificação limitada ao HTTP 200 por fluxo autenticado com persistência, alerta de negócio e reinício de ambos os serviços.
- **Promoção consistente:** apenas um build de imagem; staging e produção carregam o mesmo artefato validado por checksum.
- **Execução remota:** ambiente de autoria sem Docker e sem execução funcional do SDK; validação feita em runners Linux do GitHub Actions.

## Limites da simulação e evolução para hospedagem permanente

A entrega demonstra CD em dois ambientes acadêmicos, mas não provisiona servidor público. Para operação permanente, é necessário um host Linux ou serviço cloud, registry para as imagens, credenciais por ambiente, backups e monitoramento. O deploy deve executar no host de destino e manter os serviços ativos, removendo a etapa de limpeza que é própria desta demonstração.

Antes de uma operação comercial: usar licença/edição SQL Server adequada, usuário de banco com privilégios mínimos (esta demonstração utiliza `sa`), TLS via proxy, segredo persistente em cofre e estratégia de migração/backup. `TrustServerCertificate=True` é restrito à rede acadêmica. Um rollback de imagem não desfaz migrations: restaure uma imagem compatível com o schema ou use backup validado. Não há rollback automático nesta versão.

## Checklist de entrega

O checklist consolidado com o estado verificado da entrega está no final de [docs/evidencias/README.md](docs/evidencias/README.md) e no PDF técnico. Não considerar a simples presença do YAML uma evidência de execução: conferir os resultados e os links registrados.

## Referências técnicas

- [Docker: ordem de inicialização](https://docs.docker.com/compose/how-tos/startup-order/)
- [Docker: referência dos serviços Compose](https://docs.docker.com/reference/compose-file/services/)
- [Microsoft: contêineres no .NET 8](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8/containers)
- [GitHub: ambientes de implantação](https://docs.github.com/en/actions/concepts/workflows-and-actions/deployment-environments)
