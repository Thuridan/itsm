# Mapa de telas e navegação — ITSM

Estado: especificação de UX e fluxos, sem interface implementada. Regras confirmadas vêm de requisitos.md; permissões e comportamentos pendentes continuam identificados pelo código P correspondente em decisoes.md. Este documento não incorpora o exercício paralelo do Lovable.

## Princípios de experiência

Português do Brasil, interface moderna e limpa. Portal do colaborador com ações simples; operação com tabelas e atalhos claros; gestão com indicadores e detalhamento. Os perfis compartilham a identidade e os dados, mas recebem navegação adequada às permissões.

Pessoas podem acumular papéis. Alternar entre “Meu atendimento”, “Operação” e “Gestão” muda a perspectiva de navegação, nunca concede permissão. Sem troca fictícia de usuário em produção. Proposta: técnicos e administradores chegam à operação; gestores à visão da área; demais ao portal. Preferência de página inicial pode ser definida depois.

Layout proposto: cabeçalho com nome do produto, notificações e conta; navegação lateral compacta no desktop e recolhível no mobile; título da página, ação principal e filtros no conteúdo. Sem menus de CMDB, inventário, reservas ou agendamentos.

## Inventário de telas

Rotas são propostas de interface, não contratos de API. “Todos” significa usuários autenticados e provisionados, dentro do escopo autorizado.

| ID | Tela / rota | Perfil e escopo | Conteúdo e ação principal |
| --- | --- | --- | --- |
| T01 | Entrar /entrar | Não autenticado | Entrar com Microsoft; erro de autenticação e nova tentativa |
| T02 | Acesso pendente /acesso-pendente | Identidade sem vínculo válido | Explicação e orientação para administrador; sem cadastro de privilégios pelo usuário; P09 |
| T03 | Início /inicio | Todos; dados próprios | Abrir chamado, pendências de resposta, chamados recentes e busca de artigos |
| T04 | Novo chamado /chamados/novo | Colaborador; gestor com abertura delegada | Formulário, validação e protocolo |
| T05 | Meus chamados /meus-chamados | Solicitante | Status, número, título, atualização e filtros |
| T06 | Detalhe /chamados/:id | Solicitante, gestor da área, técnico autorizado ou administrador | Conversa, dados, histórico e ações por permissão/estado |
| T07 | Notificações /notificacoes | Destinatário | Lidas/não lidas, link para recurso autorizado |
| T08 | Conhecimento /conhecimento | Todos; visibilidade do artigo | Busca por texto/categoria, lista e acesso a artigo |
| T09 | Artigo /conhecimento/:id | Leitor autorizado | Conteúdo, autor, atualização e versão publicada |
| T10 | Visão operacional /operacao | Técnico: grupos autorizados; administrador: global | Fila pendente, carga e alertas com detalhamento |
| T11 | Filas /operacao/filas | Técnico nos grupos; administrador global | Chamados sem responsável e ações de assunção |
| T12 | Meus atendimentos /operacao/meus-atendimentos | Técnico | Atribuídos, aguardando solicitante e prazos |
| T13 | Gestão de artigos /operacao/artigos | Técnicos e administrador | Rascunhos/publicados/arquivados; criar, editar e publicar |
| T14 | Editor /operacao/artigos/:id/editar | Técnicos e administrador | Conteúdo, visibilidade, categoria, prévia e histórico |
| T15 | Minha área /gestao/area | Gestor com vínculo vigente | Indicadores consolidados/individuais e leitura de chamados; P05 |
| T16 | Visão estratégica /gestao/estrategica | Gestor TI / administrador | Tendências, SLA, reaberturas, demanda e filtros globais |
| T17 | Usuários /administracao/usuarios | Administrador | Vínculos, ativação e papéis; histórico |
| T18 | Organização /administracao/organizacao | Administrador | Unidades, áreas e gestores |
| T19 | Grupos /administracao/grupos | Administrador | Grupos e membros |
| T20 | Categorias /administracao/categorias | Administrador | Nome, grupo padrão e ativação |
| T21 | SLA /administracao/sla | Administrador | Severidades, matriz de impacto, políticas versionadas e alertas |
| T22 | Calendários /administracao/calendarios | Administrador | Janelas semanais, feriados, fuso e exceções |
| T23 | Auditoria /administracao/auditoria | Administrador | Busca de operações e alterações sanitizadas |

T10–T12 não concedem acesso global aos técnicos: abrangência exata entre grupos deve ser confirmada em P20. T15 não expõe dados fora da área por filtros manipulados. Perfis adicionais como técnico podem conceder ações próprias na mesma tela; a razão da permissão deve ser explícita no backend.

## Navegação por perfil

