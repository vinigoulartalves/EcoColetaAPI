# language: pt
@alertas @ambiental
Funcionalidade: Alertas para orientar a coleta antes do transbordamento

  @ALT-01
  Esquema do Cenário: Classificar ocupação nos limites de atenção e criticidade
    Dado que existe um ponto de coleta ativo para plástico com capacidade de 100 kg
    Quando envio "POST" para "/api/registros-residuos" com:
      """
      {"pontoColetaId":${pontoId},"tipoResiduo":1,"pesoKg":<peso>,"origem":"BDD limites"}
      """
    Então o status HTTP deve ser 201
    E o contrato JSON deve ser "registro"
    E a ocupação do ponto deve ser <peso> kg
    E deve haver <quantidade> alertas abertos de nível "<nivel>" no ponto
    Exemplos:
      | peso   | quantidade | nivel   |
      | 79.99  | 0          | nenhum  |
      | 80     | 1          | Atencao |
      | 99.99  | 1          | Atencao |
      | 100    | 1          | Critico |
      | 100.01 | 1          | Critico |

  @ALT-02
  Cenário: Consultar alertas com filtros publicamente
    Dado que existe um alerta aberto no ponto
    E que estou sem autenticação
    Quando envio "GET" para "/api/alertas-coleta?pontoColetaId=${pontoId}&nivel=1&resolvido=false"
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "alertas"
    E o campo numérico "totalItens" deve ser 1
    E o campo "itens[0].nivel" deve ser "Atencao"
    E o campo "itens[0].pontoColetaId" deve ser "${pontoId}"

  @ALT-03
  Cenário: Resolver alerta e confirmar persistência
    Dado que existe um alerta aberto no ponto
    Quando envio "PATCH" para "/api/alertas-coleta/${alertaId}/resolver"
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "alerta"
    E o campo "resolvido" deve ser "true"
    E o campo "resolvidoEm" não deve estar vazio
    E o alerta resolvido deve estar persistido

  @ALT-04
  Cenário: Impedir resolução repetida
    Dado que existe um alerta aberto no ponto
    E que o alerta já foi resolvido
    Quando envio "PATCH" para "/api/alertas-coleta/${alertaId}/resolver"
    Então o status HTTP deve ser 400
    E o contrato JSON deve ser "erro"
    E a mensagem deve conter "já está resolvido"

  @ALT-05
  Cenário: Resolver alerta inexistente
    Dado que estou autenticado como administrador
    Quando envio "PATCH" para "/api/alertas-coleta/2147483647/resolver"
    Então o status HTTP deve ser 404
    E o contrato JSON deve ser "erro"

  @ALT-06
  Cenário: Recalcular sem duplicar alerta aberto
    Dado que existe um alerta aberto no ponto
    Quando envio "POST" para "/api/alertas-coleta/recalcular"
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "recalculo"
    E a mensagem deve conter "Recálculo concluído"
    E deve haver 1 alertas abertos de nível "Atencao" no ponto

  @ALT-07
  Cenário: Resolver alerta automaticamente após redução percentual da ocupação
    Dado que existe um alerta aberto no ponto
    E que a capacidade do ponto foi alterada para 200 kg
    Quando envio "POST" para "/api/alertas-coleta/recalcular"
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "recalculo"
    E deve haver 0 alertas abertos de nível "nenhum" no ponto

  @ALT-08
  Cenário: Substituir atenção por crítico sem manter dois alertas abertos
    Dado que existe um alerta aberto no ponto
    Quando envio "POST" para "/api/registros-residuos" com:
      """
      {"pontoColetaId":${pontoId},"tipoResiduo":1,"pesoKg":20,"origem":"BDD escalada"}
      """
    Então o status HTTP deve ser 201
    E o contrato JSON deve ser "registro"
    E deve haver 1 alertas abertos de nível "Critico" no ponto
