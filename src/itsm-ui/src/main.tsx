import { StrictMode, useEffect, useState } from 'react';
import { createRoot } from 'react-dom/client';
import { getSystem } from './api.mjs';
import { logo } from './brand';
import './styles.css';

function App() {
  const [view, setView] = useState<'home' | 'platform'>('home');
  const [state, setState] = useState<'loading' | 'connected' | 'error'>('loading');
  const [attempt, setAttempt] = useState(0);
  useEffect(() => {
    const controller = new AbortController();
    setState('loading');
    getSystem(fetch, controller.signal).then(() => setState('connected')).catch(() => {
      if (!controller.signal.aborted) setState('error');
    });
    return () => controller.abort();
  }, [attempt]);
  return <div className="shell">
    <aside><a className="brand" href="#inicio" onClick={() => setView('home')}><img src={logo} alt="Colégio Arbos" /></a>
      <div className="small">CENTRAL DE SERVIÇOS</div>
      <nav aria-label="Navegação principal">
        <button className={view === 'home' ? 'active' : ''} aria-current={view === 'home' ? 'page' : undefined} onClick={() => setView('home')}>Início</button>
        <button className={view === 'platform' ? 'active' : ''} aria-current={view === 'platform' ? 'page' : undefined} onClick={() => setView('platform')}>Estado da plataforma</button>
      </nav><div className="asidefoot"><span className="service-dot" />TI e Audiovisual<br />Fundação da aplicação</div>
    </aside>
    <div className="workspace"><header><div><span className="header-label">COLÉGIO ARBOS</span><span className="header-separator"> / </span>Central de Serviços</div><span className="badge">Em preparação</span></header>
      <div className="demo">VERSÃO DE DESENVOLVIMENTO · Atendimento e autenticação ainda não habilitados.</div>
      <main id="inicio"><div className="pagehead"><div><div className="eyebrow">Tecnologia e atendimento</div><h1>{view === 'home' ? 'Bem-vindo à Central de Serviços.' : 'Estado da plataforma'}</h1><p className="muted">{view === 'home' ? 'Uma experiência integrada para a comunidade Arbos.' : 'Conectividade da aplicação, sem dados de produção.'}</p></div></div>
        {view === 'home' && <><section className="hero"><div><div className="hero-kicker">TECNOLOGIA A SERVIÇO DA EDUCAÇÃO</div><h2>Para ensinar, criar e trabalhar.<br />Conte com a nossa equipe.</h2><p>Estamos preparando o acesso corporativo e o atendimento. A abertura de chamados será disponibilizada após a integração e a validação dos serviços.</p></div><button className="btn" onClick={() => setView('platform')}>Ver estado da plataforma</button></section>
        <div className="cards foundation-cards">{['Sistemas e portais', 'Operações de TI', 'Audiovisual'].map(name => <section className="card" key={name}><div className="eyebrow">Atendimento</div><h2>{name}</h2><p className="small">Disponível em uma próxima entrega.</p></section>)}</div></>}
        <section className="card"><div className="row"><h2>Conexão com o serviço</h2><span role="status" aria-live="polite" className="badge">{state === 'loading' ? 'Verificando…' : state === 'connected' ? 'Backend conectado' : 'Serviço indisponível'}</span></div>
          <p className="muted">{state === 'connected' ? 'A interface React está conectada ao backend. O sistema ainda não está pronto para receber chamados reais.' : state === 'error' ? 'Não foi possível confirmar a conexão. Inicie o backend ou tente novamente.' : 'Consultando o serviço da aplicação.'}</p>
          <button className="btn" disabled={state === 'loading'} onClick={() => setAttempt(n => n + 1)}>Verificar novamente</button>
          <div className="divider" /><div className="row"><span>Login Microsoft 365</span><span>Não configurado</span></div><div className="row"><span>Persistência SQL Server</span><span>Não configurada</span></div><div className="row"><span>Abertura de chamados</span><span>Não habilitada</span></div>
        </section><footer>Colégio Arbos · Central de Serviços · Fundação técnica</footer>
      </main></div></div>;
}
createRoot(document.getElementById('root')!).render(<StrictMode><App /></StrictMode>);
