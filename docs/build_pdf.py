#!/usr/bin/env python3
"""Gera o PDF tecnico a partir das evidencias reais versionadas.
Requisitos locais: reportlab, Pillow. Uso: python3 docs/build_pdf.py caminho.pdf
"""
import json
import sys
from pathlib import Path
from xml.sax.saxutils import escape
import xml.etree.ElementTree as ET
from reportlab.pdfgen.canvas import Canvas
from reportlab.lib.colors import HexColor, white
from reportlab.lib.enums import TA_LEFT
from reportlab.lib.styles import ParagraphStyle
from reportlab.platypus import Paragraph, Table, TableStyle
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from PIL import Image

ROOT = Path(__file__).resolve().parents[1]
E = ROOT / 'docs/evidencias'
OUT = Path(sys.argv[1] if len(sys.argv)>1 else '../entrega/EcoColetaAPI_Documentacao_DevOps.pdf')
OUT.parent.mkdir(parents=True,exist_ok=True)
font_dir=Path('/usr/share/fonts/truetype/dejavu')
for name, file in [('Body','DejaVuSans.ttf'),('Bold','DejaVuSans-Bold.ttf'),('Mono','DejaVuSansMono.ttf')]:
    pdfmetrics.registerFont(TTFont(name,str(font_dir/file)))
W,H=595.276,841.89
INK=HexColor('#173C32'); GREEN=HexColor('#14805F'); MUTED=HexColor('#597468'); LINE=HexColor('#D1DFD6'); BG=HexColor('#F3F7F3'); NAVY=HexColor('#112F29')
c=Canvas(str(OUT),pagesize=(W,H))
c.setTitle('EcoColeta API - Documentação DevOps')
c.setAuthor('Vinícius Goulart Alves')
meta=json.loads((E/'run.json').read_text())
run=meta['run']; jobs=meta['jobs']; sha=run['head_sha']; run_url=run['html_url']
assert run['conclusion']=='success'
counts=json.loads((E/'testes/results.json').read_text())['counters']
cov=json.loads((E/'testes/coverage-summary.json').read_text())
page=0

def txt(text,x,y,size=10,color=INK,font='Body'):
    c.setFillColor(color);c.setFont(font,size);c.drawString(x,y,text)

def para(text,y,size=10.5,color=INK,width=499,x=48,leading=None):
    style=ParagraphStyle('p',fontName='Body',fontSize=size,leading=leading or size*1.48,textColor=color,spaceAfter=0)
    p=Paragraph(text,style);_,h=p.wrap(width,750);p.drawOn(c,x,y-h);return y-h

def head(kicker,title,sub=None):
    global page
    page+=1
    c.setFillColor(BG);c.rect(0,0,W,H,fill=1,stroke=0)
    c.setFillColor(GREEN);c.rect(0,H-9,W,9,fill=1,stroke=0)
    txt('EcoColeta.',48,H-45,16,font='Bold')
    txt('FIAP  /  DEVOPS  /  2026',370,H-44,8,MUTED)
    txt(kicker.upper(),48,H-93,9,GREEN,'Bold')
    y=para(title,H-107,26,width=499,leading=32)
    if sub:y=para(sub,y-12,10.5,MUTED)
    c.setStrokeColor(LINE);c.line(48,45,W-48,45)
    txt('Vinícius Goulart Alves  ·  Cidades ESG Inteligentes',48,29,8,MUTED)
    txt(f'{page:02}',W-64,29,9,GREEN,'Bold')
    return y-25

def table(rows,y,widths=None,size=9):
    style=ParagraphStyle('cell',fontName='Body',fontSize=size,leading=size*1.4,textColor=INK)
    data=[[Paragraph(str(v),style) for v in row] for row in rows]
    t=Table(data,colWidths=widths or [160,339],hAlign='LEFT')
    t.setStyle(TableStyle([('BACKGROUND',(0,0),(-1,0),HexColor('#DFEBE3')),('VALIGN',(0,0),(-1,-1),'TOP'),('LEFTPADDING',(0,0),(-1,-1),10),('RIGHTPADDING',(0,0),(-1,-1),10),('TOPPADDING',(0,0),(-1,-1),8),('BOTTOMPADDING',(0,0),(-1,-1),8),('LINEBELOW',(0,0),(-1,-1),.5,LINE),('ROWBACKGROUNDS',(0,1),(-1,-1),[white,HexColor('#F7FAF7')])]))
    _,h=t.wrap(499,700);t.drawOn(c,48,y-h);return y-h

