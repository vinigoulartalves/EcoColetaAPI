/**
 * EcoColeta API - Seed de dados MongoDB
 * 5 collections com 10+ documentos cada, demonstrando esquema flexível.
 */

function getSeedData() {
  const now = new Date();
  const daysAgo = (n) => new Date(now.getTime() - n * 24 * 60 * 60 * 1000);

  const pontosColeta = [
    {
      nome: "EcoPonto Centro",
      endereco: "Rua das Flores, 100",
      bairro: "Centro",
      cidade: "São Paulo",
      localizacao: { type: "Point", coordinates: [-46.6333, -23.5505] },
      tipoResiduoAceito: "Plastico",
      capacidadeMaximaKg: 500,
      ocupacaoAtualKg: 120,
      ativo: true,
      criadoEm: daysAgo(90),
      horarioFuncionamento: { abertura: "08:00", fechamento: "18:00", diasUteis: true }
    },
    {
      nome: "EcoPonto Jardins",
      endereco: "Av. Paulista, 500",
      bairro: "Jardins",
      cidade: "São Paulo",
      localizacao: { type: "Point", coordinates: [-46.6559, -23.5614] },
      tipoResiduoAceito: "Papel",
      capacidadeMaximaKg: 300,
      ocupacaoAtualKg: 85,
      ativo: true,
      criadoEm: daysAgo(60),
      contato: { telefone: "(11) 3000-1000", email: "jardins@ecocoleta.com.br" }
    },
    {
      nome: "EcoPonto Vila Mariana",
      endereco: "Rua Domingos de Morais, 200",
      bairro: "Vila Mariana",
      cidade: "São Paulo",
      localizacao: { type: "Point", coordinates: [-46.6342, -23.5890] },
      tipoResiduoAceito: "Vidro",
      capacidadeMaximaKg: 400,
      ocupacaoAtualKg: 320,
      ativo: true,
      criadoEm: daysAgo(45)
    },
    {
      nome: "EcoPonto Pinheiros",
      endereco: "Rua dos Pinheiros, 800",
      bairro: "Pinheiros",
      cidade: "São Paulo",
      localizacao: { type: "Point", coordinates: [-46.6912, -23.5678] },
      tipoResiduoAceito: "Metal",
      capacidadeMaximaKg: 600,
      ocupacaoAtualKg: 150,
      ativo: true,
      criadoEm: daysAgo(30),
      certificacoes: ["ISO 14001", "Selo Verde SP"]
    },
    {
      nome: "EcoPonto Mooca",
      endereco: "Rua da Mooca, 1500",
      bairro: "Mooca",
      cidade: "São Paulo",
      localizacao: { type: "Point", coordinates: [-46.5945, -23.5540] },
      tipoResiduoAceito: "Organico",
      capacidadeMaximaKg: 250,
      ocupacaoAtualKg: 200,
      ativo: true,
      criadoEm: daysAgo(25),
      parceria: { ong: "Composta Cidade", tipo: "compostagem comunitária" }
    },
    {
      nome: "EcoPonto Santana",
      endereco: "Av. Cruzeiro do Sul, 1200",
      bairro: "Santana",
      cidade: "São Paulo",
      localizacao: { type: "Point", coordinates: [-46.6267, -23.5050] },
      tipoResiduoAceito: "Eletronico",
      capacidadeMaximaKg: 350,
      ocupacaoAtualKg: 45,
      ativo: true,
      criadoEm: daysAgo(20)
    },
    {
      nome: "EcoPonto Tatuapé",
      endereco: "Rua Serra de Bragança, 500",
      bairro: "Tatuapé",
      cidade: "São Paulo",
      localizacao: { type: "Point", coordinates: [-46.5756, -23.5412] },
      tipoResiduoAceito: "PilhasBaterias",
      capacidadeMaximaKg: 100,
      ocupacaoAtualKg: 30,
      ativo: true,
      criadoEm: daysAgo(15),
      seguranca: { recipienteBlindado: true, sinalizacao: "Resíduo perigoso" }
    },
    {
      nome: "EcoPonto Campinas Centro",
      endereco: "Av. Francisco Glicério, 300",
      bairro: "Centro",
      cidade: "Campinas",
      localizacao: { type: "Point", coordinates: [-47.0608, -22.9035] },
      tipoResiduoAceito: "OleoCozinha",
      capacidadeMaximaKg: 200,
      ocupacaoAtualKg: 80,
      ativo: true,
      criadoEm: daysAgo(10)
    },
    {
      nome: "EcoPonto Guarulhos",
      endereco: "Av. Tiradentes, 700",
      bairro: "Centro",
      cidade: "Guarulhos",
      localizacao: { type: "Point", coordinates: [-46.5333, -23.4628] },
      tipoResiduoAceito: "Plastico",
      capacidadeMaximaKg: 450,
      ocupacaoAtualKg: 410,
      ativo: true,
      criadoEm: daysAgo(8),
      alertaAtivo: true
    },
    {
      nome: "EcoPonto Inativo Teste",
      endereco: "Rua Teste, 1",
      bairro: "Teste",
      cidade: "São Paulo",
      localizacao: { type: "Point", coordinates: [-46.6000, -23.5500] },
      tipoResiduoAceito: "Outros",
      capacidadeMaximaKg: 100,
      ocupacaoAtualKg: 0,
      ativo: false,
      criadoEm: daysAgo(5),
      motivoInativacao: "Reforma estrutural"
    },
    {
      nome: "EcoPonto Osasco",
      endereco: "Av. dos Autonomistas, 2000",
      bairro: "Centro",
      cidade: "Osasco",
      localizacao: { type: "Point", coordinates: [-46.7917, -23.5329] },
      tipoResiduoAceito: "Papel",
      capacidadeMaximaKg: 280,
      ocupacaoAtualKg: 95,
      ativo: true,
      criadoEm: daysAgo(3),
      acessibilidade: { rampa: true, estacionamento: true }
    }
  ];

  const destinacoesResiduos = [
    { tipoResiduo: "Plastico", descricao: "Plásticos recicláveis", instrucoesDescarte: "Lavar e secar antes do descarte.", reciclavel: true, riscoAmbiental: "Baixo quando reciclado", impactoESG: { pilar: "Ambiental", reducaoCO2KgPorTonelada: 1500 } },
    { tipoResiduo: "Papel", descricao: "Papéis e papelões", instrucoesDescarte: "Manter seco e livre de gordura.", reciclavel: true, riscoAmbiental: "Baixo", impactoESG: { pilar: "Ambiental", reducaoCO2KgPorTonelada: 900 } },
    { tipoResiduo: "Vidro", descricao: "Vidros em geral", instrucoesDescarte: "Separar por cor quando possível.", reciclavel: true, riscoAmbiental: "Baixo" },
    { tipoResiduo: "Metal", descricao: "Metais ferrosos e não ferrosos", instrucoesDescarte: "Separar alumínio de ferro.", reciclavel: true, riscoAmbiental: "Baixo" },
    { tipoResiduo: "Organico", descricao: "Resíduos orgânicos", instrucoesDescarte: "Destinar à compostagem.", reciclavel: true, riscoAmbiental: "Médio — gera metano em aterros", compostagem: { tempoDecomposicaoDias: 90, producaoAduboKg: 0.3 } },
    { tipoResiduo: "Eletronico", descricao: "Equipamentos eletrônicos", instrucoesDescarte: "Levar a pontos especializados.", reciclavel: true, riscoAmbiental: "Alto — metais pesados", legislacao: { lei: "PNRS", artigo: "Logística reversa" } },
    { tipoResiduo: "PilhasBaterias", descricao: "Pilhas e baterias", instrucoesDescarte: "Armazenar em recipiente seguro.", reciclavel: true, riscoAmbiental: "Alto — contaminação" },
    { tipoResiduo: "OleoCozinha", descricao: "Óleo de cozinha usado", instrucoesDescarte: "Armazenar em garrafa PET.", reciclavel: true, riscoAmbiental: "Alto — contamina corpos d'água", destinoFinal: "Biodiesel" },
    { tipoResiduo: "Outros", descricao: "Outros resíduos", instrucoesDescarte: "Consultar orientação local.", reciclavel: false, riscoAmbiental: "Variável" },
    { tipoResiduo: "Plastico", descricao: "Plástico flexível (filme)", instrucoesDescarte: "Agrupar em saco limpo.", reciclavel: true, riscoAmbiental: "Médio", subtipo: "Filme", flexSchema: true },
    { tipoResiduo: "Papel", descricao: "Papelão ondulado", instrucoesDescarte: "Desmontar caixas e empilhar.", reciclavel: true, riscoAmbiental: "Baixo", subtipo: "Papelao" }
  ];

  const usuariosSistema = [
    { nome: "Administrador", email: "admin@ecocoleta.com", senhaHash: "$2a$11$exemploHashAdmin", role: "Admin", criadoEm: daysAgo(120), ativo: true },
    { nome: "Maria Silva", email: "maria.silva@ecocoleta.com", senhaHash: "$2a$11$exemploHash1", role: "Operador", criadoEm: daysAgo(60), preferencias: { notificacoesEmail: true, tema: "claro" } },
    { nome: "João Santos", email: "joao.santos@ecocoleta.com", senhaHash: "$2a$11$exemploHash2", role: "Operador", criadoEm: daysAgo(55), cidadeAtuacao: "São Paulo" },
    { nome: "Ana Costa", email: "ana.costa@ecocoleta.com", senhaHash: "$2a$11$exemploHash3", role: "Auditor", criadoEm: daysAgo(50), permissoes: ["relatorios", "auditoria"] },
    { nome: "Carlos Oliveira", email: "carlos@ecocoleta.com", senhaHash: "$2a$11$exemploHash4", role: "Operador", criadoEm: daysAgo(40) },
    { nome: "Fernanda Lima", email: "fernanda@ecocoleta.com", senhaHash: "$2a$11$exemploHash5", role: "Admin", criadoEm: daysAgo(35), preferencias: { notificacoesEmail: false } },
    { nome: "Pedro Alves", email: "pedro@ecocoleta.com", senhaHash: "$2a$11$exemploHash6", role: "Cidadao", criadoEm: daysAgo(30), tipoConta: "voluntario" },
    { nome: "Juliana Rocha", email: "juliana@ecocoleta.com", senhaHash: "$2a$11$exemploHash7", role: "Cidadao", criadoEm: daysAgo(25) },
    { nome: "Ricardo Mendes", email: "ricardo@ecocoleta.com", senhaHash: "$2a$11$exemploHash8", role: "Operador", criadoEm: daysAgo(20), cidadeAtuacao: "Campinas" },
    { nome: "Patricia Souza", email: "patricia@ecocoleta.com", senhaHash: "$2a$11$exemploHash9", role: "Auditor", criadoEm: daysAgo(15), permissoes: ["relatorios", "compliance", "governanca"] },
    { nome: "Usuario Inativo", email: "inativo@ecocoleta.com", senhaHash: "$2a$11$exemploHash10", role: "Cidadao", criadoEm: daysAgo(10), ativo: false, motivo: "Conta desativada por inatividade" }
  ];

  const registrosResiduos = [
    { pontoColetaNome: "EcoPonto Centro", tipoResiduo: "Plastico", pesoKg: 12.5, origem: "Residência", registradoEm: daysAgo(7) },
    { pontoColetaNome: "EcoPonto Centro", tipoResiduo: "Plastico", pesoKg: 8.0, origem: "Comércio local", registradoEm: daysAgo(6), observacao: "Garrafas PET" },
    { pontoColetaNome: "EcoPonto Jardins", tipoResiduo: "Papel", pesoKg: 15.0, origem: "Escritório", registradoEm: daysAgo(5) },
    { pontoColetaNome: "EcoPonto Vila Mariana", tipoResiduo: "Vidro", pesoKg: 22.0, origem: "Restaurante", registradoEm: daysAgo(5), fotos: ["vidro_001.jpg"] },
    { pontoColetaNome: "EcoPonto Pinheiros", tipoResiduo: "Metal", pesoKg: 35.0, origem: "Indústria", registradoEm: daysAgo(4), classificacaoNbr: { codigo: "A004", descricao: "Sucata metálica" } },
    { pontoColetaNome: "EcoPonto Mooca", tipoResiduo: "Organico", pesoKg: 18.0, origem: "Feira livre", registradoEm: daysAgo(4) },
    { pontoColetaNome: "EcoPonto Santana", tipoResiduo: "Eletronico", pesoKg: 5.5, origem: "Domicílio", registradoEm: daysAgo(3), itens: ["celular", "carregador"] },
    { pontoColetaNome: "EcoPonto Tatuapé", tipoResiduo: "PilhasBaterias", pesoKg: 2.0, origem: "Escola", registradoEm: daysAgo(3) },
    { pontoColetaNome: "EcoPonto Campinas Centro", tipoResiduo: "OleoCozinha", pesoKg: 10.0, origem: "Restaurante", registradoEm: daysAgo(2), volumeLitros: 12 },
    { pontoColetaNome: "EcoPonto Guarulhos", tipoResiduo: "Plastico", pesoKg: 45.0, origem: "Cooperativa", registradoEm: daysAgo(2), doador: { nome: "Coop Verde", cnpj: "12.345.678/0001-90" } },
    { pontoColetaNome: "EcoPonto Osasco", tipoResiduo: "Papel", pesoKg: 20.0, origem: "Condomínio", registradoEm: daysAgo(1), observacao: "Coleta mensal" },
    { pontoColetaNome: "EcoPonto Centro", tipoResiduo: "Plastico", pesoKg: 6.0, origem: "Evento sustentável", registradoEm: daysAgo(1), evento: { nome: "Feira ESG 2026", participantes: 500 } }
  ];

  const alertasColeta = [
    { pontoColetaNome: "EcoPonto Vila Mariana", nivel: "Atencao", mensagem: "Ocupação atingiu 80% da capacidade", resolvido: false, criadoEm: daysAgo(3) },
    { pontoColetaNome: "EcoPonto Guarulhos", nivel: "Critico", mensagem: "Ocupação atingiu 91% — coleta urgente necessária", resolvido: false, criadoEm: daysAgo(2), acoesSugeridas: ["Acionar equipe", "Notificar prefeitura"] },
    { pontoColetaNome: "EcoPonto Mooca", nivel: "Atencao", mensagem: "Ocupação em 80%", resolvido: true, criadoEm: daysAgo(10), resolvidoEm: daysAgo(8) },
    { pontoColetaNome: "EcoPonto Centro", nivel: "Atencao", mensagem: "Capacidade próxima do limite", resolvido: false, criadoEm: daysAgo(1) },
    { pontoColetaNome: "EcoPonto Jardins", nivel: "Atencao", mensagem: "Ocupação em 28%", resolvido: true, criadoEm: daysAgo(20), resolvidoEm: daysAgo(19) },
    { pontoColetaNome: "EcoPonto Pinheiros", nivel: "Atencao", mensagem: "Previsão de lotação em 3 dias", resolvido: false, criadoEm: daysAgo(1), previsao: { diasParaLotacao: 3, confianca: 0.85 } },
    { pontoColetaNome: "EcoPonto Santana", nivel: "Atencao", mensagem: "Baixa rotatividade detectada", resolvido: false, criadoEm: daysAgo(5) },
    { pontoColetaNome: "EcoPonto Tatuapé", nivel: "Critico", mensagem: "Recipiente de pilhas quase cheio", resolvido: false, criadoEm: daysAgo(1), prioridade: "alta" },
    { pontoColetaNome: "EcoPonto Campinas Centro", nivel: "Atencao", mensagem: "Óleo acumulado — risco de vazamento", resolvido: true, criadoEm: daysAgo(7), resolvidoEm: daysAgo(6) },
    { pontoColetaNome: "EcoPonto Osasco", nivel: "Atencao", mensagem: "Ocupação em 34%", resolvido: false, criadoEm: daysAgo(2) },
    { pontoColetaNome: "EcoPonto Guarulhos", nivel: "Critico", mensagem: "Ponto sobrecarregado — bloquear novos descartes", resolvido: false, criadoEm: daysAgo(0), bloqueioDescarte: true }
  ];

  return { pontosColeta, destinacoesResiduos, usuariosSistema, registrosResiduos, alertasColeta };
}

module.exports = { getSeedData };
