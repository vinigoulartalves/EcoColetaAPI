# MongoDB — EcoColeta API

Scripts e evidências da migração SQL Server → MongoDB para a atividade avaliativa NoSQL (FIAP).

## Collections (5)

| Collection | Documentos | Pilar ESG |
|---|---|---|
| `pontos_coleta` | 11+ | Ambiental |
| `registros_residuos` | 12+ | Ambiental |
| `alertas_coleta` | 11+ | Ambiental + Governança |
| `destinacoes_residuos` | 11+ | Ambiental + Social |
| `usuarios_sistema` | 10+ | Governança |

## Execução rápida

```bash
# 1. Subir MongoDB
docker compose up mongodb -d

# 2. Executar script mongosh
mongosh EcoColetaDb < ecocoleta-mongodb-crud.js

# 3. Ou executar via Node.js (gera evidências)
npm install
node scripts/run-evidencias.js

# Com MongoDB local
MONGODB_URI=mongodb://localhost:27017 node scripts/run-evidencias.js
```

## Arquivos

- `ecocoleta-mongodb-crud.js` — Script completo para mongosh
- `scripts/seed-data.js` — Dados de seed
- `scripts/run-evidencias.js` — Seed + CRUD + geração de evidências
- `evidencias/crud-evidencias.txt` — Saída das operações

## Documentação completa

Ver `docs/ATIVIDADE_NOSQL.md` e `docs/ATIVIDADE_NOSQL.pdf`.