def pic(path,y,maxh=360):
    im=Image.open(path); iw,ih=im.size;scale=min(499/iw,maxh/ih);w,h=iw*scale,ih*scale
    c.setStrokeColor(LINE);c.setFillColor(white);c.roundRect(46,y-h-2,503,h+4,6,fill=1,stroke=1)
    c.drawImage(str(path),48+(499-w)/2,y-h,width=w,height=h)
    return y-h

def link(label,url,y):
    return para(f'<link href="{escape(url)}" color="#14805F"><u>{escape(label)}</u></link>',y,9)

def code(lines,y,size=8.5):
    h=20+len(lines)*(size*1.55)
    c.setFillColor(NAVY);c.roundRect(48,y-h,499,h,9,fill=1,stroke=0)
    for i,line in enumerate(lines):txt(line,61,y-18-i*size*1.55,size,HexColor('#D9F1E3'),'Mono')
    return y-h

def end():c.showPage()

# 1. Overview
y=head('01 / Projeto e resultado','EcoColeta API','Integração de práticas DevOps à gestão de resíduos e reciclagem.')
y=para('Navegando pelo mundo DevOps',y,20,GREEN);y=para('Integrante: Vinícius Goulart Alves<br/>Instituição: FIAP · Outubro de 2026',y-17,11)
y=para('A aplicação apoia cidades ESG inteligentes com pontos de coleta seletiva, registro de descartes, alertas por ocupação e indicadores de impacto ambiental. A entrega automatiza o caminho do código até dois ambientes de execução.',y-26)
y=table([['Validação observada','Resultado'],['Build Release','Aprovado; sem erros de compilação'],['Testes automatizados',f"{counts['passed']} aprovados / {counts['total']} executados"],['Deploy staging','API + SQL Server; smoke e persistência aprovados'],['Deploy produção acadêmica','Mesma imagem de staging; smoke e persistência aprovados'],['Rastreabilidade',f"GitHub Actions · execução #{run['run_number']} · 12/10/2026"]],y-24,[180,319])
y=para('Escopo da demonstração',y-25,13,GREEN)
y=para('Os dois ambientes são reais, porém temporários, executados em runners isolados do GitHub Actions. As stacks são encerradas ao final de cada job. A entrega não inclui hospedagem pública permanente; SQL Server Developer é utilizado apenas para fins acadêmicos.',y-9,10)
y=link('Abrir a execução validada no GitHub',run_url,y-20)
end()

# 2. Pipeline with real screenshot
y=head('02 / Integração e entrega contínuas','Pipeline CI/CD','GitHub Actions com promoção condicionada entre ambientes.')
y=pic(E/'pipeline.jpg',y,maxh=270)
y=para('Figura 1. Captura da execução no GitHub Actions. Os arquivos de workflow e trechos de log acompanham o código-fonte.',y-10,8.5,MUTED)
y=table([['Etapa','Comportamento e condição'],['Build e testes','Restore NuGet, build Release, suíte xUnit e cobertura. Falha interrompe a cadeia.'],['Imagem única','Docker multi-stage; tag por SHA; artefato compactado e checksum SHA-256.'],['Staging','Valida checksum, sobe Compose, executa smoke e teste de persistência.'],['Produção acadêmica','Depende de staging aprovado; usa a mesma imagem, sem recompilar.'],['Evidências e limpeza','Captura tela, saúde, contêineres e logs; publica artefatos e remove a stack temporária.']],y-16,[118,381],8.5)
y=para('Push em main ou codex/devops-atividade dispara CI/CD. Pull requests executam CI sem deploy. Os jobs usam GitHub Environments staging e production. Código: .github/workflows/ci-cd.yml e deploy-environment.yml.',y-16,9)
end()

