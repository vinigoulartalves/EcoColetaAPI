# Testes Automatizados BDD - EcoColetaAPI

Atividade FIAP: Testes Automatizados. Projeto ESG reutilizado: EcoColetaAPI, em ASP.NET Core 8.
Suíte independente em Java 17, Cucumber 7.20.1, JUnit Platform 1.11.4, REST Assured 5.5.0 e Maven 3.9.9.

## Execução completa recomendada

Pré-requisitos: Docker Engine/Desktop iniciado, Docker Compose v2, Python 3, Bash, acesso à internet para baixar imagens e dependências. No Windows, usar WSL2 ou Git Bash com Python 3 e Docker disponíveis. Reserve recursos para SQL Server: pelo menos 2 GB só para o banco; recomenda-se 6 GB de RAM livres para o conjunto.

Na raiz do projeto:

```bash
bash scripts/bdd/run.sh
```

O script gera credenciais temporárias, recria **somente** o projeto Compose `ecocoleta-bdd`, sobe SQL Server 2022 e a API real, espera a verificação de saúde, prepara a fixture e executa a suíte. Ao final, coleta logs e remove os containers e o volume desse ambiente. Não aponta para produção e não reutiliza o Compose de deploy. Não execute duas instâncias desse script simultaneamente no mesmo Docker.

Os testes modificam dados. A base `EcoColetaBdd` é descartável e recriada a cada execução. Cada cenário cria pontos próprios; a execução é sequencial. Não existe DELETE público: o isolamento ocorre no ciclo de vida do banco. Para testar criação de destinação, o script remove previamente apenas a destinação `Outros` (tipo 9) do seed do ambiente isolado. A criação desse tipo ocorre em um único cenário; não há dependência de ordem entre cenários. O valor 99 é usado para consulta de tipo sem destinação.

Não execute a suíte contra dados pessoais ou ambientes compartilhados. `BASE_URL` aceita somente localhost/127.0.0.1 ou o serviço `api` do Compose. Não há credenciais reais versionadas; `.env.bdd` é ignorado pelo Git e excluído no encerramento do script. Senhas e tokens são removidos das evidências HTTP.

## Relatórios gerados

- `artifacts/bdd/cucumber/cucumber.html`: relatório interativo com cenários, passos e HTTP sanitizado.
- `artifacts/bdd/cucumber/cucumber.json`: resultado Cucumber para processamento.
- `artifacts/bdd/cucumber/cucumber.xml`: resultado JUnit.
- `artifacts/bdd/cucumber/http-evidence.jsonl`: chamadas e respostas sanitizadas por cenário.
- `artifacts/bdd/summary.json` e `summary.md`: cobertura de operações e resultados.
- `artifacts/bdd/surefire-reports/`: execução dos testes de contrato e da suíte.
- `artifacts/bdd/openapi.json`: especificação extraída da API em execução.
- `artifacts/bdd/compose-ps.txt`, `services.log` e `artifacts/bdd-execution.log`: serviços e execução.

O resumo falha se houver passos pendentes, ignorados ou falhos, se faltar evidência ou se alguma das 16 operações não tiver uma resposta de sucesso testada. Também verifica se o OpenAPI passou a expor operações ainda não contempladas no inventário. A contagem de operações não equivale à cobertura de linhas de código ou a todos os valores possíveis de entrada.

## Executar pelo GitHub Actions

Workflow: `.github/workflows/bdd-tests.yml`, **EcoColeta BDD e contratos**.
Executa em push em `main` e `codex/testes-bdd`, pull request para `main` ou acionamento manual na aba Actions. O job usa o mesmo script local, em runner Ubuntu, com SQL Server real. Não faz deploy. Os relatórios são publicados como artifact por 30 dias. Baixe o artifact e abra `cucumber/cucumber.html` em um navegador.

## Execução Maven para desenvolvimento

Com uma instância **isolada, preparada e descartável** da API rodando, Java 17 e Maven instalados:

```bash
export BASE_URL=http://localhost:8080
export ADMIN_EMAIL=admin@ecocoleta.test
export ADMIN_PASSWORD='senha-do-seu-ambiente-de-teste'
mvn -B -ntp -f tests/EcoColeta.Bdd/pom.xml clean verify
```

Essa alternativa não prepara nem limpa o banco: providencie o mesmo seed e a reserva do tipo 9 descritos acima. Resultados em `tests/EcoColeta.Bdd/target/`. Use `clean` entre execuções para não acumular evidências. Para filtrar um grupo, adicione `-Dcucumber.filter.tags=@alertas`; o filtro não serve como execução completa para entrega.

Para verificar apenas se o validador detecta contratos quebrados, sem API:

```bash
mvn -B -ntp -f tests/EcoColeta.Bdd/pom.xml -Dtest=ContractValidationTest test
```

São cinco verificações: contrato válido, campo obrigatório ausente, tipo incorreto, enum desconhecido e campo não documentado. Elas não substituem os cenários HTTP.

## Contratos e comportamento

Os 15 arquivos de schema usam JSON Schema draft-04, compatível com o validador selecionado. Definem campos obrigatórios, tipos, enums, limites, formatos e ausência de campos extras. Listagens também validam cada item. Os erros de negócio (`ErroResponse`) e os erros de modelo (`ValidationProblemDetails`) têm contratos diferentes. Respostas 401 do middleware JWT não possuem corpo e são validadas dessa forma.

A API não possui endpoint DELETE. Registros aceitos devem refletir na ocupação e nas consultas; registros rejeitados por peso, ponto inativo ou tipo incompatível não podem deixar alterações. Os alertas são testados abaixo e nos limites de 80% e 100%, incluindo escalada e prevenção de duplicidade. O recálculo é global, por isso a asserção de duplicidade filtra pelo ponto criado pelo próprio cenário.

A estimativa de impacto positivo é `total de resíduos × 0,75`, arredondada a duas casas. É uma regra existente no projeto, sem validação científica de impacto ambiental. O vínculo ESG demonstrado pelos testes é a consistência e a rastreabilidade dos dados de descarte, as orientações por tipo e os avisos operacionais.

## Estrutura

- `tests/EcoColeta.Bdd/src/test/resources/features/`: sete funcionalidades Gherkin em português.
- `tests/EcoColeta.Bdd/src/test/resources/schemas/`: contratos JSON.
- `tests/EcoColeta.Bdd/src/test/java/br/com/ecocoleta/`: passos, runner e verificações do validador.
- `docker-compose.bdd.yml`: banco, API e runner de testes isolados.
- `scripts/bdd/`: execução, análise dos resultados e empacotamento.
- `docs/bdd/coverage.md`: rastreabilidade entre operações e cenários.

## Problemas comuns

- Docker indisponível: iniciar Docker Desktop/Engine; verificar `docker compose version`.
- SQL Server não saudável: conferir memória, virtualização e logs em `artifacts/bdd/services.log`.
- Download bloqueado: liberar acesso a Maven Central, Docker Hub e Microsoft Container Registry.
- Falha em `DES-04`: executar o script completo, que recria a base e reserva o tipo 9.
- Falha em contratos: comparar a resposta com o DTO; alterar o schema só se a mudança de contrato for intencional.

## Fontes técnicas

- https://cucumber.io/docs/gherkin/reference/
- https://cucumber.io/docs/cucumber/api/
- https://github.com/rest-assured/rest-assured/wiki/Usage
- https://docs.docker.com/compose/how-tos/startup-order/
- https://docs.github.com/en/actions/writing-workflows
