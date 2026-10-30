# Protótipo navegável do ITSM

Protótipo estático independente, para validar UX. Não altera a stack de produção selecionada (ASP.NET Core / React / SQL Server). HTML/CSS/JavaScript sem dependências externas, com dados fictícios em memória.

## Abrir

Baixe este diretório e abra index.html no navegador. Não precisa instalar pacotes nem configurar banco. Recarregar a página restaura os dados. O GitHub exibe código, não executa a página automaticamente. Nenhuma publicação pública em GitHub Pages foi configurada.

## Telas

Início do colaborador, abertura, detalhe, fila técnica e dashboard gerencial. Inclui listagem de chamados próprios e orientações ilustrativas. Controle “Simular perfil” permite explorar colaborador, técnico, gestor da área Financeiro e gestor de TI.

## Roteiro de revisão

1. Como colaborador, abra novo chamado de Sistemas de negócio.
2. Troque para técnico, selecione o chamado na fila e assuma.
3. Escreva uma pergunta e selecione Solicitar informação.
4. Troque para colaborador, abra Meus chamados e responda.
5. Volte ao técnico, abra a fila, localize o chamado e use Resolver e encerrar; informe a solução.
6. Como colaborador, reabra o mesmo chamado com motivo.
7. Como gestor de TI, observe backlog, reaberturas e encerramentos; filtre grupo/área e investigue a lista.
8. Compare com o gestor de área: somente Financeiro é mostrado na simulação.

O técnico fictício Rafael participa de Sistemas e Operações de TI; Audiovisual pode ser explorado pelo gestor de TI. O gestor de área possui apenas leitura de chamados de terceiros.

## Hipóteses de demonstração

Matriz impacto/severidade, retomada após resposta, motivo de reabertura, escopo por grupo e interface de resolução são hipóteses de UX. Não aprovam regras pendentes. A abertura delegada pelo gestor ainda não é simulada.

## Limites

- Não há SSO, backend, autorização real, envio de e-mail, anexos, persistência, auditoria segura ou relógio SLA.
- Perfis e filtros no navegador não constituem segurança. Todo o dataset fictício está no arquivo.
- SLA aparece como Não configurado. Não há gráficos históricos ou percentuais inventados.
- Dashboards refletem o estado em memória e contadores de ciclos da demonstração, sem filtro de datas.
- Dados não são mantidos após recarregar.
- Ainda não há alerta geral de abandono de formulário; troca de tipo de mensagem confirma descarte.
- Gestão completa de conhecimento/administração é uma próxima etapa.
- Encerramento/reabertura usam diálogos nativos temporários; interfaces finais serão refinadas.

## Verificação

Execute node check.cjs. Testes sem dependências verificam renderização lógica, filtros por perfil, assunção repetida, visibilidade simulada de nota interna, espera/retomada, encerramento/reabertura, criação, escape HTML e reconciliação do backlog.

Os testes passaram no ambiente de elaboração. Não são testes de DOM real, concorrência de banco ou autorização do backend. QA visual e interação em navegador não verificados: o serviço de prévia do ambiente estava indisponível. Responsividade e acessibilidade ainda requerem revisão no navegador.
