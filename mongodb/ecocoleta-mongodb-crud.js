// EcoColeta API - Script MongoDB Shell (mongosh)
// Execução: mongosh < ecocoleta-mongodb-crud.js
// Ou: docker exec -i ecocoleta-mongodb mongosh EcoColetaDb < ecocoleta-mongodb-crud.js

const DB_NAME = "EcoColetaDb";
db = db.getSiblingDB(DB_NAME);

print("========== LIMPEZA INICIAL (apenas para demonstração) ==========");
db.pontos_coleta.drop();
db.destinacoes_residuos.drop();
db.usuarios_sistema.drop();
db.registros_residuos.drop();
db.alertas_coleta.drop();

print("\n========== CREATE: Índices ==========");
db.pontos_coleta.createIndex({ cidade: 1, bairro: 1 });
db.pontos_coleta.createIndex({ localizacao: "2dsphere" });
db.destinacoes_residuos.createIndex({ tipoResiduo: 1 });
db.usuarios_sistema.createIndex({ email: 1 }, { unique: true });
db.registros_residuos.createIndex({ pontoColetaId: 1, registradoEm: -1 });
db.alertas_coleta.createIndex({ resolvido: 1, nivel: 1 });

print("\n========== INSERT: pontos_coleta (11 documentos) ==========");
const pontosIds = db.pontos_coleta.insertMany([
  { nome: "EcoPonto Centro", endereco: "Rua das Flores, 100", bairro: "Centro", cidade: "São Paulo", localizacao: { type: "Point", coordinates: [-46.6333, -23.5505] }, tipoResiduoAceito: "Plastico", capacidadeMaximaKg: 500, ocupacaoAtualKg: 120, ativo: true, horarioFuncionamento: { abertura: "08:00", fechamento: "18:00" } },
  { nome: "EcoPonto Jardins", endereco: "Av. Paulista, 500", bairro: "Jardins", cidade: "São Paulo", localizacao: { type: "Point", coordinates: [-46.6559, -23.5614] }, tipoResiduoAceito: "Papel", capacidadeMaximaKg: 300, ocupacaoAtualKg: 85, ativo: true, contato: { telefone: "(11) 3000-1000", email: "jardins@ecocoleta.com.br" } },
  { nome: "EcoPonto Vila Mariana", endereco: "Rua Domingos de Morais, 200", bairro: "Vila Mariana", cidade: "São Paulo", localizacao: { type: "Point", coordinates: [-46.6342, -23.5890] }, tipoResiduoAceito: "Vidro", capacidadeMaximaKg: 400, ocupacaoAtualKg: 320, ativo: true },
  { nome: "EcoPonto Pinheiros", endereco: "Rua dos Pinheiros, 800", bairro: "Pinheiros", cidade: "São Paulo", localizacao: { type: "Point", coordinates: [-46.6912, -23.5678] }, tipoResiduoAceito: "Metal", capacidadeMaximaKg: 600, ocupacaoAtualKg: 150, ativo: true, certificacoes: ["ISO 14001"] },
  { nome: "EcoPonto Mooca", endereco: "Rua da Mooca, 1500", bairro: "Mooca", cidade: "São Paulo", localizacao: { type: "Point", coordinates: [-46.5945, -23.5540] }, tipoResiduoAceito: "Organico", capacidadeMaximaKg: 250, ocupacaoAtualKg: 200, ativo: true, parceria: { ong: "Composta Cidade" } },
  { nome: "EcoPonto Santana", endereco: "Av. Cruzeiro do Sul, 1200", bairro: "Santana", cidade: "São Paulo", localizacao: { type: "Point", coordinates: [-46.6267, -23.5050] }, tipoResiduoAceito: "Eletronico", capacidadeMaximaKg: 350, ocupacaoAtualKg: 45, ativo: true },
  { nome: "EcoPonto Tatuapé", endereco: "Rua Serra de Bragança, 500", bairro: "Tatuapé", cidade: "São Paulo", localizacao: { type: "Point", coordinates: [-46.5756, -23.5412] }, tipoResiduoAceito: "PilhasBaterias", capacidadeMaximaKg: 100, ocupacaoAtualKg: 30, ativo: true, seguranca: { recipienteBlindado: true } },
  { nome: "EcoPonto Campinas Centro", endereco: "Av. Francisco Glicério, 300", bairro: "Centro", cidade: "Campinas", localizacao: { type: "Point", coordinates: [-47.0608, -22.9035] }, tipoResiduoAceito: "OleoCozinha", capacidadeMaximaKg: 200, ocupacaoAtualKg: 80, ativo: true },
  { nome: "EcoPonto Guarulhos", endereco: "Av. Tiradentes, 700", bairro: "Centro", cidade: "Guarulhos", localizacao: { type: "Point", coordinates: [-46.5333, -23.4628] }, tipoResiduoAceito: "Plastico", capacidadeMaximaKg: 450, ocupacaoAtualKg: 410, ativo: true },
  { nome: "EcoPonto Inativo Teste", endereco: "Rua Teste, 1", bairro: "Teste", cidade: "São Paulo", localizacao: { type: "Point", coordinates: [-46.6000, -23.5500] }, tipoResiduoAceito: "Outros", capacidadeMaximaKg: 100, ocupacaoAtualKg: 0, ativo: false },
  { nome: "EcoPonto Osasco", endereco: "Av. dos Autonomistas, 2000", bairro: "Centro", cidade: "Osasco", localizacao: { type: "Point", coordinates: [-46.7917, -23.5329] }, tipoResiduoAceito: "Papel", capacidadeMaximaKg: 280, ocupacaoAtualKg: 95, ativo: true, acessibilidade: { rampa: true } }
]).insertedIds;

