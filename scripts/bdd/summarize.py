#!/usr/bin/env python3
"""Fail the run for missing endpoints, undefined/skipped steps or incomplete evidence."""
import json, re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / 'artifacts/bdd'
EXPECTED = {
    ('POST','/api/auth/login'),
    ('GET','/api/pontos-coleta'), ('GET','/api/pontos-coleta/{id}'),
    ('POST','/api/pontos-coleta'), ('PUT','/api/pontos-coleta/{id}'),
    ('GET','/api/registros-residuos'), ('POST','/api/registros-residuos'),
    ('GET','/api/destinacoes-residuos'), ('GET','/api/destinacoes-residuos/{tipoResiduo}'),
    ('POST','/api/destinacoes-residuos'), ('GET','/api/alertas-coleta'),
    ('PATCH','/api/alertas-coleta/{id}/resolver'), ('POST','/api/alertas-coleta/recalcular'),
    ('GET','/api/relatorios-impacto/resumo'), ('GET','/health/live'), ('GET','/health/ready')
}
def normalize(path):
    path=path.split('?')[0]
    path=re.sub(r'/pontos-coleta/\d+', '/pontos-coleta/{id}', path)
    path=re.sub(r'/destinacoes-residuos/\d+', '/destinacoes-residuos/{tipoResiduo}', path)
    return re.sub(r'/alertas-coleta/\d+/resolver', '/alertas-coleta/{id}/resolver', path)

def main():
    report=json.loads((OUT/'cucumber/cucumber.json').read_text())
    scenarios=[e for f in report for e in f.get('elements',[]) if e.get('type')=='scenario']
    states=[s.get('result',{}).get('status','missing') for e in scenarios for s in e.get('steps',[]) + e.get('before',[]) + e.get('after',[])]
    if not scenarios or any(s!='passed' for s in states): raise SystemExit('Existem cenários ou hooks falhos, pendentes ou não executados.')
    evidence=[json.loads(line) for line in (OUT/'cucumber/http-evidence.jsonl').read_text().splitlines() if line.strip()]
    if len(evidence)!=len(scenarios) or any(e['status']!='PASSED' for e in evidence):
        raise SystemExit('Evidência HTTP incompleta ou execução com falhas.')
    calls={}
    for e in evidence:
        for x in e['exchanges']:
            key=(x['method'],normalize(x['path']))
            calls.setdefault(key,set()).add(x['statusCode'])
    missing=EXPECTED-set(calls)
    missing_success={k for k in EXPECTED if not any(200<=s<300 for s in calls.get(k,[]))}
    if missing or missing_success: raise SystemExit(f'Operações sem cobertura de sucesso: {sorted(missing|missing_success)}')
    # Catch new application endpoints omitted from the maintained test inventory.
    spec=json.loads((OUT/'openapi.json').read_text())
    actual={(method.upper(), normalize(path)) for path, methods in spec['paths'].items() for method in methods if method in {'get','post','put','patch','delete'}}
    if actual-EXPECTED: raise SystemExit(f'Novas operações sem planejamento de cobertura: {sorted(actual-EXPECTED)}')
    rows=[{'method':m,'path':p,'statusCodes':sorted(calls[(m,p)])} for m,p in sorted(EXPECTED)]
    summary={'scenarios':len(scenarios),'passed':len(scenarios),'operations':len(rows),'coverage':rows,'database':'SQL Server 2022','transport':'HTTP','parallel':False}
    (OUT/'summary.json').write_text(json.dumps(summary,ensure_ascii=False,indent=2)+'\n')
    text=['# Evidências BDD EcoColeta','',f"- Cenários aprovados: **{len(scenarios)}**",'- Operações cobertas com sucesso: **16/16**','- Banco: **SQL Server 2022 real**','', '| Método | Rota | Status observados |','|---|---|---|']
    text += [f"| {r['method']} | `{r['path']}` | {', '.join(map(str,r['statusCodes']))} |" for r in rows]
    (OUT/'summary.md').write_text('\n'.join(text)+'\n')
    print('\n'.join(text))
if __name__=='__main__':main()
