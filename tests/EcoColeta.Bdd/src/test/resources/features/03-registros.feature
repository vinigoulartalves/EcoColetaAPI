# language: pt
@registros @ambiental
Funcionalidade: Registro rastreável de resíduos compatíveis com o ponto

  @REG-01
  Cenário: Registrar descarte válido e atualizar ocupação
    Dado que existe um ponto de coleta ativo para plástico com capacidade de 100 kg
    Quando envio "POST" para "/api/registros-residuos" com:
      """
      {"pontoColetaId":${pontoId},"tipoResiduo":1,"pesoKg":25.5,"origem":"Comunidade FIAP","observacao":"Material separado"}
      """
    Então o status HTTP deve ser 201
    E o contrato JSON deve ser "registro"
    E o campo "tipoResiduo" deve ser "Plastico"
    E o campo numérico "pesoKg" deve ser 25.5
    E o campo "origem" deve ser "Comunidade FIAP"
    E a ocupação do ponto deve ser 25.5 kg
    E deve haver 1 registros no ponto

  @REG-02
  Cenário: Consultar registros com filtros e acesso público
    Dado que existe um ponto de coleta ativo para plástico com capacidade de 100 kg
    E que foram registrados 10 kg de plástico no ponto
    E que estou sem autenticação
    Quando envio "GET" para "/api/registros-residuos?pontoColetaId=${pontoId}&tipoResiduo=1&pagina=1&tamanhoPagina=5"
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "registros"
    E o campo numérico "totalItens" deve ser 1
    E o campo "itens[0].pontoColetaId" deve ser "${pontoId}"
    E o campo "itens[0].tipoResiduo" deve ser "Plastico"
    E o campo numérico "itens[0].pesoKg" deve ser 10

  @REG-03
  Esquema do Cenário: Rejeitar peso fora da faixa permitida
    Dado que existe um ponto de coleta ativo para plástico com capacidade de 100 kg
    Quando envio "POST" para "/api/registros-residuos" com:
      """
      {"pontoColetaId":${pontoId},"tipoResiduo":1,"pesoKg":<peso>,"origem":"BDD FIAP"}
      """
    Então o status HTTP deve ser 400
    E o contrato JSON deve ser "validacao"
    E o erro de validação deve mencionar "PesoKg"
    E a ocupação do ponto deve ser 0 kg
    E deve haver 0 registros no ponto
    Exemplos:
      | peso  |
      | 0     |
      | -1    |
      | 10001 |

  @REG-04
  Cenário: Rejeitar descarte em ponto inativo
    Dado que existe um ponto de coleta ativo para plástico com capacidade de 100 kg
    E que o ponto está inativo
    Quando envio "POST" para "/api/registros-residuos" com:
      """
      {"pontoColetaId":${pontoId},"tipoResiduo":1,"pesoKg":10,"origem":"BDD FIAP"}
      """
    Então o status HTTP deve ser 400
    E o contrato JSON deve ser "erro"
    E a mensagem deve conter "ponto de coleta inativo"
    E a ocupação do ponto deve ser 0 kg
    E deve haver 0 registros no ponto

  @REG-05
  Cenário: Rejeitar resíduo incompatível sem alterar os dados
    Dado que existe um ponto de coleta ativo para plástico com capacidade de 100 kg
    Quando envio "POST" para "/api/registros-residuos" com:
      """
      {"pontoColetaId":${pontoId},"tipoResiduo":2,"pesoKg":10,"origem":"BDD FIAP"}
      """
    Então o status HTTP deve ser 400
    E o contrato JSON deve ser "erro"
    E a mensagem deve conter "aceita apenas resíduos do tipo Plastico"
    E a ocupação do ponto deve ser 0 kg
    E deve haver 0 registros no ponto

  @REG-06
  Cenário: Rejeitar registro associado a ponto inexistente
    Dado que estou autenticado como administrador
    Quando envio "POST" para "/api/registros-residuos" com:
      """
      {"pontoColetaId":2147483647,"tipoResiduo":1,"pesoKg":10,"origem":"BDD FIAP"}
      """
    Então o status HTTP deve ser 404
    E o contrato JSON deve ser "erro"
    E a mensagem deve conter "não encontrado"