print("Inseridos: " + Object.keys(pontosIds).length + " pontos de coleta");

print("\n========== INSERT: destinacoes_residuos (11 documentos) ==========");
db.destinacoes_residuos.insertMany([
  { tipoResiduo: "Plastico", descricao: "Plásticos recicláveis", instrucoesDescarte: "Lavar e secar.", reciclavel: true, riscoAmbiental: "Baixo", impactoESG: { pilar: "Ambiental", reducaoCO2KgPorTonelada: 1500 } },
  { tipoResiduo: "Papel", descricao: "Papéis e papelões", instrucoesDescarte: "Manter seco.", reciclavel: true, riscoAmbiental: "Baixo" },
  { tipoResiduo: "Vidro", descricao: "Vidros em geral", instrucoesDescarte: "Separar por cor.", reciclavel: true, riscoAmbiental: "Baixo" },
  { tipoResiduo: "Metal", descricao: "Metais", instrucoesDescarte: "Separar alumínio.", reciclavel: true, riscoAmbiental: "Baixo" },
  { tipoResiduo: "Organico", descricao: "Orgânicos", instrucoesDescarte: "Compostagem.", reciclavel: true, riscoAmbiental: "Médio", compostagem: { tempoDecomposicaoDias: 90 } },
  { tipoResiduo: "Eletronico", descricao: "Eletrônicos", instrucoesDescarte: "Pontos especializados.", reciclavel: true, riscoAmbiental: "Alto", legislacao: { lei: "PNRS" } },
  { tipoResiduo: "PilhasBaterias", descricao: "Pilhas", instrucoesDescarte: "Recipiente seguro.", reciclavel: true, riscoAmbiental: "Alto" },
  { tipoResiduo: "OleoCozinha", descricao: "Óleo usado", instrucoesDescarte: "Garrafa PET.", reciclavel: true, riscoAmbiental: "Alto", destinoFinal: "Biodiesel" },
  { tipoResiduo: "Outros", descricao: "Outros", instrucoesDescarte: "Consultar local.", reciclavel: false, riscoAmbiental: "Variável" },
  { tipoResiduo: "Plastico", descricao: "Filme plástico", instrucoesDescarte: "Agrupar limpo.", reciclavel: true, riscoAmbiental: "Médio", subtipo: "Filme" },
  { tipoResiduo: "Papel", descricao: "Papelão", instrucoesDescarte: "Desmontar caixas.", reciclavel: true, riscoAmbiental: "Baixo", subtipo: "Papelao" }
]);

