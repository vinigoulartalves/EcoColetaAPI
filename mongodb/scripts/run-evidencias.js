/**
 * Executa seed + CRUD no MongoDB e gera arquivos de evidência.
 * Uso: node scripts/run-evidencias.js
 * Com MongoDB local: MONGODB_URI=mongodb://localhost:27017 node scripts/run-evidencias.js
 */

const fs = require("fs");
const path = require("path");
const { MongoClient, ObjectId } = require("mongodb");
const { MongoMemoryServer } = require("mongodb-memory-server");
const { getSeedData } = require("./seed-data");

const DB_NAME = "EcoColetaDb";
const EVIDENCIAS_DIR = path.join(__dirname, "..", "evidencias");

function log(lines, text) {
  lines.push(text);
  console.log(text);
}

async function createIndexes(db) {
  await db.collection("pontos_coleta").createIndex({ cidade: 1, bairro: 1 });
  await db.collection("pontos_coleta").createIndex({ localizacao: "2dsphere" });
  await db.collection("pontos_coleta").createIndex({ tipoResiduoAceito: 1 });
  await db.collection("destinacoes_residuos").createIndex({ tipoResiduo: 1 });
  await db.collection("usuarios_sistema").createIndex({ email: 1 }, { unique: true });
  await db.collection("registros_residuos").createIndex({ pontoColetaId: 1, registradoEm: -1 });
  await db.collection("alertas_coleta").createIndex({ resolvido: 1, nivel: 1 });
}

async function seedDatabase(db, lines) {
  const data = getSeedData();

  log(lines, "\n========== CREATE / INSERT (SEED) ==========");
  log(lines, `use ${DB_NAME}`);

  await db.collection("pontos_coleta").deleteMany({});
  await db.collection("destinacoes_residuos").deleteMany({});
  await db.collection("usuarios_sistema").deleteMany({});
  await db.collection("registros_residuos").deleteMany({});
  await db.collection("alertas_coleta").deleteMany({});

  const pontosResult = await db.collection("pontos_coleta").insertMany(data.pontosColeta);
  log(lines, `\n// Collection: pontos_coleta (${data.pontosColeta.length} documentos)`);
  log(lines, `db.pontos_coleta.insertMany([...]) // ${pontosResult.insertedCount} inseridos`);

  const destResult = await db.collection("destinacoes_residuos").insertMany(data.destinacoesResiduos);
  log(lines, `\n// Collection: destinacoes_residuos (${data.destinacoesResiduos.length} documentos)`);
  log(lines, `db.destinacoes_residuos.insertMany([...]) // ${destResult.insertedCount} inseridos`);

  const userResult = await db.collection("usuarios_sistema").insertMany(data.usuariosSistema);
  log(lines, `\n// Collection: usuarios_sistema (${data.usuariosSistema.length} documentos)`);
  log(lines, `db.usuarios_sistema.insertMany([...]) // ${userResult.insertedCount} inseridos`);

  const pontoMap = {};
  const pontoDocs = await db.collection("pontos_coleta").find().toArray();
  pontoDocs.forEach((p) => { pontoMap[p.nome] = p._id; });

  const registros = data.registrosResiduos.map((r) => ({
    pontoColetaId: pontoMap[r.pontoColetaNome],
    pontoColetaNome: r.pontoColetaNome,
    tipoResiduo: r.tipoResiduo,
    pesoKg: r.pesoKg,
    origem: r.origem,
    observacao: r.observacao,
    registradoEm: r.registradoEm,
    fotos: r.fotos,
    classificacaoNbr: r.classificacaoNbr,
    itens: r.itens,
    volumeLitros: r.volumeLitros,
    doador: r.doador,
    evento: r.evento
  }));

  const regResult = await db.collection("registros_residuos").insertMany(registros);
  log(lines, `\n// Collection: registros_residuos (${registros.length} documentos)`);
  log(lines, `db.registros_residuos.insertMany([...]) // ${regResult.insertedCount} inseridos`);

  const alertas = data.alertasColeta.map((a) => ({
    pontoColetaId: pontoMap[a.pontoColetaNome],
    pontoColetaNome: a.pontoColetaNome,
    nivel: a.nivel,
    mensagem: a.mensagem,
    resolvido: a.resolvido,
    criadoEm: a.criadoEm,
    resolvidoEm: a.resolvidoEm,
    acoesSugeridas: a.acoesSugeridas,
    previsao: a.previsao,
    prioridade: a.prioridade,
    bloqueioDescarte: a.bloqueioDescarte
  }));

  const alertResult = await db.collection("alertas_coleta").insertMany(alertas);
  log(lines, `\n// Collection: alertas_coleta (${alertas.length} documentos)`);
  log(lines, `db.alertas_coleta.insertMany([...]) // ${alertResult.insertedCount} inseridos`);

  await createIndexes(db);
  log(lines, "\n// Índices criados em todas as collections");

  return { pontoMap };
}