| Perspectiva | Itens principais |
| --- | --- |
| Colaborador | Início, Novo chamado, Meus chamados, Conhecimento, Notificações |
| Gestor de área | Itens de colaborador + Minha área |
| Técnico | Itens de colaborador + Visão operacional, Filas, Meus atendimentos, Gestão de artigos |
| Gestor de TI | Operação global, Visão estratégica, Conhecimento e Administração; mantém acesso aos próprios chamados |

Menus derivam das permissões, mas o servidor valida todos os endpoints e objetos. Links salvos passam pela mesma autorização.

## T04 — abertura

Campos propostos:

| Campo | Regra |
| --- | --- |
| Solicitante | Próprio usuário; gestor pode selecionar terceiro. Limitação à própria área é P04 |
| Aberto por | Identidade autenticada, imutável |
| Área/unidade | Cadastro do solicitante, leitura; não confiar no valor enviado pelo navegador |
| Tipo | Problema ou solicitação; classificação técnica precisa de validação P20 |
| Categoria | Seleção simples; “Não sei informar” depende de fila padrão P10 |
| Título e descrição | Obrigatórios, limites definidos no contrato de API |
| Local do atendimento | Unidade/local, separados do vínculo do solicitante |
| Impacto | Somente eu, toda a área, toda a unidade; no contexto delegado usar “somente o colaborador” |
| Anexos | Opcionais, limites e tipos ainda P13; validar antes da aceitação |

CTA “Enviar chamado”. Após sucesso, mostrar número e abrir T06. Falha preserva campos na sessão atual; não salvar descrição sensível no armazenamento permanente do navegador por padrão. Evitar duplo envio com comando idempotente e indicar envio em andamento. Se o anexo falhar, informar claramente se o chamado foi ou não registrado e permitir recuperação sem duplicar ticket.

Não há campo de ativo. Horário fora do expediente não impede a abertura na proposta; mostrar calendário sem prometer mobilização excepcional automática.

## T06 — detalhe

Cabeçalho: número, título, situação e ação principal. Conteúdo central: descrição, conversa e anexos. Painel complementar: solicitante/autor, área, unidade/local, categoria, grupo, responsável, impacto, severidade e SLA. Histórico em aba separada, com projeção pública que não exponha dados internos.

Na visualização técnica, separar “Responder ao solicitante” de “Nota interna” com rótulo e indicação persistente de audiência. Não depender apenas da cor. Texto de nota não pode ser enviado como resposta pública por troca acidental de aba.

| Ação | Solicitante | Gestor apenas da área | Técnico autorizado | Administrador |
| --- | --- | --- | --- | --- |
| Ler conteúdo público | Próprio | Área permitida | Escopo operacional proposto | Global |
| Responder | Próprio não terminal, proposta | Não em chamados alheios | Conforme atendimento | Sim, respeitando regras |
| Ler/gravar nota interna | Não | Não, proposta P16 | Sim, proposta P16 | Sim |
| Assumir | Não | Não | Fila autorizada | Administração global |
| Transferir/classificar | Não | Não | Proposta; P06/P10/P20 | Sim, com validação |
| Aguardar solicitante | Não | Não | Proposta: responsável | Sim |
| Resolver e encerrar | Não | Não | Técnico que resolve; solução obrigatória proposta | Sim |
| Reabrir | Próprio encerrado | Não em chamados alheios | Permissão adicional não definida | Poder administrativo; regra a detalhar |
| Cancelar | P19 | P19 | P19 | P19 |

As ações marcadas como proposta não são requisitos aprovados. Estados e transições devem coincidir com eventos-atendimento.md. Administrador também passa por integridade/transações; “administração total” não significa editar histórico silenciosamente.

### Estados e respostas de interface

- Aberto sem responsável: técnico vê “Assumir”. Conflito de assunção atualiza dados e informa que outro técnico assumiu.
- Em atendimento: responsável pode responder, solicitar informação ou resolver conforme permissões.
- Aguardando solicitante: colaborador vê a pergunta pendente; técnico vê SLA pausado e motivo. Retomada automática ainda P07.
- Encerrado: solução e histórico visíveis; solicitante pode reabrir. Exigência de motivo e janela temporal ainda propostas/P08.
- Cancelado: motivo, sem ação de reabertura até P19.
- SLA sem política: “Não configurado”; nunca apresentar como cumprido.
- Chamado sem autorização ou inexistente: resposta pública equivalente, sem título ou dados revelados; diagnóstico interno preservado.

## T10/T11 — operação

Tabela com número, título, idade, situação, severidade, categoria, unidade, grupo, responsável e prazo quando configurado. Filtros por grupo autorizado, status, severidade e unidade. Preservar filtros e posição ao voltar do detalhe.

