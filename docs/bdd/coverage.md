# Matriz de cobertura por operação

Cada ID corresponde à tag no arquivo `.feature`; esquemas de cenário expandem cada linha de Exemplos em um teste. Os contratos citados estão em `src/test/resources/schemas/` do módulo Java.

| Método | Rota | Cenários principais | Positivo | Negativo | Schema de sucesso |
|---|---|---|---|---|---|
| POST | /api/auth/login | AUTH-01,02,03 | 200, token e perfil | 401 credenciais; 400 e-mail | login |
| GET | /api/pontos-coleta | PTO-02,03,04 | 200, filtros, paginação e vazio | parâmetros normalizados conforme regra | pontos |
| GET | /api/pontos-coleta/{id} | PTO-01,05 | 200 e persistência | 404 inexistente | ponto |
| POST | /api/pontos-coleta | PTO-01,06; AUTH-04 | 201, Location e consulta | 400 campos; 401 sem token | ponto |
| PUT | /api/pontos-coleta/{id} | PTO-07,08,09; AUTH-04 | 200 e persistência | 400 capacidade; 404; 401 | ponto |
| GET | /api/registros-residuos | REG-02 | 200 e filtros | consulta de vazio nas rejeições | registros |
| POST | /api/registros-residuos | REG-01,03,04,05,06; AUTH-04 | 201 e atualização da ocupação | 400 peso/inativo/tipo; 404 ponto; 401 | registro |
| GET | /api/destinacoes-residuos | DES-01 | 200 e paginação | não há falha de negócio prevista para listagem válida | destinacoes |
| GET | /api/destinacoes-residuos/{tipoResiduo} | DES-02,03,04 | 200 e instruções | 404 sem destinação; 400 enum inválido | destinacao |
| POST | /api/destinacoes-residuos | DES-04,05,06; AUTH-04 | 201 e persistência | 400 duplicidade/campos; 401 | destinacao |
| GET | /api/alertas-coleta | ALT-01,02,03,06,07,08 | 200, filtros e níveis | ausência de alerta abaixo de 80% | alertas |
| PATCH | /api/alertas-coleta/{id}/resolver | ALT-03,04,05; AUTH-04 | 200 e persistência | 400 repetido; 404; 401 | alerta |
| POST | /api/alertas-coleta/recalcular | ALT-06,07; AUTH-04,05 | 200, sem duplicação e resolução automática | 401 sem token/inválido | recalculo |
| GET | /api/relatorios-impacto/resumo | REL-01,02 | 200, totais e fórmula | não há falha de negócio prevista para consulta válida | relatorio |
| GET | /health/live | SAU-01 | 200 processo disponível | não simulado | health-live |
| GET | /health/ready | SAU-02 | 200 banco conectado | indisponibilidade 503 não simulada | health-ready |

## Limites da cobertura

Não são testes de carga, segurança ofensiva, interface, concorrência, conformidade legal ou comprovação de benefícios ambientais. Não é medida de cobertura de linhas. O perfil Admin é obtido por login real; a suíte testa falta e invalidade de token, mas não contém um usuário autenticado sem papel Admin (403). A execução sequencial protege as verificações de relatório global. A indisponibilidade do banco (503) não é injetada nesta atividade.
