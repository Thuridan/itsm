# Backlog inicial

Status inicial de todos os itens: **A detalhar**. Ordem proposta; sem estimativas ou datas comprometidas.

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
