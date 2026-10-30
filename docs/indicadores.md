# Catálogo inicial de indicadores

Estado: definições propostas para validação de negócio. Nenhum dashboard implementado. Métricas devem declarar unidade, coorte, filtros, versão da regra e instante de atualização.

## Convenções

Intervalo de relatório [a,b) no fuso configurado, convertido a UTC. Visão operacional usa estado atual em t. Visão histórica reconstrói estado e atributos no instante t. Filtros por área/unidade usam contexto de abertura como proposta; filtro por grupo atual e grupo no evento são opções distintas e rotuladas.

Aplicar autorização antes da agregação. O detalhamento de um cartão deve usar a mesma população e filtros. Gestor de área vê apenas o escopo permitido; uma agregação não autoriza revelar linhas externas à área.

## Métricas

| ID | Indicador | População e fórmula proposta |
| --- | --- | --- |
| M01 | Chamados criados | COUNT de tickets com CriadoUtc em [a,b); reabertura não é criação |
| M02 | Chamados com encerramento no período | COUNT DISTINCT TicketId dos eventos ChamadoEncerrado em [a,b); pode incluir ticket posteriormente reaberto |
| M03 | Encerramentos realizados | COUNT de eventos ChamadoEncerrado em [a,b); ciclos repetidos contam separadamente |
| M04 | Backlog em t | Tickets criados até t cujo último estado efetivo em t é Aberto, Em atendimento ou Aguardando solicitante |
| M05 | Sem responsável em t | Subconjunto de M04 com responsável nulo naquele instante |
| M06 | Idade do backlog | Duração corrida entre criação e t para M04; mediana, p90 e faixas. Mostrar idade do ciclo atual separadamente |
| M07 | Primeira resposta | Segundos úteis entre abertura inicial e primeira resposta pública humana da equipe; coorte de tickets criados em [a,b) com resposta observada até corte t; mostrar quantidade ainda sem resposta |
| M08 | Tempo de resolução por ciclo | Tempo útil líquido das pausas dos ciclos resolvidos com término em [a,b); média, mediana e p90; cancelamentos excluídos |
| M09 | Cumprimento SLA por tipo | Metas cumpridas no prazo / metas cumpridas avaliáveis com término em [a,b). Exibir numerador, denominador e exclusões; pendentes não entram como sucesso |
| M10 | SLA ativo violado em t | COUNT de metas ativas avaliáveis cujo consumo até t excede alvo; separar primeira resposta de resolução e mostrar tickets distintos |
| M11 | Reaberturas no período | COUNT de eventos ChamadoReaberto em [a,b); também apresentar tickets distintos |
| M12 | Taxa de reabertura por coorte | Para ciclos resolvidos em [a,b) com janela W completa até corte t: ciclos seguidos de reabertura em até W / ciclos elegíveis. W a definir, sem valor fixo aprovado |
| M13 | Demanda por dimensão | M01 agrupado por categoria, área ou unidade de abertura; sem duplicar tickets por joins com mensagens/ciclos |
| M14 | Carga atual por técnico | M04 agrupado pelo responsável atual, com severidade e idade; volume não equivale a produtividade |

M09 isolado pode ocultar atrasos ainda abertos; apresentar sempre com M10 e total pendente. Sem denominador, exibir “Sem dados”, não 0% nem 100%. Sem política configurada, exibir não avaliável e contagem separada.

## Reconciliação

Para escopo fixo sem filtros dimensionais que mudem: backlog final = backlog inicial + aberturas + reaberturas - encerramentos - cancelamentos durante o intervalo. Usar eventos, não M02 distinto. Para grupo/responsável, adicionar entradas e saídas por transferência. Alterações de autorização podem mudar o escopo visível e devem ser distinguidas de mudanças operacionais.

Um ticket encerrado duas vezes no mês tem M02 = 1 e M03 = 2. Uma reabertura não eleva M01. Um ticket aberto antes do período pode contribuir para M02/M03 e backlog sem entrar em M01.

## Modelo de leitura

- Projeção operacional por ticket: estado, grupo, responsável, tempos e metas atuais.
- Fato de ciclos: uma linha por ciclo, com início/fim, desfecho e tempos calculados sob regra versionada.
- Fato de eventos: uma linha por evento de negócio, para movimentação e reconstrução.
- Agregados diários opcionais por dimensões permitidas, gerados após necessidade medida; não somar contagens distintas de períodos para produzir total distinto.
- Percentis devem usar observações originais ou estrutura adequada; não fazer média de medianas/p90.
- Checkpoints, atualização idempotente e reconstrução de partições. Cada resposta informa data de corte e atualização.
- Somas por dimensões de grupo mutável exigem atribuição explícita (abertura, evento ou término); nunca misturar silenciosamente.

## Exemplo de conferência manual

Em um período que começa com backlog zero: A e B são abertos; A recebe resposta, encerra, reabre e encerra novamente; B permanece aguardando solicitante. Esperado: criados 2; tickets com encerramento 1; encerramentos 2; reaberturas 1; backlog final 1 (2 + 1 - 2). A pausa de B não remove B do backlog.

## Integrações e limites

Dashboard nativo React com endpoints analíticos autorizados. Power BI, construtor livre de relatórios, previsões e machine learning não são requisitos aprovados. Exportação utiliza o mesmo escopo de acesso e corte dos dados; arquivos gerados devem ter expiração/retenção definida.

## Pendências de produto

- P05: visibilidade após transferências organizacionais.
- P07: ciclos e política de SLA na reabertura e alterações de severidade.
- P18: retenção, janela W, metas de atualização e definições do catálogo.
- Regras de atribuição de resultado a técnico/grupo quando houver transferência.

Não avaliar desempenho individual apenas por contagem de chamados ou tempo bruto; complexidade e colaboração alteram a interpretação.
