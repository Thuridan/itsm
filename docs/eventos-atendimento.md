# Eventos e transições de atendimento

Estado: contrato lógico proposto, ainda não implementado. Encerramento pelo técnico, reabertura pelo colaborador e pausa por espera do solicitante são confirmados. Estados exatos, retomada automática e política de SLA na reabertura permanecem propostas.

## Envelope

EventId, TicketId, Sequence, EventType, SchemaVersion, CycleId opcional, ActorId (ou ator sistema), OccurredAtUtc, RecordedAtUtc, CorrelationId e payload mínimo com IDs/valores alterados. Evento não deve conter tokens, credenciais ou cópias desnecessárias de mensagens/anexos.

## Catálogo e efeitos

| Evento | Condição / transição proposta | Dados específicos e efeito |
| --- | --- | --- |
| ChamadoAberto | Usuário autorizado; estado Aberto | Solicitante, autor, contexto organizacional, categoria/grupo, impacto, severidade e políticas; cria ciclo inicial |
| ChamadoAssumido | Aberto e sem responsável → Em atendimento | Técnico, grupo, versão esperada; uma operação concorrente vence |
| ChamadoTransferido | Não terminal; técnico autorizado | Grupo/responsável anterior e novo, motivo; não reinicia SLA automaticamente |
| ClassificacaoAlterada | Não terminal; autorizado | Categoria, impacto e severidade antes/depois; efeitos de SLA dependem de P07 |
| RespostaPublicaRegistrada | Em atendimento ou aguardando, conforme ator | ID da mensagem e papel do autor; pode cumprir primeira resposta, conforme definição |
| NotaInternaRegistrada | Técnico autorizado | ID da mensagem; nunca cumpre primeira resposta pública |
| EsperaSolicitanteIniciada | Em atendimento → Aguardando solicitante | ID de pergunta pública, motivo, início; pausa meta de resolução |
| EsperaSolicitanteFinalizada | Aguardando → Em atendimento | Resposta ou ação autorizada, fim da pausa; retomada automática depende de P07 |
| ChamadoEncerrado | Em atendimento ou aguardando → Encerrado | Solução, técnico, fim do ciclo; fecha eventual pausa no mesmo instante |
| ChamadoReaberto | Encerrado → Em atendimento ou Aberto | Solicitante, motivo proposto, novo ciclo; atribuição se técnico indisponível depende de P10 |
| ChamadoCancelado | Não terminal → Cancelado | Autor e motivo; finaliza ciclo como cancelado, sem registrar resolução |
| SLAViolado | Meta ativa cruza limite configurado | Meta, instante efetivo e instante de detecção; sem evento repetido por polling |

Cancelado não reabre neste desenho até decisão explícita. Permissão de cancelamento e eventual registro de resposta em estado encerrado são pendências P19. Chamados encerrados podem ser consultados, sem edições silenciosas no histórico.

## Consistência transacional

Na mesma transação: verificar autorização e versão; alterar estado; atualizar ciclo/metas/pausas; inserir evento; inserir trabalhos de notificação. Se falhar, nenhuma parte é confirmada. Notificações externas são posteriores ao commit.

Registro de estado atual mais eventos complementares: não é exigido implementar event sourcing completo. Reprocessar eventos reconstrói projeções analíticas, não reenvia automaticamente e-mails nem reexecuta comandos de atendimento.

Consumidores mantêm EventId processado e checkpoint. Uma reentrega não incrementa métricas duas vezes. Sequência por chamado resolve timestamps iguais. Em importações futuras, distinguir momento ocorrido de momento registrado e recomputar partições afetadas.

## Cálculo temporal proposto

Tempo consumido de resolução até t = duração da interseção entre [início, t) e janelas úteis do calendário aplicado, menos a interseção dessas janelas com a união das pausas válidas. Aplicar o corte de término quando a meta for cumprida/cancelada. Nunca subtrair duas vezes pausas sobrepostas.

Usar precisão consistente (segundos). Cumprida no limite é pontual; consumido maior que alvo é violação. Prazo de primeira resposta não herda pausa de resolução. Notificação automática e nota interna não satisfazem a primeira resposta humana proposta.

## Cenários de aceite a implementar

1. Dois técnicos assumem a mesma versão: um sucesso, um conflito, um evento.
2. Mensagem do solicitante não satisfaz primeira resposta técnica.
3. Pausa sobre domingo remove somente a parcela útil; fim de semana não é descontado duas vezes.
4. Mudança de mês durante pausa não perde continuidade.
5. Encerramento em espera fecha pausa e ciclo atomicamente.
6. Encerrar e reabrir preserva solução anterior, sem novo ticket.
7. Mudar categoria não reescreve grupo histórico dos eventos.
8. Falha de e-mail preserva chamado e trabalho pendente; reentrega não duplica indicador.
9. Mudança de área exige reavaliação de acesso; payload/caches não expõem dados fora do escopo.
10. Meta sem política aparece como não configurada, não como SLA cumprido.

São critérios de testes futuros, não resultados de testes executados.
