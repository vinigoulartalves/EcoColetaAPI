#!/usr/bin/env python3
"""Gera credenciais descartaveis no runner. Nao usar para redefinir um banco existente."""
import os
import secrets
from pathlib import Path

required = ['COMPOSE_PROJECT_NAME', 'API_IMAGE', 'APP_ENVIRONMENT', 'API_PORT']
values = {key: os.environ[key] for key in required}
values.update({
    'MSSQL_SA_PASSWORD': 'Sql_' + secrets.token_hex(20) + '!9aA',
    'JWT_SECRET': secrets.token_hex(32),
    'ADMIN_EMAIL': 'admin@ecocoleta.com',
    'ADMIN_PASSWORD': 'Admin_' + secrets.token_hex(20) + '!9aA',
    'SWAGGER_ENABLED': 'true' if values['APP_ENVIRONMENT'] == 'Staging' else 'false',
})
for key in ['MSSQL_SA_PASSWORD', 'JWT_SECRET', 'ADMIN_PASSWORD']:
    print('::add-mask::' + values[key])
path = Path('.env')
path.write_text('\n'.join(f'{k}={v}' for k, v in values.items()) + '\n')
path.chmod(0o600)
# Somente para uso pelos proximos passos do mesmo job.
with open(os.environ['GITHUB_ENV'], 'a') as output:
    output.write('ADMIN_EMAIL=' + values['ADMIN_EMAIL'] + '\n')
    output.write('ADMIN_PASSWORD=' + values['ADMIN_PASSWORD'] + '\n')
