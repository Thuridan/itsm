// Testes de comportamento do protótipo; não substituem QA visual ou autorização no servidor.
const fs = require('node:fs');
const vm = require('node:vm');
const assert = require('node:assert/strict');
const html = fs.readFileSync(__dirname + '/index.html', 'utf8');
const source = html.split('<script>')[1].split('</script>')[0];
const nodes = new Map();
const node = s => { if (!nodes.has(s)) nodes.set(s, {innerHTML:'',textContent:'',value:'',focus(){}}); return nodes.get(s); };
let click;
const ctx = vm.createContext({
 document: {querySelector:node, addEventListener(type,fn){ if(type==='click')click=fn; }},
 window:{scrollTo(){}}, setTimeout(){}, confirm(){return true;},
 prompt(){return 'Solução ou motivo fictício';},
 FormData:class {constructor(target){this.fields=target.fields;}get(k){return this.fields[k];}}
});
vm.runInContext(source, ctx);
const run = s => vm.runInContext(s,ctx);
function action(a,arg=''){click({preventDefault(){},target:{closest(){return {dataset:{action:a,arg:String(arg)}};}}});}
assert.equal(run('visible().length'),3);
run("role='manager'");assert.equal(run("visible().every(t=>t.area==='Financeiro')"),true);
run("role='tech'");assert.equal(run("visible().some(t=>t.group==='Audiovisual')"),false);
run("role='admin'");assert.equal(run('visible().length'),8);
for(const r of ['user','tech','manager','admin']){
 run("role='"+r+"'");
 for(const p of ['home','new','mine','detail',...(r==='tech'||r==='admin'?['queue']:[]),...(r==='manager'||r==='admin'?['dashboard']:[])]){
  run("page='"+p+"';render()");
  assert.ok(node('#app').innerHTML.length>100);
 }
}
run("role='tech';page='detail';selected=1046");
action('assume',1046);assert.equal(run("tickets.find(t=>t.id===1046).assignee"),'Rafael Lima');
const historyCount=run("tickets.find(t=>t.id===1046).history.length");
action('assume',1046);assert.equal(run("tickets.find(t=>t.id===1046).history.length"),historyCount);
run("role='tech';selected=1048;mode='internal';page='detail';render()");
node('#reply').value='INTERNAL_SECRET_DEMO';
run("reply({preventDefault(){}})");
run("role='user';render()");assert.ok(!node('#app').innerHTML.includes('INTERNAL_SECRET_DEMO'));
run("role='tech';mode='public';render()");
node('#reply').value='Confirme a conexão utilizada.';
action('wait',1048);assert.equal(run("tickets.find(t=>t.id===1048).status"),'Aguardando solicitante');
run("role='user';render()");node('#reply').value='Conexão confirmada.';run("reply({preventDefault(){}})");
assert.equal(run("tickets.find(t=>t.id===1048).status"),'Em atendimento');
run("role='tech';render()");action('close',1048);
assert.equal(run("tickets.find(t=>t.id===1048).status"),'Encerrado');
const size=run('tickets.length');
run("role='user';render()");action('reopen',1048);
assert.equal(run('tickets.length'),size);
assert.equal(run("tickets.find(t=>t.id===1048).cycle"),2);
assert.equal(run("tickets.find(t=>t.id===1048).closures"),1);
assert.equal(run("tickets.find(t=>t.id===1048).reopens"),1);
run("role='user';page='new';render()");
ctx.formInput={fields:{title:'<img src=x onerror=alert(1)>',description:'Teste de conteúdo',category:'Audiovisual',type:'Problema',unit:'Unidade 3',impact:'Média'}};
run("create({preventDefault(){},target:formInput})");
assert.equal(run('tickets.length'),size+1);
assert.equal(run('tickets[0].group'),'Audiovisual');
assert.ok(!node('#app').innerHTML.includes('<img src=x'));
assert.ok(node('#app').innerHTML.includes('&lt;img'));
run("role='manager';page='dashboard';dashGroup='Sistemas';dashArea='';render()");
assert.ok(!node('#app').innerHTML.includes('Projetor da sala'));
assert.equal(run("tickets.length+tickets.reduce((n,t)=>n+t.reopens,0)-tickets.reduce((n,t)=>n+t.closures,0)"),run("tickets.filter(t=>t.status!=='Encerrado').length"));
console.log('PASS: renderização lógica por perfil, escopo simulado, assunção repetida, nota interna, pausa/retomada, encerramento/reabertura, criação, escape HTML e reconciliação.');
