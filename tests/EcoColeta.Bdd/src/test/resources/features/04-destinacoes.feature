# language: pt
@destinacoes @ambiental
Funcionalidade: Orientações de destinação de resíduos

  @DES-01
  Cenário: Listar destinações publicamente com paginação
    Quando envio "GET" para "/api/destinacoes-residuos?pagina=1&tamanhoPagina=2"
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "destinacoes"
    E o campo numérico "tamanhoPagina" deve ser 2
    E o campo numérico "itens.size()" deve ser 2

  @DES-02
  Cenário: Consultar instruções de descarte de plástico
    Quando envio "GET" para "/api/destinacoes-residuos/1"
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "destinacao"
    E o campo "tipoResiduo" deve ser "Plastico"
    E o campo "instrucoesDescarte" não deve estar vazio

  @DES-03
  Cenário: Consultar tipo sem destinação
    Quando envio "GET" para "/api/destinacoes-residuos/99"
    Então o status HTTP deve ser 404
    E o contrato JSON deve ser "erro"

  @DES-04
  Cenário: Cadastrar destinação disponível e consultar sua persistência
    Dado que estou autenticado como administrador
    E que não há destinação cadastrada para Outros
    Quando envio "POST" para "/api/destinacoes-residuos" com:
      """
      {"tipoResiduo":9,"descricao":"Outros resíduos BDD","instrucoesDescarte":"Consultar orientação local de descarte.","reciclavel":false,"riscoAmbiental":"Variável"}
      """
    Então o status HTTP deve ser 201
    E o contrato JSON deve ser "destinacao"
    E o campo "tipoResiduo" deve ser "Outros"
    E a destinação criada deve ser consultável por tipo

  @DES-05
  Cenário: Rejeitar destinação duplicada para plástico
    Dado que estou autenticado como administrador
    Quando envio "POST" para "/api/destinacoes-residuos" com:
      """
      {"tipoResiduo":1,"descricao":"Plástico duplicado","instrucoesDescarte":"Separar e lavar antes do descarte.","reciclavel":true,"riscoAmbiental":"Baixo"}
      """
    Então o status HTTP deve ser 400
    E o contrato JSON deve ser "erro"
    E a mensagem deve conter "Já existe destinação cadastrada"

  @DES-06
  Cenário: Rejeitar cadastro sem instruções de descarte
    Dado que estou autenticado como administrador
    Quando envio "POST" para "/api/destinacoes-residuos" com:
      """
      {"tipoResiduo":9,"descricao":"Outros resíduos BDD","instrucoesDescarte":"","reciclavel":false,"riscoAmbiental":"Variável"}
      """
    Então o status HTTP deve ser 400
    E o contrato JSON deve ser "validacao"
    E o erro de validação deve mencionar "InstrucoesDescarte"
