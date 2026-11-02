# language: pt
@saude
Funcionalidade: Disponibilidade da aplicação e do banco

  @SAU-01
  Cenário: Verificar processo disponível
    Quando envio "GET" para "/health/live"
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "health-live"
    E o campo "status" deve ser "Healthy"

  @SAU-02
  Cenário: Verificar conexão disponível com o banco
    Quando envio "GET" para "/health/ready"
    Então o status HTTP deve ser 200
    E o contrato JSON deve ser "health-ready"
    E o campo "database" deve ser "Connected"