print("\n========== INSERT: usuarios_sistema (11 documentos) ==========");
db.usuarios_sistema.insertMany([
  { nome: "Administrador", email: "admin@ecocoleta.com", senhaHash: "$2a$11$hash", role: "Admin", ativo: true },
  { nome: "Maria Silva", email: "maria.silva@ecocoleta.com", senhaHash: "$2a$11$hash", role: "Operador", preferencias: { tema: "claro" } },
  { nome: "João Santos", email: "joao.santos@ecocoleta.com", senhaHash: "$2a$11$hash", role: "Operador", cidadeAtuacao: "São Paulo" },
  { nome: "Ana Costa", email: "ana.costa@ecocoleta.com", senhaHash: "$2a$11$hash", role: "Auditor", permissoes: ["relatorios", "auditoria"] },
  { nome: "Carlos Oliveira", email: "carlos@ecocoleta.com", senhaHash: "$2a$11$hash", role: "Operador" },
  { nome: "Fernanda Lima", email: "fernanda@ecocoleta.com", senhaHash: "$2a$11$hash", role: "Admin" },
  { nome: "Pedro Alves", email: "pedro@ecocoleta.com", senhaHash: "$2a$11$hash", role: "Cidadao", tipoConta: "voluntario" },
  { nome: "Juliana Rocha", email: "juliana@ecocoleta.com", senhaHash: "$2a$11$hash", role: "Cidadao" },
  { nome: "Ricardo Mendes", email: "ricardo@ecocoleta.com", senhaHash: "$2a$11$hash", role: "Operador", cidadeAtuacao: "Campinas" },
  { nome: "Patricia Souza", email: "patricia@ecocoleta.com", senhaHash: "$2a$11$hash", role: "Auditor", permissoes: ["governanca"] },
  { nome: "Usuario Inativo", email: "inativo@ecocoleta.com", senhaHash: "$2a$11$hash", role: "Cidadao", ativo: false }
]);

const centroId = db.pontos_coleta.findOne({ nome: "EcoPonto Centro" })._id;
const jardinsId = db.pontos_coleta.findOne({ nome: "EcoPonto Jardins" })._id;
const vilaId = db.pontos_coleta.findOne({ nome: "EcoPonto Vila Mariana" })._id;

print("\n========== INSERT: registros_residuos (12 documentos) ==========");
db.registros_residuos.insertMany([
  { pontoColetaId: centroId, pontoColetaNome: "EcoPonto Centro", tipoResiduo: "Plastico", pesoKg: 12.5, origem: "Residência", registradoEm: new Date() },
  { pontoColetaId: centroId, pontoColetaNome: "EcoPonto Centro", tipoResiduo: "Plastico", pesoKg: 8.0, origem: "Comércio", observacao: "PET" },
  { pontoColetaId: jardinsId, pontoColetaNome: "EcoPonto Jardins", tipoResiduo: "Papel", pesoKg: 15.0, origem: "Escritório" },
  { pontoColetaId: vilaId, pontoColetaNome: "EcoPonto Vila Mariana", tipoResiduo: "Vidro", pesoKg: 22.0, origem: "Restaurante", fotos: ["vidro_001.jpg"] },
  { pontoColetaId: centroId, pontoColetaNome: "EcoPonto Centro", tipoResiduo: "Metal", pesoKg: 35.0, origem: "Indústria", classificacaoNbr: { codigo: "A004" } },
  { pontoColetaId: jardinsId, pontoColetaNome: "EcoPonto Jardins", tipoResiduo: "Organico", pesoKg: 18.0, origem: "Feira" },
  { pontoColetaId: vilaId, pontoColetaNome: "EcoPonto Vila Mariana", tipoResiduo: "Eletronico", pesoKg: 5.5, origem: "Domicílio", itens: ["celular"] },
  { pontoColetaId: centroId, pontoColetaNome: "EcoPonto Centro", tipoResiduo: "PilhasBaterias", pesoKg: 2.0, origem: "Escola" },
  { pontoColetaId: jardinsId, pontoColetaNome: "EcoPonto Jardins", tipoResiduo: "OleoCozinha", pesoKg: 10.0, origem: "Restaurante", volumeLitros: 12 },
  { pontoColetaId: vilaId, pontoColetaNome: "EcoPonto Vila Mariana", tipoResiduo: "Plastico", pesoKg: 45.0, origem: "Cooperativa", doador: { nome: "Coop Verde" } },
  { pontoColetaId: centroId, pontoColetaNome: "EcoPonto Centro", tipoResiduo: "Papel", pesoKg: 20.0, origem: "Condomínio" },
  { pontoColetaId: jardinsId, pontoColetaNome: "EcoPonto Jardins", tipoResiduo: "Plastico", pesoKg: 6.0, origem: "Evento", evento: { nome: "Feira ESG 2026" } }
]);

