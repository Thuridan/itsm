# Decisões e pendências

## Decisões confirmadas

- D01: entregar ITSM antes de CMDB, inventário e agendamentos.
- D02: abertura exclusiva no portal; avisos no portal e e-mail.
- D03: categorias encaminham aos grupos; técnicos assumem chamados.
- D04: cadastro organizacional local; autenticação Microsoft 365.
- D05: SLA configurável; pausa por espera do solicitante.
- D06: encerramento pelo técnico e reabertura pelo colaborador.
- D07: publicação de conhecimento pelos técnicos, sem aprovação.
- D08: Linux/Docker disponíveis e homologação separada.
- D09: por delegação do usuário, seleção técnica de C#/ASP.NET Core 10 + EF Core 10, React/TypeScript e SQL Server como referência. Dashboards nativos e camada analítica com consultas e agregações autorizadas; Power BI e Python analítico não são dependências do MVP.

## Proposta de banco

Recomendação: reutilizar SQL Server 2022 Standard Edition (64-bit). Edição e versão foram informadas pelo responsável pelo produto a partir da consulta da instância: ProductVersion 16.0.4305.5, ProductLevel RTM, ProductUpdateLevel CU27, referência KB5126934. Esses valores são evidência fornecida pelo usuário, não auditoria independente do servidor ou confirmação de patch mais recente.

A escolha é tecnicamente adequada ao escopo funcional planejado; capacidade disponível, carga, conectividade, backup e recuperação ainda precisam de validação antes da implantação.

## Pendências que influenciam implementação

| ID | Decisão necessária |
| --- | --- |
| P01 | Parcialmente resolvida: SQL Server 2022 Standard identificado. Pendentes CPU/RAM, armazenamento, carga compartilhada, capacidade disponível e backup/restauração. |
| P02 | Stack selecionada (D09). Pendentes versões exatas, prova de integração e dimensionamento; justificativa e estratégia de BI em arquitetura.md. |
| P03 | Um gestor pode responder por várias áreas? |
| P04 | Abertura pelo gestor é limitada à sua área? |
| P05 | Acesso histórico após transferência de colaborador/gestor entre áreas. |
| P06 | Matriz impacto/severidade, permissões de ajuste e tratamento de impacto entre unidades. |
| P07 | SLA após reabertura, retomada após resposta e efeito das alterações de política nos chamados existentes. |
| P08 | Eventual limite temporal de reabertura; nenhum limite foi aprovado. |
| P09 | Cadastro/provisionamento no primeiro login e bloqueio de usuários sem vínculo organizacional. |
| P10 | Categoria indefinida, transferências e técnico indisponível na reabertura. |
| P11 | SLA excepcional, feriados por unidade, fuso horário e calendários. |
| P12 | Serviço de envio de e-mail, identidade remetente e credenciais por ambiente. |
| P13 | Limites, retenção, armazenamento e proteção dos anexos. |
| P14 | Metas de disponibilidade, perda máxima de dados e tempo de recuperação. |
| P15 | Infraestrutura dos ambientes, executor de implantação e estratégia de acesso. |
| P16 | Visibilidade de notas internas e permissões de edição/interação dos gestores. |
| P17 | Dados atuais de chamados a migrar ou manter para consulta. |
| P18 | Validar catálogo de métricas: janela de reabertura, atualização, retenção e atribuição de resultados após transferências. |
| P19 | Permissões de cancelamento, comportamento após cancelamento e edição/interação em chamados encerrados. |
| P20 | Escopo técnico entre grupos, ações de responsável versus outros técnicos, classificação problema/solicitação e detalhes de abertura/local. |

Cada decisão futura deve registrar contexto, escolha, motivo, consequências e validação. Não considerar sugestões anteriores como aprovação tácita.

## Modelagem elaborada

Modelo lógico em modelo-dados.md; eventos em eventos-atendimento.md; métricas em indicadores.md. São especificações para revisão, sem migrações executadas. Ciclo de atendimento não decide automaticamente a política de SLA após reabertura. A preservação do contexto histórico não resolve por si só o acesso de gestores após transferências.

## Experiência especificada

Mapa de telas, rotas propostas, ações por papel/estado e jornadas em telas-navegacao.md. Não é interface implementada. Navegação não amplia autorização. Detalhamento de métricas deve manter população, granularidade, corte e escopo do indicador.

## Identidade visual validada

O usuário validou o protótipo Arbos: logotipo original, verdes #00563A e #00A245, superfícies brancas e navegação institucional. Esta validação é visual; não aprova implicitamente políticas de negócio pendentes.

## Fundação em implementação

A primeira entrega técnica está em docs/fundacao.md. Implementa shell React e endpoints de estado/saúde. SSO, autorização de negócio, persistência, migrações e atendimento real permanecem pendentes. A infraestrutura interna não foi acessada ou alterada.