“Sem responsável”, “Meus atendimentos” e “Aguardando solicitante” são atalhos para filtros. Ordenação inicial proposta por severidade e antiguidade, com nomes claros; não inventar prioridade operacional além da matriz configurada. Ações em massa ficam fora da primeira entrega para evitar alterações sem revisão individual.

## T15/T16 — dashboards

Usar catálogo em indicadores.md como contrato. Barra de filtros mostra escopo, intervalo e atualização. Diferenciar métricas de período de snapshots de backlog “em t”.

Composição proposta: faixa de poucos KPIs essenciais, evolução temporal, distribuição por dimensão e tabela de investigação. M01 criados, M02 tickets com encerramento e M03 encerramentos não são intercambiáveis. Cards têm nome e definição acessível.

Clique abre detalhamento com mesmo escopo, filtros e corte; indicadores por ciclo/evento devem abrir linhas nessa granularidade e informar contagem de chamados distintos. Retornar conserva filtros. Denominador zero exibe “Sem dados”. Oferecer tabela equivalente aos gráficos e não depender de tooltip para informação essencial.

Exportação só aparece quando implementada e autorizada; utilizar o mesmo corte dos dados e indicar geração em andamento. Atualização, retenção e disponibilidade dessa ação dependem de P18.

## Conhecimento

Busca de usuário não inclui títulos/trechos internos. Técnico publica sem aprovação. Prévia mostra audiência antes de publicar. Histórico de versões e arquivamento são propostas. Conversão de solução em artigo cria rascunho revisável, não publicação automática de dados de chamado.

Editor exige validação e sanitização de conteúdo na implementação. Conflito entre editores deve ser exibido sem sobrescrever silenciosamente. Anexos seguem autorização do artigo; um link em resposta não transforma artigo interno em público.

## Administração

Formulários mostram impacto antes de salvar/desativar: usuários e vínculos afetados, categoria sem grupo, calendário e vigência. Não permitir área operacional sem gestor ou categoria ativa sem encaminhamento válido, salvo fluxo de cadastro incompleto claramente separado.

Histórico é preservado nas mudanças. Tela de SLA cria versão e mostra vigência; regras de aplicação retroativa são P07. Administrador vê ausência de configuração, sem valores de SLA fictícios predefinidos como produção.

## Estados globais e acessibilidade

- Carregando: indicador contextual sem números inventados.
- Lista vazia: diferenciar nenhum registro de nenhum resultado do filtro.
- Falha de rede/servidor: tentativa novamente sem perda de edição nem duplicação de comando.
- Sessão expirada: solicitar autenticação, preservando somente navegação interna validada; não enviar destino externo arbitrário.
- Sem permissão: não mostrar dados que estavam em cache após troca de escopo/logout.
- Mudanças não salvas: confirmação antes de sair; autosave não assumido.
- Notificação aponta para recurso removido/restrito: mensagem genérica, sem conteúdo vazado.
- Formulários com rótulos, erros associados e foco no erro; diálogos com foco gerenciado.
- Teclado, foco visível, contraste legível, status com texto, tabelas com cabeçalhos e gráficos com alternativa tabular.
- No mobile, priorizar abertura, leitura e resposta; tabelas técnicas podem ter rolagem sinalizada sem perder ações essenciais.

## Jornadas e critérios de aceite

| Jornada | Telas | Evidência esperada na implementação |
| --- | --- | --- |
| J01 Abrir e acompanhar | T01 → T03 → T04 → T06 → T05 | Um ticket, grupo correto, protocolo e acesso exclusivo |
| J02 Assumir e resolver | T11 → T06 → T12 | Conflito concorrente tratado; resposta pública e solução preservadas |
| J03 Esperar e responder | T06 ↔ T07 | Pergunta visível, pausa registrada; retomada conforme política validada |
| J04 Reabrir | T05 → T06 | Mesmo número, novo ciclo, solução anterior preservada |
| J05 Consultar área | T15 → T06 | Leitura sem ações de edição; filtros/URLs não ampliam acesso |
| J06 Publicar conhecimento | T13 → T14 → T09 | Publicação direta pelo técnico; leitor vê só audiência autorizada |
| J07 Alterar política | T21/T22 → T23 | Versão, vigência e auditoria; resultado consistente com regra aprovada |
| J08 Investigar indicador | T16/T15 → detalhe → T06 | Mesma população e corte; reabertura não aumenta criados |

Estes critérios ainda não foram executados. Protótipo e validação com usuários serão as próximas evidências de UX.

## Prioridade de prototipação

1. Início do colaborador, abertura e detalhe (T03–T06).
2. Fila e ações do técnico (T11/T12/T06).
3. Dashboard da área e estratégico (T15/T16), com detalhe coerente.
4. Administração e conhecimento.

Para validar visualmente, usar dados sintéticos e sinalizar qualquer ação simulada. A implementação real do SSO e banco não pode ser inferida a partir de um protótipo.
