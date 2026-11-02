// Screenshots of the actual generated report and actual source files, taken in CI.
const {chromium}=require('playwright');
const fs=require('fs');
const path=require('path');
const root=path.resolve(__dirname,'../..');
const out=path.join(root,'artifacts/bdd/screenshots');
const esc=s=>s.replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;');
(async()=>{
 fs.mkdirSync(out,{recursive:true});
 const browser=await chromium.launch({headless:true});
 const page=await browser.newPage({viewport:{width:1200,height:850},deviceScaleFactor:1.5});
 await page.goto('file://'+path.join(root,'artifacts/bdd/cucumber/cucumber.html'));
 await page.waitForLoadState('networkidle');
 await page.screenshot({path:path.join(out,'cucumber-report.png')});
 async function capture(name,title,text){
  await page.setContent(`<html><meta charset="utf-8"><style>body{margin:0;padding:28px;background:#edf6f4;color:#123342;font:18px Arial}h1{font-size:23px}pre{font:20px/1.45 monospace;background:white;padding:22px;white-space:pre-wrap;overflow-wrap:anywhere;border-left:5px solid #087b69}</style><h1>${esc(title)}</h1><pre>${esc(text)}</pre></html>`);
  await page.screenshot({path:path.join(out,name+'.png'),fullPage:true});
 }
 const pom=fs.readFileSync(path.join(root,'tests/EcoColeta.Bdd/pom.xml'),'utf8').split('\n');
 await capture('config-pom','EcoColeta / pom.xml - propriedades e dependências (trecho)',pom.slice(5,18).join('\n'));
 const workflow=fs.readFileSync(path.join(root,'.github/workflows/bdd-tests.yml'),'utf8');
 await capture('config-workflow','EcoColeta / bdd-tests.yml - job e execução (trecho)',workflow.slice(workflow.indexOf('jobs:'),workflow.indexOf('      - name: Capturar')));
 const log=fs.readFileSync(path.join(root,'artifacts/bdd-execution.log'),'utf8');
 const results=log.split('\n').filter(x=>/Tests run:|BUILD SUCCESS|Total time:|Finished at:/.test(x));
 await capture('ci-log','Log bruto Maven / execução no GitHub Actions',results.join('\n'));
 const evidence=fs.readFileSync(path.join(root,'artifacts/bdd/cucumber/http-evidence.jsonl'),'utf8').trim().split('\n').map(JSON.parse);
 const scenario=evidence.find(e=>e.scenario==='Registrar descarte válido e atualizar ocupação');
 const record=scenario.exchanges.find(e=>e.method==='POST'&&e.path==='/api/registros-residuos');
 await capture('http-evidence','HTTP real sanitizado / '+scenario.scenario,JSON.stringify({scenario:scenario.scenario,status:scenario.status,exchange:record},null,2));
 await browser.close();
 console.log('Capturas reais geradas em artifacts/bdd/screenshots/');
})().catch(e=>{console.error(e);process.exit(1);});
