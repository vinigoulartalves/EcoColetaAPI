# language: pt
@autenticacao @governanca
Funcionalidade: Autenticação e proteção das operações administrativas
  Para preservar a integridade dos dados ambientais
  Como administrador do EcoColeta
  Quero autenticar minhas operações

  @AUTH-01
  Cenário: Autenticar administrador com credenciais válidas
    Dado que estou sem autenticação
    Quando envio "POST" para "/api/auth/login" com:
      """
      {"email":"${adminEmail}","senha":"${adminPassword}"}
      """
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "login"
    E o campo "token" não deve estar vazio
    E o campo "role" deve ser "Admin"
    E o campo "email" deve ser "${adminEmail}"

  @AUTH-02
  Esquema do Cenário: Rejeitar credenciais inválidas sem revelar qual campo está incorreto
    Dado que estou sem autenticação
    Quando envio "POST" para "/api/auth/login" com:
      """
      {"email":"<email>","senha":"<senha>"}
      """
    Então o status HTTP deve ser 401
    E o contrato JSON deve ser "erro"
    E a mensagem deve conter "E-mail ou senha inválidos."
    Exemplos:
      | email                    | senha             |
      | ${adminEmail}            | SenhaIncorreta123 |
      | inexistente@example.test | ${adminPassword}  |

  @AUTH-03
  Cenário: Rejeitar login com e-mail malformado
    Quando envio "POST" para "/api/auth/login" com:
      """
      {"email":"email-invalido","senha":"SenhaValida123"}
      """
    Então o status HTTP deve ser 400
    E o contrato JSON deve ser "validacao"
    E o erro de validação deve mencionar "Email"

  @AUTH-04
  Esquema do Cenário: Bloquear todas as operações protegidas sem token
    Dado que estou sem autenticação
    Quando envio "<metodo>" para "<rota>" com:
      """
      {}
      """
    Então o status HTTP deve ser 401
    E a resposta deve estar sem corpo
    Exemplos:
      | metodo | rota                            |
      | POST   | /api/pontos-coleta              |
      | PUT    | /api/pontos-coleta/1            |
      | POST   | /api/registros-residuos         |
      | POST   | /api/destinacoes-residuos       |
      | PATCH  | /api/alertas-coleta/1/resolver   |
      | POST   | /api/alertas-coleta/recalcular   |

  @AUTH-05
  Cenário: Rejeitar token inválido
    Dado que uso um token inválido
    Quando envio "POST" para "/api/alertas-coleta/recalcular"
    Então o status HTTP deve ser 401
    E a resposta deve estar sem corpo
