#!/usr/bin/env bash
# One isolated SQL Server + actual API + Java BDD suite per invocation.
set -Eeuo pipefail
cd "$(dirname "$0")/../.."
command -v docker >/dev/null || { echo 'Instale Docker Engine/Docker Desktop com Compose v2.' >&2; exit 1; }
command -v python3 >/dev/null || { echo 'Instale Python 3 para gerar credenciais e o resumo.' >&2; exit 1; }
docker compose version >/dev/null
mkdir -p artifacts/bdd
python3 - <<'PY'
import os, secrets
from pathlib import Path
p = Path('.env.bdd')
p.write_text('BDD_SQL_PASSWORD=Sql!'+secrets.token_hex(16)+'\nBDD_ADMIN_PASSWORD=Admin!'+secrets.token_hex(16)+'\nBDD_JWT_SECRET='+secrets.token_hex(32)+'\n')
os.chmod(p, 0o600)
PY
compose=(docker compose --project-name ecocoleta-bdd --env-file .env.bdd -f docker-compose.bdd.yml)
cleanup() {
  local exit_status=$?
  trap - EXIT
  "${compose[@]}" logs --no-color api sqlserver > artifacts/bdd/services.log 2>&1 || true
  "${compose[@]}" down --volumes --remove-orphans > /dev/null 2>&1 || true
  rm -f .env.bdd
  exit "$exit_status"
}
trap cleanup EXIT
# Only the dedicated ecocoleta-bdd stack is reset; no other project's volume is touched.
"${compose[@]}" down --volumes --remove-orphans
"${compose[@]}" up --build -d --wait --wait-timeout 300 sqlserver api
"${compose[@]}" ps > artifacts/bdd/compose-ps.txt
# Reserve one enum value for the successful creation scenario; keep all other seeded destinations.
"${compose[@]}" exec -T sqlserver sh -c 'SQLCMDPASSWORD="$MSSQL_SA_PASSWORD" /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -C -d EcoColetaBdd -b -Q "DELETE FROM DestinacoesResiduos WHERE TipoResiduo = 9;"'
"${compose[@]}" exec -T api curl --fail --silent http://localhost:8080/swagger/v1/swagger.json > artifacts/bdd/openapi.json
set +e
"${compose[@]}" run --build --rm bdd-tests 2>&1 | tee artifacts/bdd-execution.log
test_exit=${PIPESTATUS[0]}
set -e
if [[ "$test_exit" -ne 0 ]]; then exit "$test_exit"; fi
python3 scripts/bdd/summarize.py
