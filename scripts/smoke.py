#!/usr/bin/env python3
"""Smoke tests reais via HTTP. Nunca persiste senha ou JWT nas evidencias."""
import argparse
import json
import os
from datetime import datetime, timezone
from pathlib import Path
from urllib.error import HTTPError
from urllib.request import Request, urlopen

parser = argparse.ArgumentParser()
parser.add_argument('--url', required=True)
parser.add_argument('--environment', required=True)
parser.add_argument('--version', required=True)
parser.add_argument('--output', required=True)
parser.add_argument('--verify-persistence', action='store_true')
a = parser.parse_args()
results = []

def request(method, path, expected=200, payload=None, token=None):
    headers = {'Content-Type': 'application/json'}
    if token:
        headers['Authorization'] = 'Bearer ' + token
    req = Request(a.url + path, data=json.dumps(payload).encode() if payload is not None else None,
                  method=method, headers=headers)
    try:
        with urlopen(req, timeout=30) as response:
            status, body = response.status, response.read()
    except HTTPError as error:
        status, body = error.code, error.read()
    if status != expected:
        raise RuntimeError(f'{method} {path}: esperado HTTP {expected}, recebido {status}')
    results.append({'method': method, 'path': path, 'status': status, 'result': 'PASS'})
    print(f'PASS {method} {path} HTTP {status}')
    return json.loads(body) if body and status != 404 else None

out = Path(a.output)
out.mkdir(parents=True, exist_ok=True)
health = request('GET', '/health/ready')
assert health['status'] == 'Healthy' and health['database'] == 'Connected'
assert health['environment'] == a.environment, health
assert health['version'] == a.version, health
if a.verify_persistence:
    previous = json.loads((out / 'smoke.json').read_text())
    point = request('GET', f"/api/pontos-coleta/{previous['point_id']}")
    assert point['ocupacaoAtualKg'] == 81, point
    report = {'timestamp_utc': datetime.now(timezone.utc).isoformat(), 'health': health,
              'point_id': point['id'], 'occupancy_kg': point['ocupacaoAtualKg'],
              'result': 'PASS', 'checks': results}
    (out / 'persistence.json').write_text(json.dumps(report, indent=2) + '\n')
else:
    request('GET', '/health/live')
    request('POST', '/api/pontos-coleta', expected=401, payload={})
    login = request('POST', '/api/auth/login', payload={
        'email': os.environ.get('ADMIN_EMAIL', 'admin@ecocoleta.com'),
        'senha': os.environ['ADMIN_PASSWORD']})
    token = login['token']
    assert token
    point = request('POST', '/api/pontos-coleta', expected=201, token=token, payload={
        'nome': 'Ponto smoke DevOps', 'endereco': 'Rua de Testes, 100', 'bairro': 'Centro',
        'cidade': 'Palhoca', 'latitude': -27.64, 'longitude': -48.67,
        'tipoResiduoAceito': 1, 'capacidadeMaximaKg': 100})
    request('POST', '/api/registros-residuos', expected=201, token=token, payload={
        'pontoColetaId': point['id'], 'tipoResiduo': 1, 'pesoKg': 81, 'origem': 'Smoke DevOps'})
    alerts = request('GET', f"/api/alertas-coleta?resolvido=false&pontoColetaId={point['id']}")
    assert alerts['totalItens'] >= 1
    request('GET', '/api/relatorios-impacto/resumo')
    request('GET', '/swagger/v1/swagger.json', expected=200 if a.environment == 'Staging' else 404)
    report = {'timestamp_utc': datetime.now(timezone.utc).isoformat(), 'health': health,
              'point_id': point['id'], 'result': 'PASS', 'checks': results}
    (out / 'smoke.json').write_text(json.dumps(report, indent=2) + '\n')
print('Smoke concluido: todas as verificacoes passaram.')
