# language: pt
@relatorio @governanca
Funcionalidade: Consolidação consistente dos indicadores de resíduos
  A estimativa de impacto é uma regra da aplicação e não uma medição ambiental certificada

  @REL-01
  Cenário: Consultar resumo público de impacto
    Quando envio "GET" para "/api/relatorios-impacto/resumo"
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "relatorio"
    E o campo "geradoEm" não deve estar vazio

  @REL-02
  Cenário: Refletir descarte nos totais e na estimativa de 75 por cento
    Dado que existe um ponto de coleta ativo para plástico com capacidade de 100 kg
    E que capturei os totais atuais do relatório
    Quando envio "POST" para "/api/registros-residuos" com:
      """
      {"pontoColetaId":${pontoId},"tipoResiduo":1,"pesoKg":12,"origem":"BDD relatório"}
      """
    Então o status HTTP deve ser 201
    E o contrato JSON deve ser "registro"
    E o relatório deve refletir mais 12 kg de plástico
