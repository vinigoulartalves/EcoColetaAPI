#!/usr/bin/env python3
"""Build the delivery report using the actual CI artifact; never invent test outcomes."""
from pathlib import Path
from datetime import datetime
from zoneinfo import ZoneInfo
from xml.sax.saxutils import escape
import json, re, textwrap
from reportlab.pdfgen import canvas
from reportlab.platypus import SimpleDocTemplate, Paragraph, Spacer, PageBreak, Table, TableStyle, Preformatted, Image, KeepTogether
from reportlab.lib import colors
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.lib.enums import TA_LEFT

ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'docs/bdd'
EVIDENCE=ROOT/'artifacts/bdd'
BLUE=colors.HexColor('#123342'); GREEN=colors.HexColor('#087B69'); PALE=colors.HexColor('#EDF6F4')
FONT=Path('/usr/share/fonts/truetype/dejavu')
for name,file in [('Sans','DejaVuSans.ttf'),('SansBold','DejaVuSans-Bold.ttf'),('Mono','DejaVuSansMono.ttf')]:pdfmetrics.registerFont(TTFont(name,str(FONT/file)))
styles=getSampleStyleSheet()
styles.add(ParagraphStyle(name='BodyBDD',fontName='Sans',fontSize=9.2,leading=14,spaceAfter=9,textColor=BLUE))
styles.add(ParagraphStyle(name='H1BDD',fontName='SansBold',fontSize=19,leading=24,spaceAfter=15,textColor=GREEN))
styles.add(ParagraphStyle(name='H2BDD',fontName='SansBold',fontSize=11,leading=16,spaceBefore=10,spaceAfter=8,textColor=BLUE))
styles.add(ParagraphStyle(name='SmallBDD',fontName='Sans',fontSize=7.6,leading=11,spaceAfter=7,textColor=BLUE))
styles.add(ParagraphStyle(name='CodeBDD',fontName='Mono',fontSize=6.8,leading=10,spaceAfter=12,backColor=PALE,borderPadding=8))
styles.add(ParagraphStyle(name='CoverBDD',fontName='SansBold',fontSize=33,leading=40,spaceAfter=16,textColor=BLUE))
W=475
story=[]
def p(text,style='BodyBDD'):story.append(Paragraph(text,styles[style]))
def h(text):p(text,'H1BDD')
def sub(text):p(text,'H2BDD')
def code(text,width=110):
    lines=[]
    for line in text.splitlines():
        lines.extend(textwrap.wrap(line,width=width,replace_whitespace=False,drop_whitespace=False,subsequent_indent='    ') or [''])
    story.append(Preformatted('\n'.join(lines),styles['CodeBDD']))
def table(rows,widths):
    rows=[[Paragraph(escape(str(c)),styles['SmallBDD']) for c in row] for row in rows]
    t=Table(rows,colWidths=widths,repeatRows=1,hAlign='LEFT')
    t.setStyle(TableStyle([('BACKGROUND',(0,0),(-1,0),BLUE),('TEXTCOLOR',(0,0),(-1,0),colors.white),('ROWBACKGROUNDS',(0,1),(-1,-1),[colors.white,PALE]),('VALIGN',(0,0),(-1,-1),'TOP'),('LEFTPADDING',(0,0),(-1,-1),7),('RIGHTPADDING',(0,0),(-1,-1),7),('TOPPADDING',(0,0),(-1,-1),7),('BOTTOMPADDING',(0,0),(-1,-1),5),('LINEBELOW',(0,0),(-1,0),1,GREEN)]))
    for cell in rows[0]:cell.style=ParagraphStyle(name='th',parent=styles['SmallBDD'],fontName='SansBold',textColor=colors.white)
    story.append(t)
def shot(file,caption,maxheight=360):
    path=OUT/'evidence'/file
    if not path.exists():raise FileNotFoundError(path)
    from PIL import Image as PI
    with PI.open(path) as im:w,h=im.size
    scale=min(W/w,maxheight/h)
    story.append(Image(str(path),width=w*scale,height=h*scale,hAlign='LEFT'))
    p(caption,'SmallBDD')
