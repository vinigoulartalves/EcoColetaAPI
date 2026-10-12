#!/usr/bin/env python3
"""Empacota somente arquivos rastreados, sem .git, .env, bin/obj ou caches."""
import argparse
from pathlib import Path
import subprocess
import zipfile

parser = argparse.ArgumentParser()
parser.add_argument('--output', default='../EcoColetaAPI_DevOps.zip')
args = parser.parse_args()
root = Path(__file__).resolve().parents[1]
files = subprocess.check_output(['git', '-C', str(root), 'ls-files', '-z']).decode().split('\0')
output = Path(args.output).resolve()
output.parent.mkdir(parents=True, exist_ok=True)
with zipfile.ZipFile(output, 'w', zipfile.ZIP_DEFLATED) as archive:
    for name in files:
        if name and (root / name).is_file():
            archive.write(root / name, 'EcoColetaAPI/' + name)
print(output)
