# language: pt
@pontos @ambiental
Funcionalidade: Gestão e consulta dos pontos de coleta

  @PTO-01
  Cenário: Cadastrar ponto e consultar os dados persistidos
    Dado que estou autenticado como administrador
    Quando envio "POST" para "/api/pontos-coleta" com:
      """
      {"nome":"EcoPonto FIAP","endereco":"Rua ESG, 100","bairro":"Centro","cidade":"Palhoca","latitude":-27.64,"longitude":-48.67,"tipoResiduoAceito":1,"capacidadeMaximaKg":100}
      """
    Então o status HTTP deve ser 201
    E o contrato JSON deve ser "ponto"
    E o campo "nome" deve ser "EcoPonto FIAP"
    E o campo numérico "ocupacaoAtualKg" deve ser 0
    E o registro criado deve ser consultável

  @PTO-02
  Cenário: Filtrar pontos com paginação e acesso público
    Dado que existe um ponto de coleta ativo para plástico com capacidade de 100 kg
    E que estou sem autenticação
    Quando envio "GET" para "/api/pontos-coleta?cidade=Palhoca BDD&bairro=Centro BDD&tipoResiduo=1&ativo=true&pagina=1&tamanhoPagina=1"
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "pontos"
    E o campo "itens[0].cidade" deve ser "Palhoca BDD"
    E o campo "itens[0].bairro" deve ser "Centro BDD"
    E o campo "itens[0].tipoResiduoAceito" deve ser "Plastico"
    E o campo "itens[0].ativo" deve ser "true"
    E o campo numérico "tamanhoPagina" deve ser 1
    E o campo numérico "itens.size()" deve ser 1

  @PTO-03
  Cenário: Normalizar paginação fora dos limites
    Quando envio "GET" para "/api/pontos-coleta?pagina=0&tamanhoPagina=999"
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "pontos"
    E o campo numérico "paginaAtual" deve ser 1
    E o campo numérico "tamanhoPagina" deve ser 50

  @PTO-04
  Cenário: Retornar lista vazia para filtro sem correspondência
    Quando envio "GET" para "/api/pontos-coleta?cidade=CidadeInexistenteBDD999999"
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "pontos"
    E o campo numérico "totalItens" deve ser 0
    E o campo numérico "itens.size()" deve ser 0

  @PTO-05
  Cenário: Consultar ponto inexistente
    Quando envio "GET" para "/api/pontos-coleta/2147483647"
    Então o status HTTP deve ser 404
    E o contrato JSON deve ser "erro"
    E a mensagem deve conter "não encontrado"

  @PTO-06
  Esquema do Cenário: Rejeitar cadastro inválido
    Dado que estou autenticado como administrador
    Quando envio "POST" para "/api/pontos-coleta" com:
      """
      {"nome":"<nome>","endereco":"Rua ESG, 100","bairro":"Centro","cidade":"Palhoca","latitude":<latitude>,"longitude":-48.67,"tipoResiduoAceito":<tipo>,"capacidadeMaximaKg":<capacidade>}
      """
    Então o status HTTP deve ser 400
    E o contrato JSON deve ser "validacao"
    E o erro de validação deve mencionar "<campo>"
    Exemplos:
      | nome          | latitude | tipo | capacidade | campo               |
      |               | -27.64   | 1    | 100        | Nome                |
      | Ponto válido  | 91       | 1    | 100        | Latitude            |
      | Ponto válido  | -27.64   | 99   | 100        | TipoResiduoAceito   |
      | Ponto válido  | -27.64   | 1    | 0          | CapacidadeMaximaKg  |

  @PTO-07
  Cenário: Atualizar ponto e conferir persistência
    Dado que existe um ponto de coleta ativo para plástico com capacidade de 100 kg
    Quando envio "PUT" para "/api/pontos-coleta/${pontoId}" com:
      """
      {"nome":"Ponto Atualizado BDD","endereco":"Rua ESG, 200","bairro":"Centro","cidade":"Palhoca","latitude":-27.64,"longitude":-48.67,"tipoResiduoAceito":1,"capacidadeMaximaKg":150,"ativo":true}
      """
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "ponto"
    E o campo "nome" deve ser "Ponto Atualizado BDD"
    E a atualização do ponto deve estar persistida

  @PTO-08
  Cenário: Rejeitar capacidade abaixo da ocupação atual
    Dado que existe um ponto de coleta ativo para plástico com capacidade de 100 kg
    E que foram registrados 50 kg de plástico no ponto
    Quando envio "PUT" para "/api/pontos-coleta/${pontoId}" com:
      """
      {"nome":"Ponto capacidade BDD","endereco":"Rua ESG, 200","bairro":"Centro","cidade":"Palhoca","latitude":-27.64,"longitude":-48.67,"tipoResiduoAceito":1,"capacidadeMaximaKg":40,"ativo":true}
      """
    Então o status HTTP deve ser 400
    E o contrato JSON deve ser "erro"
    E a mensagem deve conter "capacidade máxima não pode ser menor"
    E a ocupação do ponto deve ser 50 kg

  @PTO-09
  Cenário: Atualizar ponto inexistente
    Dado que estou autenticado como administrador
    Quando envio "PUT" para "/api/pontos-coleta/2147483647" com:
      """
      {"nome":"Ponto inexistente","endereco":"Rua ESG, 200","bairro":"Centro","cidade":"Palhoca","latitude":-27.64,"longitude":-48.67,"tipoResiduoAceito":1,"capacidadeMaximaKg":150,"ativo":true}
      """
    Então o status HTTP deve ser 404
    E o contrato JSON deve ser "erro"
