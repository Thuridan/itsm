# Requisitos do ITSM

## Confirmados pelo responsável pelo produto

| ID | Requisito |
| --- | --- |
| R01 | Aplicação interna, acessível pela rede interna ou VPN, com SSO Microsoft 365. |
| R02 | Cada colaborador pertence a uma única unidade e área; uma área pode abranger unidades diferentes. |
| R03 | Cada área possui um único gestor. Vínculos organizacionais são administrados no sistema apenas por administradores. |
| R04 | Colaborador abre chamados somente para si. Gestor pode abrir para um colaborador. |
| R05 | Colaborador visualiza seus chamados; gestor de área consulta chamados da área em leitura e indicadores consolidados e individuais. |
| R06 | Gestor de TI possui visão e administração total e dashboards estratégicos e operacionais. |
| R07 | Grupos: Sistemas, Operações de TI e Audiovisual. Técnicos escolhem e assumem chamados, sem triagem obrigatória. |
| R08 | Categoria encaminha automaticamente ao grupo responsável; categorias genéricas iniciais serão refinadas pelo administrador. |
| R09 | Usuário informa impacto: somente ele, área inteira ou unidade inteira, para determinar severidade. Matriz exata ainda pendente. |
| R10 | Atendimento regular de segunda a sábado, 06h40–18h40. Feriados: atendimento excepcional. |
| R11 | Eventos e emergências podem exigir atendimento fora do expediente, envolvendo grupos necessários. Acionamento por telefone e WhatsApp; registro no portal. |
| R12 | Notificações no portal e por e-mail. Abertura exclusivamente pelo portal. |
| R13 | Prazos de SLA configuráveis. Espera por resposta do solicitante pausa o prazo de resolução. |
| R14 | Técnico pode resolver e encerrar diretamente; confirmação do solicitante não é necessária. Colaborador pode reabrir. |
| R15 | Todos os técnicos podem criar, editar e publicar artigos diretamente. Artigos para colaboradores e artigos internos à equipe de TI. |
| R16 | Interface moderna, limpa e simples, com indicadores úteis para a gestão. |
| R17 | Linux e Docker permitidos; homologação separada de produção; código no GitHub. |

## Propostas ainda não confirmadas

- Gestor abre chamados em nome de pessoas apenas da própria área.
- Estados: Aberto, Em atendimento, Aguardando solicitante, Encerrado e Cancelado.
- Uma assunção simultânea deve produzir apenas um responsável principal.
- Respostas públicas e notas internas, com controle de acesso também em anexos e buscas.
- Justificativa para transferências, cancelamentos, alterações de severidade e reabertura.
- Retomada automática do SLA quando o solicitante responde.
- Novo ciclo de SLA após reabertura, preservando resultados anteriores.
- Registro da área e unidade na abertura para relatórios históricos.
- Artigos com rascunho, publicação, arquivamento, versões e busca.
- Primeira resposta válida exige retorno público humano; mensagens automáticas não contam.

## Categorias iniciais propostas

Sistemas de negócio; computadores e periféricos; sistemas operacionais e aplicativos locais; contas e acessos; rede e conectividade; e-mail e colaboração; impressão e digitalização; audiovisual; infraestrutura e servidores; outros/não sei informar.

A associação de cada categoria ao grupo deverá ser validada. A fila padrão para categoria indefinida permanece pendente.

## Fases futuras

CMDB, inventário, importação de ativos, códigos de barras, reservas, empréstimos e agendamento de recursos ficam fora do MVP.

Requisitos já levantados para eventos: aprovação prévia, antecedência mínima de uma semana e designação da equipe somente pelo gestor. A autoridade de aprovação, exceções e implementação serão detalhadas na fase correspondente; não converter automaticamente uma semana em sete dias corridos sem validação.
