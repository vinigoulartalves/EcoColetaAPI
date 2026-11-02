#!/usr/bin/env python3
"""Package source and evidence without caches, temporary credentials or unrelated artifacts."""
import hashlib, json, subprocess, zipfile
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT.parent/'output'

def main():
    OUT.mkdir(exist_ok=True)
    files=subprocess.check_output(['git','ls-files','--cached','--others','--exclude-standard','-z'],cwd=ROOT).decode().split('\0')
    files=[f for f in files if f and not f.startswith('docs/evidencias/')]
    evidence=ROOT/'artifacts/bdd'
    if not (evidence/'summary.json').exists(): raise SystemExit('Execute e valide a suíte antes de empacotar.')
    for p in evidence.rglob('*'):
        if p.is_file() and not any(x in p.parts for x in ['test-classes','classes','maven-status','maven-archiver']):
            files.append(p.relative_to(ROOT).as_posix())
    for p in [ROOT/'artifacts/bdd-execution.log', ROOT/'docs/bdd/EcoColeta_BDD_Documentacao.pdf']:
        if p.is_file(): files.append(p.relative_to(ROOT).as_posix())
    files=sorted(set(files))
    forbidden=lambda f: (Path(f).name.startswith('.env') and not f.endswith('.example')) or any(x in Path(f).parts for x in ['.git','bin','obj','__pycache__'])
    if any(forbidden(f) for f in files):raise SystemExit('Arquivo temporário/credencial na seleção.')
    hashes={f:hashlib.sha256((ROOT/f).read_bytes()).hexdigest() for f in files}
    archive=OUT/'EcoColeta_BDD_Codigo_Fonte.zip'
    with zipfile.ZipFile(archive,'w',zipfile.ZIP_DEFLATED) as z:
        for f in files:
            info=zipfile.ZipInfo('EcoColetaAPI/'+f)
            info.compress_type=zipfile.ZIP_DEFLATED
            info.external_attr=(0o100755 if f.endswith('.sh') else 0o100644)<<16
            z.writestr(info,(ROOT/f).read_bytes())
        z.writestr('EcoColetaAPI/SHA256SUMS.json',json.dumps(hashes,indent=2)+'\n')
    with zipfile.ZipFile(archive) as z:
        assert z.testzip() is None
        for f,digest in hashes.items():assert hashlib.sha256(z.read('EcoColetaAPI/'+f)).hexdigest()==digest
    print(json.dumps({'archive':str(archive),'files':len(files),'bytes':archive.stat().st_size,'sha256':hashlib.sha256(archive.read_bytes()).hexdigest()}))
if __name__=='__main__':main()
