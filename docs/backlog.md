# Backlog inicial

Status: especificações iniciais elaboradas; fundação técnica iniciada em branch de desenvolvimento. Itens permanecem **A detalhar** até resolver as dependências relevantes. Ordem proposta; sem estimativas ou datas comprometidas.

| Item | Entrega | Critério de aceite inicial | Dependências |
| --- | --- | --- | --- |
| B01 | Fechar arquitetura | Stack, banco e isolamento dos ambientes documentados | P01, P02, P15 |
| B02 | Identidade e organização | Login e vínculos; colaborador não acessa dados alheios; gestor limitado à área; administrador global | B01, P03–P05, P09 |
| B03 | Cadastro de categorias e grupos | Administrador configura encaminhamento; técnico recebe fila do grupo | B02, P10 |
| B04 | Abertura e consulta | Usuário abre e acompanha; autoria distinta do beneficiário quando gestor abre | B03, P04, P13 |
| B05 | Atendimento | Assunção concorrente testada; histórico e transições autorizadas | B04, P16 |
| B06 | Encerramento e reabertura | Técnico encerra e colaborador reabre sem perder histórico | B05, P07, P08 |
| B07 | SLA | Políticas configuráveis; calendário, pausa e retomada verificados | B05, P06, P07, P11 |
| B08 | Notificações | Portal/e-mail; falha de entrega não perde o chamado; sem divulgação restrita | B05, P12 |
| B09 | Conhecimento | Técnicos publicam; usuários não acessam artigos internos por busca ou link direto | B02, P13 |
| B10 | Dashboards | Totais conciliam com chamados; filtros respeitam permissões; fórmulas documentadas | B06, B07 |
| B11 | Operação e piloto | Restauração testada, logs, validação em homologação e plano de implantação | B01–B10, P14, P17 |

## Verificações prioritárias

- Autorização no servidor: objetos, anexos, buscas, exportações e dashboards.
- Concorrência ao assumir chamados.
- Calendário e SLA: limite de expediente, sábado, domingo, feriados e pausas.
- Reabertura e preservação do histórico.
- Migrações em banco descartável e recuperação antes de produção.

Testes serão escolhidos conforme a implementação; esta lista não indica que já foram executados.

## Artefatos de planejamento disponíveis

- [Modelo lógico](modelo-dados.md): entidades, relações e invariantes propostas.
- [Eventos](eventos-atendimento.md): transições, atomicidade e cenários de aceite.
- [Indicadores](indicadores.md): fórmulas, coortes e reconciliação.

## Próxima entrega de planejamento

Mapa de telas e navegação elaborado em [telas-navegacao.md](telas-navegacao.md), com 23 telas e 8 jornadas. Protótipo visual Arbos validado. Fundação ASP.NET Core/React em desenvolvimento; detalhes em [fundacao.md](fundacao.md). Próxima etapa: identidade e provisionamento antes de habilitar o atendimento real. Decisões pendentes P03–P10, P16, P19 e P20 devem ser resolvidas antes de ativar os comportamentos correspondentes.

## Critérios adicionais

B07: verificar limites de expediente, pausa atravessando domingo e encerramento com pausa aberta.
B10: conferir exemplo manual do catálogo; reaberturas não aumentam criados; métricas sem denominador mostram sem dados; escopo de gestor se mantém em cache/exportação. Dependência adicional P18.

## Rastreabilidade de experiência

| Item | Telas / jornadas de referência |
| --- | --- |
| B02 | T01/T02/T17/T18; controle de escopo em J01/J05 |
| B03 | T19/T20 e encaminhamento de J01 |
| B04 | T03/T04/T05/T06; J01 |
| B05 | T10/T11/T12/T06; J02/J03 |
| B06 | T06; J04 |
| B07 | T21/T22/T06; J03/J07 |
| B08 | T07/T06; J03 |
| B09 | T08/T09/T13/T14; J06 |
| B10 | T15/T16; J05/J08 |

Critérios transversais: estados de erro/vazio/carregamento, teclado, tratamento de concorrência e expiração de sessão. Planejamento documentado não equivale a teste executado.