# 3 Docker
y=head('03 / Contêineres e infraestrutura','Docker e orquestração','API + banco com inicialização coordenada e dados persistidos em volume.')
y=table([['Camada','Estratégia'],['Imagem da API','SDK .NET compila; ASP.NET Runtime executa. Usuário APP_UID sem privilégios; porta interna 8080.'],['Banco','SQL Server 2022 Developer em rede bridge; porta 1433 não publicada no host.'],['Orquestração','Compose aguarda healthcheck SQL (SELECT 1) antes da API; up --wait bloqueia até a stack ficar saudável.'],['Volume e rede','sqldata em /var/opt/mssql e rede backend; nomes prefixados pelo projeto Compose.'],['Configuração','Variáveis para ambiente, conexão, JWT e seed. Credenciais aleatórias por job e arquivo .env ignorado pelo Git.']],y,[127,372])
y=para('Comandos usados',y-23,13,GREEN)
y=code(['docker build --build-arg APP_VERSION=<SHA> \\','  -t ecocoleta-api:<SHA> .','docker compose config --quiet','docker compose up -d --no-build --wait \\','  --wait-timeout 240','docker compose stop','docker compose up -d --no-build --wait \\','  --wait-timeout 240'],y-13,8.4)
y=para('Imagem implantada',y-22,12,GREEN)
y=para('ecocoleta-api:<br/>'+sha,y-8,9)
y=para('A imagem é criada uma vez e transportada como artefato. O ID registrado em staging e produção permite confirmar a identidade. As bases usam tags de manutenção; o README registra a limitação de reprodutibilidade entre builds futuros.',y-14,9)
end()

# 4 tests
y=head('04 / Qualidade e rastreabilidade','Build e testes automatizados','Resultados extraídos dos arquivos TRX e Cobertura publicados pelo workflow.')
y=table([['Métrica','Resultado'],['Suíte xUnit',f"{counts['total']} executados · {counts['passed']} aprovados · {counts['failed']} falhas"],['Cobertura de linhas',f"{float(cov['line-rate'])*100:.2f}% ({cov['lines-covered']} de {cov['lines-valid']} linhas)".replace('.',',')],['Cobertura de ramificações',f"{float(cov['branch-rate'])*100:.2f}%".replace('.',',')]],y,[190,309])
y=para('Cenários da suíte',y-22,13,GREEN)
y=table([['Grupo','Verificação'],['Consultas (5)','Pontos, registros, alertas, destinações e relatório retornam HTTP 200.'],['Autenticação (2)','Login válido emite JWT; operação protegida sem token retorna 401.'],['Regra ESG (1)','Descarte acima de 80% da capacidade gera alerta.'],['Operação (3)','Readiness com banco/ambiente; liveness; Swagger desabilitado não expõe especificação.']],y-12,[135,364],9)
y=para('Trecho do log real de build/testes',y-22,13,GREEN)
log=(E/'build-test-image.log.txt').read_text()
selected=[]
for line in log.splitlines():
    if any(k in line for k in ['Build succeeded.','0 Warning(s)','0 Error(s)','Passed!']):
        clean=line[29:].strip() if len(line)>29 else line
        if 'Passed!' in clean:clean=f"Passed: {counts['passed']} | Failed: {counts['failed']} | Total: {counts['total']}"
        selected.append(clean)
y=code(selected[:4],y-12,8.7)
y=para('A suíte usa EF Core InMemory. O deploy complementa essa cobertura com SQL Server real, migrations, fluxo autenticado, alerta e persistência após reinício. Os 11 testes aprovados não significam cobertura completa do sistema.',y-20,9)
y=link('Conferir job de build, testes e imagem',jobs[0]['html_url'],y-16)
end()

# 5 and 6 environments
for index,env,label in [(5,'staging','Staging'),(6,'production','Produção acadêmica')]:
    smoke=json.loads((E/env/'smoke.json').read_text());persist=json.loads((E/env/'persistence.json').read_text());health=smoke['health']
    y=head(f'0{index} / Deploy validado',label,'Captura real da API executando em contêiner no runner do GitHub Actions.')
    y=pic(E/env/'ambiente.png',y,maxh=380)
    y=para(f'Figura {index-3}. Painel consultando /health/ready; ambiente {health["environment"]}, banco conectado e versão do commit.',y-11,8.5,MUTED)
    y=table([['Evidência','Resultado'],['Prontidão',f"{health['status']} · {health['database']} · {health['environment']}"],['Smoke HTTP',f"{len(smoke['checks'])} verificações HTTP + validações de conteúdo; resultado {smoke['result']}"],['Persistência',f"Mesmo ponto ID {persist['point_id']} com {persist['occupancy_kg']} kg após reiniciar API e banco."],['Data UTC',smoke['timestamp_utc']]],y-17,[116,383],8.5)
    job=next(j for j in jobs if ('staging' if env=='staging' else 'producao') in j['name'].lower())
    y=link('Abrir job e logs deste ambiente',job['html_url'],y-16)
    end()