async function runCrudDemo(db, lines) {
  log(lines, "\n========== READ (FIND) ==========");

  const countPontos = await db.collection("pontos_coleta").countDocuments();
  log(lines, `\ndb.pontos_coleta.countDocuments() => ${countPontos}`);

  log(lines, "\ndb.pontos_coleta.find({ cidade: 'São Paulo', ativo: true }).limit(3)");
  const pontosSP = await db.collection("pontos_coleta")
    .find({ cidade: "São Paulo", ativo: true })
    .limit(3)
    .project({ nome: 1, bairro: 1, ocupacaoAtualKg: 1, capacidadeMaximaKg: 1 })
    .toArray();
  log(lines, JSON.stringify(pontosSP, null, 2));

  log(lines, "\ndb.destinacoes_residuos.find({ reciclavel: true, riscoAmbiental: /Alto/ })");
  const destAltoRisco = await db.collection("destinacoes_residuos")
    .find({ reciclavel: true, riscoAmbiental: /Alto/ })
    .toArray();
  log(lines, JSON.stringify(destAltoRisco.map(d => ({ tipoResiduo: d.tipoResiduo, riscoAmbiental: d.riscoAmbiental })), null, 2));

  log(lines, "\ndb.alertas_coleta.find({ resolvido: false, nivel: 'Critico' })");
  const alertasCriticos = await db.collection("alertas_coleta")
    .find({ resolvido: false, nivel: "Critico" })
    .toArray();
  log(lines, JSON.stringify(alertasCriticos.map(a => ({ pontoColetaNome: a.pontoColetaNome, mensagem: a.mensagem })), null, 2));

  log(lines, "\ndb.registros_residuos.aggregate([{ $group: { _id: '$tipoResiduo', totalKg: { $sum: '$pesoKg' } } }])");
  const agregacao = await db.collection("registros_residuos").aggregate([
    { $group: { _id: "$tipoResiduo", totalKg: { $sum: "$pesoKg" }, quantidade: { $sum: 1 } } },
    { $sort: { totalKg: -1 } }
  ]).toArray();
  log(lines, JSON.stringify(agregacao, null, 2));

  log(lines, "\n========== UPDATE ==========");

  log(lines, "\ndb.pontos_coleta.updateOne({ nome: 'EcoPonto Centro' }, { $set: { ocupacaoAtualKg: 135, ultimaAtualizacao: new Date() } })");
  const updatePonto = await db.collection("pontos_coleta").updateOne(
    { nome: "EcoPonto Centro" },
    { $set: { ocupacaoAtualKg: 135, ultimaAtualizacao: new Date() } }
  );
  log(lines, `=> matched: ${updatePonto.matchedCount}, modified: ${updatePonto.modifiedCount}`);

  log(lines, "\ndb.alertas_coleta.updateOne({ pontoColetaNome: 'EcoPonto Santana', resolvido: false }, { $set: { resolvido: true, resolvidoEm: new Date() } })");
  const updateAlerta = await db.collection("alertas_coleta").updateOne(
    { pontoColetaNome: "EcoPonto Santana", resolvido: false },
    { $set: { resolvido: true, resolvidoEm: new Date() } }
  );
  log(lines, `=> matched: ${updateAlerta.matchedCount}, modified: ${updateAlerta.modifiedCount}`);

  log(lines, "\ndb.usuarios_sistema.updateOne({ email: 'maria.silva@ecocoleta.com' }, { $set: { 'preferencias.tema': 'escuro' } })");
  const updateUser = await db.collection("usuarios_sistema").updateOne(
    { email: "maria.silva@ecocoleta.com" },
    { $set: { "preferencias.tema": "escuro" } }
  );
  log(lines, `=> matched: ${updateUser.matchedCount}, modified: ${updateUser.modifiedCount}`);

  log(lines, "\ndb.destinacoes_residuos.updateMany({ tipoResiduo: 'Plastico' }, { $inc: { 'impactoESG.reducaoCO2KgPorTonelada': 50 } })");
  const updateDest = await db.collection("destinacoes_residuos").updateMany(
    { tipoResiduo: "Plastico" },
    { $inc: { "impactoESG.reducaoCO2KgPorTonelada": 50 } }
  );
  log(lines, `=> matched: ${updateDest.matchedCount}, modified: ${updateDest.modifiedCount}`);

  log(lines, "\n========== INSERT (CREATE adicional) ==========");

  log(lines, "\ndb.pontos_coleta.insertOne({ nome: 'EcoPonto Novo CRUD', ... })");
  const insertPonto = await db.collection("pontos_coleta").insertOne({
    nome: "EcoPonto Novo CRUD",
    endereco: "Rua Nova, 50",
    bairro: "Itaim",
    cidade: "São Paulo",
    localizacao: { type: "Point", coordinates: [-46.6800, -23.5800] },
    tipoResiduoAceito: "Plastico",
    capacidadeMaximaKg: 200,
    ocupacaoAtualKg: 0,
    ativo: true,
    criadoEm: new Date()
  });
  log(lines, `=> insertedId: ${insertPonto.insertedId}`);

  log(lines, "\n========== DELETE ==========");

  log(lines, "\ndb.pontos_coleta.deleteOne({ nome: 'EcoPonto Inativo Teste' })");
  const deletePonto = await db.collection("pontos_coleta").deleteOne({ nome: "EcoPonto Inativo Teste" });
  log(lines, `=> deletedCount: ${deletePonto.deletedCount}`);

  log(lines, "\ndb.usuarios_sistema.deleteOne({ email: 'inativo@ecocoleta.com' })");
  const deleteUser = await db.collection("usuarios_sistema").deleteOne({ email: "inativo@ecocoleta.com" });
  log(lines, `=> deletedCount: ${deleteUser.deletedCount}`);

  log(lines, "\n========== RESUMO FINAL DAS COLLECTIONS ==========");
  const collections = ["pontos_coleta", "destinacoes_residuos", "usuarios_sistema", "registros_residuos", "alertas_coleta"];
  for (const col of collections) {
    const count = await db.collection(col).countDocuments();
    log(lines, `  ${col}: ${count} documentos`);
  }

  log(lines, "\n========== EXEMPLO DE ESQUEMA FLEXÍVEL ==========");
  log(lines, "Documentos em pontos_coleta com campos diferentes:");
  const flexSamples = await db.collection("pontos_coleta").find({
    $or: [
      { horarioFuncionamento: { $exists: true } },
      { contato: { $exists: true } },
      { certificacoes: { $exists: true } },
      { parceria: { $exists: true } }
    ]
  }).project({ nome: 1, horarioFuncionamento: 1, contato: 1, certificacoes: 1, parceria: 1 }).limit(4).toArray();
  log(lines, JSON.stringify(flexSamples, null, 2));
}

