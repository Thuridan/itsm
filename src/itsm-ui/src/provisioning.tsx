import {useEffect,useState} from 'react';
import {api,mutate} from './api.mjs';
type Ref={id:string;name:string};
type User={id:string;displayName:string;state:string;unitId:string|null;areaId:string|null;version:string};
export function Provisioning(){
 const [users,setUsers]=useState<User[]>([]),[units,setUnits]=useState<Ref[]>([]),[areas,setAreas]=useState<Ref[]>([]);
 const [page,setPage]=useState(1),[revision,setRevision]=useState(0),[busy,setBusy]=useState(false),[error,setError]=useState('');
 const [selected,setSelected]=useState<User|null>(null),[unit,setUnit]=useState(''),[area,setArea]=useState('');
 useEffect(()=>{const c=new AbortController();setBusy(true);setError('');setSelected(null);
  Promise.all([api('/api/v1/admin/users?page='+page,{signal:c.signal}),api('/api/v1/admin/directory',{signal:c.signal})])
   .then(([u,d])=>{if(!c.signal.aborted){setUsers(u);setUnits(d.units);setAreas(d.areas);}})
   .catch(e=>{if(!c.signal.aborted){setUsers([]);setError(e.message);}})
   .finally(()=>{if(!c.signal.aborted)setBusy(false);});return()=>c.abort();
 },[page,revision]);
 async function activate(e:React.FormEvent){e.preventDefault();if(!selected)return;setBusy(true);setError('');
  try{await mutate('/api/v1/admin/users/'+selected.id+'/activate',{unitId:unit,areaId:area,version:selected.version});setSelected(null);setRevision(n=>n+1);}
  catch(e){setError(e instanceof Error?e.message:'Falha ao ativar.');setBusy(false);}
 }
 return <><div className="pagehead"><div><div className="eyebrow">Administração</div><h1>Cadastros de colaboradores</h1><p className="muted">Libere o acesso vinculando uma área e uma unidade.</p></div></div>
 {error&&<p role="alert" className="notice">{error}</p>}
 <section className="card"><div className="tablewrap"><table><thead><tr><th>Colaborador</th><th>Situação</th><th>Ação</th></tr></thead><tbody>{users.map(u=><tr key={u.id}><td>{u.displayName}</td><td>{u.state==='Pending'?'Pendente':u.state==='Active'?'Ativo':'Desativado'}</td><td>{u.state==='Pending'&&<button className="btn" disabled={busy} onClick={()=>{setSelected(u);setUnit('');setArea('');}}>Vincular e liberar</button>}</td></tr>)}</tbody></table></div>
 {!users.length&&<p role="status">{busy?'Carregando…':'Nenhum cadastro nesta página.'}</p>}
 <div className="actions"><button className="btn" disabled={busy||page===1} onClick={()=>setPage(p=>p-1)}>Anterior</button><span>Página {page}</span><button className="btn" disabled={busy||users.length<50} onClick={()=>setPage(p=>p+1)}>Próxima</button><button className="btn" disabled={busy} onClick={()=>setRevision(n=>n+1)}>Atualizar</button></div></section>
 {selected&&<form className="card" onSubmit={activate}><h2>Liberar {selected.displayName}</h2><p>A ativação concede somente o perfil de colaborador.</p><div className="formgrid"><div className="field"><label htmlFor="unit">Unidade</label><select id="unit" required value={unit} onChange={e=>setUnit(e.target.value)}><option value="">Selecione</option>{units.map(u=><option key={u.id} value={u.id}>{u.name}</option>)}</select></div><div className="field"><label htmlFor="area">Área</label><select id="area" required value={area} onChange={e=>setArea(e.target.value)}><option value="">Selecione</option>{areas.map(a=><option key={a.id} value={a.id}>{a.name}</option>)}</select></div></div><div className="actions"><button className="btn" type="button" disabled={busy} onClick={()=>setSelected(null)}>Cancelar</button><button className="btn primary" disabled={busy||!unit||!area}>Confirmar ativação</button></div></form>}
 </>;
}