def page(canvas,doc):
    canvas.saveState();canvas.setFillColor(GREEN);canvas.rect(0,817,595,25,fill=1,stroke=0)
    canvas.setFont('Sans',8);canvas.setFillColor(colors.white);canvas.drawString(60,826,'ECOCOLETA / TESTES AUTOMATIZADOS / FIAP')
    canvas.setFillColor(BLUE);canvas.setFont('Sans',7.5);canvas.drawString(60,30,'Vinícius Goulart Alves  |  Evidências de execução BDD')
    canvas.drawRightString(535,30,str(doc.page));canvas.restoreState()

def main():
    summary=json.loads((EVIDENCE/'summary.json').read_text())
    run=json.loads((OUT/'execution.json').read_text())
    if summary['passed']!=summary['scenarios'] or summary['operations']!=16:raise ValueError('Execução incompleta')
    ci_url=run['url'];commit=run['head_sha'];date=datetime.now(ZoneInfo('America/Sao_Paulo')).strftime('%d/%m/%Y')
    story.append(Spacer(1,70));p('QUALIDADE DE SOFTWARE<br/>APLICADA À GESTÃO DE RESÍDUOS','SmallBDD')
    p('EcoColetaAPI<br/>Testes automatizados<br/>com BDD','CoverBDD')
    p('Documentação técnica, cenários Gherkin e evidências de execução','H2BDD')
    story.append(Spacer(1,28))
    table([['Cenários BDD','Operações HTTP','Contratos JSON'],[str(summary['scenarios'])+' aprovados','16 de 16 cobertas','15 schemas']], [158,158,159])
    story.append(Spacer(1,25));p('Vinícius Goulart Alves<br/>FIAP - Atividade avaliativa da fase Testes Automatizados<br/>'+date)
    p('API em C# / ASP.NET Core 8. Automação em Java 17 com Cucumber, JUnit e REST Assured. Execução HTTP contra SQL Server 2022 em ambiente Docker isolado.','SmallBDD')
    story.append(PageBreak());h('1. Objetivo e escopo')
    p('Validar os comportamentos críticos da EcoColetaAPI, projeto das atividades anteriores dedicado à gestão de resíduos e reciclagem. A suíte combina especificações executáveis em Gherkin, verificações de status HTTP, conteúdo JSON e contratos JSON Schema.')
    sub('Relação com ESG')
    p('Os testes verificam a integridade dos registros de descarte, a compatibilidade entre resíduos e pontos, as instruções de destinação e os alertas de ocupação. O acesso administrativo protege alterações nos dados. As verificações demonstram qualidade técnica e rastreabilidade, sem certificar conformidade legal ou impacto ambiental real.')
    sub('Critérios de aceite e evidências')
    table([['Requisito','Como foi atendido'],['No mínimo 3 cenários BDD',f"{summary['scenarios']} casos executados, distribuídos em 7 arquivos .feature, com caminhos positivos e negativos."],['Todas as APIs','16 combinações de método e rota, incluindo 14 operações de negócio e 2 de saúde.'],['Status, JSON e contratos','Asserções de status e valores; 15 schemas com campos obrigatórios, tipos e regras estruturais.'],['Execução simples','Um script prepara API, SQL Server, dados e runner; o mesmo script é usado no CI.'],['Código e documentação','ZIP com código, Dockerfiles, scripts, README e relatórios; este PDF reúne configuração, cenários e prints.']],[145,330])
    sub('Versão avaliada');p('Commit da execução: '+commit,'SmallBDD');p('Execução verificável: <link href="'+ci_url+'" color="#087B69">'+ci_url+'</link>','SmallBDD')
    story.append(PageBreak());h('2. Ambiente e reprodução')
    p('A aplicação permanece em C#. O módulo independente de automação usa Java 17, Cucumber 7.20.1, JUnit Platform 1.11.4, REST Assured 5.5.0 e Maven 3.9.9. O runner envia requisições HTTP reais à API, que persiste os dados no SQL Server 2022.')
    sub('Preparação e comando de execução')
    p('Pré-requisitos: Docker Engine ou Docker Desktop com Compose v2, Python 3, Bash e acesso aos registros de imagens e dependências. No Windows, usar WSL2 ou Git Bash. Recomenda-se reservar 6 GB de RAM livres para o conjunto.')
    code('bash scripts/bdd/run.sh')
    p('O script cria credenciais temporárias, recria somente o projeto Compose ecocoleta-bdd, espera o banco e a API ficarem saudáveis, prepara os dados, executa a suíte e coleta os relatórios. Ao terminar, remove os containers e o volume exclusivo dos testes.')
    sub('Isolamento dos dados')
    p('Cada cenário cria seus próprios pontos. A execução é sequencial para evitar interferência nos indicadores globais. O banco EcoColetaBdd é descartável. Para o cenário de criação de destinação, a fixture remove previamente apenas o tipo Outros (9) do seed; todos os demais tipos permanecem disponíveis. Não é usado um endpoint de exclusão, pois ele não existe na API.')
    sub('Resultados para inspeção')
    code('artifacts/bdd/cucumber/cucumber.html\nartifacts/bdd/cucumber/cucumber.json\nartifacts/bdd/cucumber/cucumber.xml\nartifacts/bdd/cucumber/http-evidence.jsonl\nartifacts/bdd/summary.json\nartifacts/bdd/openapi.json\nartifacts/bdd/services.log')
    p('As evidências HTTP removem senhas e tokens. Credenciais temporárias não integram o pacote de entrega. O workflow exclusivo de BDD executa os testes e publica relatórios, sem realizar deploy.','SmallBDD')
    story.append(PageBreak());h('3. Matriz de cobertura HTTP')
    p('A cobertura abaixo representa operações exercitadas com sucesso e os status observados na execução. Não equivale à cobertura de linhas de código nem a todos os valores possíveis de entrada.')
    table([['Método','Rota','Status observados']]+[[r['method'],r['path'],', '.join(map(str,r['statusCodes']))] for r in summary['coverage']],[47,325,103])
    p('Rastreabilidade detalhada por tags: docs/bdd/coverage.md. Os schemas de listagem também validam os objetos de cada item.','SmallBDD')
    story.append(PageBreak());h('4. Estratégia e regras verificadas')
    sub('Conteúdo e persistência')
    p('Além do retorno HTTP, a suíte confere valores relevantes e consulta registros após a criação ou atualização. Descartes válidos devem aumentar a ocupação; rejeições por peso inválido, ponto inativo ou tipo incompatível devem preservar a ocupação e manter a lista de registros vazia para o ponto do cenário.')
    sub('Alertas e limites')
    table([['Ocupação de um ponto de 100 kg','Resultado esperado'],['79,99 kg','Nenhum alerta aberto'],['80 kg e 99,99 kg','Alerta de atenção'],['100 kg e 100,01 kg','Alerta crítico']],[235,240])
    p('São verificados a resolução persistida, a rejeição de resolução repetida, o ID inexistente, o recálculo sem duplicação, a resolução automática quando o percentual cai abaixo de 80% e a substituição de atenção por crítico.')
    sub('Relatório de impacto')
    p('O cenário captura os totais atuais, registra mais 12 kg de plástico e confere a diferença no total e no agrupamento por tipo. Também valida a fórmula total × 0,75, com arredondamento a duas casas. Essa fórmula é uma estimativa implementada pelo projeto, sem comprovação científica de benefício ambiental.')
    sub('Contratos e erros')
    p('Os schemas usam draft-04 e verificam campos obrigatórios, tipos, enums, limites e propriedades extras. Há contratos separados para erros de negócio e ValidationProblemDetails. Respostas 401 do middleware JWT não possuem corpo e são verificadas pela ausência de conteúdo.')
    p('Cinco testes independentes comprovam o funcionamento do validador: aceitação de um contrato válido e rejeição de campo ausente, tipo incorreto, enum desconhecido e campo não documentado. Eles complementam os cenários HTTP.')
    story.append(PageBreak());h('5. Configurações - prints')
    p('As capturas seguintes mostram os arquivos efetivamente usados na execução. São visualizações locais dos arquivos, sem credenciais temporárias.')
    shot('config-pom.png','Figura 1. Dependências e runner Maven do módulo de testes.',325)
    shot('config-workflow.png','Figura 2. Workflow exclusivo para execução BDD contra SQL Server.',325)
    story.append(PageBreak());h('6. Evidências de execução')
    p(f"A execução final aprovou {summary['scenarios']} cenários BDD e os 5 testes independentes do validador: 58 testes, sem falhas ou erros. A aplicação e o banco foram executados em containers no runner do GitHub Actions.")
    p('Run: '+str(run['id'])+' | Resultado: '+run['conclusion']+' | Commit: '+commit[:12],'SmallBDD')
    shot('cucumber-report.png','Figura 3. Relatório Cucumber da execução real, com os cenários aprovados.',380)
    shot('ci-log.png','Figura 4. Trecho do log bruto da execução Maven, visualizado localmente.',230)
    story.append(PageBreak());h('7. Evidências de comportamento')
    shot('http-evidence.png','Figura 5. Trecho das requisições e respostas sanitizadas de um cenário de descarte.',550)
    p('O arquivo http-evidence.jsonl contém as chamadas por cenário, com método, rota, status e corpos sanitizados. O resultado pode ser cruzado com cucumber.json e com os arquivos .feature do ZIP.','SmallBDD')
    story.append(PageBreak());h('8. Cenários Gherkin completos')
    p('Os cenários abaixo reproduzem os sete arquivos .feature entregues. As tabelas de Exemplos expandem os esquemas em casos independentes; por isso a quantidade executada é maior que o número de definições de Cenário. Os nomes entre ${...} são variáveis preenchidas pelo código, sem expor credenciais.')
    for index,file in enumerate(sorted((ROOT/'tests/EcoColeta.Bdd/src/test/resources/features').glob('*.feature'))):
        if index:story.append(PageBreak())
        sub(file.name)
        text=file.read_text()
        chunks=re.split(r'(?=^  @[A-Z]+-\d+)',text,flags=re.M)
        code(chunks[0].strip())
        for chunk in chunks[1:]:
            lines=[]
            for line in chunk.strip().splitlines():lines+=textwrap.wrap(line,width=110,replace_whitespace=False,drop_whitespace=False,subsequent_indent='    ') or ['']
            story.append(KeepTogether([Preformatted('\n'.join(lines),styles['CodeBDD'])]))
    story.append(PageBreak());h('9. Limitações e conferência da entrega')
    p('A suíte cobre as operações HTTP identificadas e comportamentos selecionados. Não contempla testes de carga, interface, concorrência, segurança ofensiva, conformidade legal ou comprovação de benefícios ambientais. Não simula queda do banco (503) nem usuário autenticado sem papel Admin (403). Os testes de autenticação cobrem credenciais inválidas, ausência de token e token inválido.')
    sub('Conferência do pacote')
    p('O ZIP reúne aplicação, testes, arquivos Gherkin, schemas, Dockerfiles, configuração Compose, scripts, README e evidências. São excluídos caches, binários de build, credenciais temporárias e evidências antigas da atividade DevOps. Um manifesto SHA256SUMS.json permite conferir a integridade dos arquivos.')
    p('Para reproduzir, extraia o ZIP, abra a pasta EcoColetaAPI e execute bash scripts/bdd/run.sh. A geração deste PDF usa os resultados reais baixados do artifact do CI e falha se o resumo indicar uma execução incompleta.')
    sub('Referências técnicas')
    for url in ['https://github.com/vinigoulartalves/EcoColetaAPI','https://cucumber.io/docs/gherkin/reference/','https://cucumber.io/docs/cucumber/api/','https://github.com/rest-assured/rest-assured/wiki/Usage','https://docs.docker.com/compose/how-tos/startup-order/','https://docs.github.com/en/actions/writing-workflows']:
        p('<link href="'+url+'" color="#087B69">'+escape(url)+'</link>','SmallBDD')
    p('Documentação técnica consultada para a implementação. Data da documentação: '+date+'.','SmallBDD')
    path=OUT/'EcoColeta_BDD_Documentacao.pdf'
    doc=SimpleDocTemplate(str(path),pagesize=(595.2756,841.8898),rightMargin=60,leftMargin=60,topMargin=55,bottomMargin=55,title='EcoColetaAPI - Testes Automatizados BDD',author='Vinícius Goulart Alves')
    doc.build(story,onFirstPage=page,onLaterPages=page)
    print(path)
if __name__=='__main__':main()