async function main() {
  fs.mkdirSync(EVIDENCIAS_DIR, { recursive: true });
  const lines = [];
  const timestamp = new Date().toISOString();

  log(lines, `EcoColeta API - Evidências MongoDB`);
  log(lines, `Executado em: ${timestamp}`);
  log(lines, `Banco de dados: ${DB_NAME}`);

  let client;
  let memoryServer;

  const uri = process.env.MONGODB_URI;

  if (uri) {
    log(lines, `Conexão: ${uri}`);
    client = new MongoClient(uri);
    await client.connect();
  } else {
    log(lines, "Conexão: MongoDB em memória (mongodb-memory-server)");
    memoryServer = await MongoMemoryServer.create();
    client = new MongoClient(memoryServer.getUri());
    await client.connect();
  }

  const db = client.db(DB_NAME);

  try {
    await seedDatabase(db, lines);
    await runCrudDemo(db, lines);
  } finally {
    await client.close();
    if (memoryServer) await memoryServer.stop();
  }

  const outputFile = path.join(EVIDENCIAS_DIR, "crud-evidencias.txt");
  fs.writeFileSync(outputFile, lines.join("\n"), "utf8");
  console.log(`\nEvidências salvas em: ${outputFile}`);
}

main().catch((err) => {
  console.error(err);
  process.exit(1);
});