print("\n========== INSERT: alertas_coleta (11 documentos) ==========");
db.alertas_coleta.insertMany([
  { pontoColetaId: vilaId, pontoColetaNome: "EcoPonto Vila Mariana", nivel: "Atencao", mensagem: "Ocupação 80%", resolvido: false, criadoEm: new Date() },
  { pontoColetaId: centroId, pontoColetaNome: "EcoPonto Centro", nivel: "Critico", mensagem: "Coleta urgente", resolvido: false, criadoEm: new Date(), bloqueioDescarte: true },
  { pontoColetaId: jardinsId, pontoColetaNome: "EcoPonto Jardins", nivel: "Atencao", mensagem: "Capacidade próxima", resolvido: true, resolvidoEm: new Date() },
  { pontoColetaId: vilaId, pontoColetaNome: "EcoPonto Vila Mariana", nivel: "Atencao", mensagem: "Monitorar", resolvido: false },
  { pontoColetaId: centroId, pontoColetaNome: "EcoPonto Centro", nivel: "Atencao", mensagem: "Previsão lotação", resolvido: false, previsao: { dias: 3 } },
  { pontoColetaId: jardinsId, pontoColetaNome: "EcoPonto Jardins", nivel: "Critico", mensagem: "Pilhas cheias", resolvido: false, prioridade: "alta" },
  { pontoColetaId: vilaId, pontoColetaNome: "EcoPonto Vila Mariana", nivel: "Atencao", mensagem: "Baixa rotatividade", resolvido: false },
  { pontoColetaId: centroId, pontoColetaNome: "EcoPonto Centro", nivel: "Atencao", mensagem: "Óleo acumulado", resolvido: true },
  { pontoColetaId: jardinsId, pontoColetaNome: "EcoPonto Jardins", nivel: "Atencao", mensagem: "Verificar", resolvido: false },
  { pontoColetaId: vilaId, pontoColetaNome: "EcoPonto Vila Mariana", nivel: "Critico", mensagem: "Sobrecarga", resolvido: false },
  { pontoColetaId: centroId, pontoColetaNome: "EcoPonto Centro", nivel: "Atencao", mensagem: "Manutenção", resolvido: false }
]);

print("\n========== READ ==========");
print("db.pontos_coleta.countDocuments(): " + db.pontos_coleta.countDocuments());
printjson(db.pontos_coleta.find({ cidade: "São Paulo", ativo: true }).limit(2).toArray());
printjson(db.alertas_coleta.find({ resolvido: false, nivel: "Critico" }).toArray());

print("\n========== UPDATE ==========");
printjson(db.pontos_coleta.updateOne({ nome: "EcoPonto Centro" }, { $set: { ocupacaoAtualKg: 135 } }));
printjson(db.alertas_coleta.updateOne({ pontoColetaNome: "EcoPonto Jardins", resolvido: false }, { $set: { resolvido: true, resolvidoEm: new Date() } }));

print("\n========== DELETE ==========");
printjson(db.pontos_coleta.deleteOne({ nome: "EcoPonto Inativo Teste" }));
printjson(db.usuarios_sistema.deleteOne({ email: "inativo@ecocoleta.com" }));

print("\n========== RESUMO ==========");
["pontos_coleta", "destinacoes_residuos", "usuarios_sistema", "registros_residuos", "alertas_coleta"].forEach(function(c) {
  print(c + ": " + db[c].countDocuments() + " documentos");
});

print("\nScript concluído com sucesso!");