# 7 challenges and setup
y=head('07 / Decisões e operação','Desafios e soluções','Problemas identificados durante a implementação e validação.')
y=table([['Desafio','Solução aplicada'],['Inicialização do banco','Healthcheck SQL e depends_on: service_healthy; bloqueio de prontidão no Compose.'],['Configuração JWT nos testes','Primeira execução apontou validação antecipada da chave. A configuração JWT passou a ser resolvida por DI e validada após Build(), incorporando os overrides da factory.'],['Segredos fixos','Remoção de credenciais de appsettings e do seed; variáveis de ambiente e máscaras nos logs.'],['Validação além de HTTP 200','Smoke autentica, grava dados, provoca alerta, consulta relatório e verifica persistência após reiniciar os dois serviços.'],['Promoção previsível','Reutilização da mesma imagem em staging e produção, com checksum do artefato e ID da imagem registrado.'],['Ambiente de autoria limitado','Build, suíte e execução Docker realizados nos runners do GitHub; evidências recuperadas pelos artefatos e logs.']],y,[136,363],9)
y=para('Reprodução local',y-24,13,GREEN)
y=code(['cp .env.example .env','# Substitua os valores de exemplo no arquivo .env','docker compose up -d --build --wait \\','  --wait-timeout 240','# Painel: http://localhost:8080'],y-12,8.3)
y=para('Para hospedar continuamente',y-22,13,GREEN)
y=para('Provisionar um host ou serviço cloud e executar o deploy no destino, mantendo os contêineres ativos. Para operação comercial, acrescentar TLS, cofre de segredos, usuário SQL de privilégio mínimo, licença adequada, backups e monitoramento. Essas etapas não estão realizadas nesta demonstração.',y-10,9)
end()

# 8 final checklist
y=head('08 / Conferência final','Checklist de entrega','Estado verificado para a execução vinculada nesta documentação.')
y=table([['Item obrigatório','OK / Evidência'],['Projeto compactado em ZIP com estrutura organizada','OK - src/, tests/, scripts/, Dockerfile, Compose, workflows e README.'],['Dockerfile funcional','OK - imagem construída e executada nos dois ambientes.'],['docker-compose.yml ou Kubernetes','OK - API + SQL Server, volume, rede e variáveis.'],['Pipeline com build, teste e deploy','OK - execução completa aprovada no GitHub Actions.'],['README com instruções e prints','OK - comandos, arquitetura, links e capturas em docs/evidencias/.'],['Documentação técnica com evidências','OK - este PDF, capturas reais, resultados e trechos de logs.'],['Deploy em staging e produção','OK - dois ambientes acadêmicos temporários, com smoke e persistência aprovados.']],y,[275,224],9)
y=para('Identificação da versão validada',y-23,13,GREEN)
y=para('Commit: '+sha+'<br/>Execução: '+str(run['id'])+' · conclusão: success<br/>Branch: codex/devops-atividade',y-10,9)
y=para('O ZIP contém a implementação validada e a documentação/evidências acrescentadas depois da execução. A branch principal foi preservada. O checklist não afirma existência de produção pública permanente.',y-15,9)
y=para('Referências técnicas',y-23,13,GREEN)
for label,url in [('Docker - ordem de inicialização','https://docs.docker.com/compose/how-tos/startup-order/'),('Microsoft - contêineres no .NET 8','https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8/containers'),('GitHub - ambientes de implantação','https://docs.github.com/en/actions/concepts/workflows-and-actions/deployment-environments')]:
    y=link(label,url,y-8)
y=para('Envio: ZIP do projeto e PDF técnico, ambos com checklist.',y-10,9)
end()
c.save()
print(OUT)
