# ITSM

Plataforma interna de gerenciamento de serviços. **Fase atual: fundação da aplicação em desenvolvimento.** Frontend React e host ASP.NET Core iniciais; Login Entra e provisionamento SQL preparados para homologação; integração com o ambiente do colégio e atendimento real ainda não habilitados. Protótipo preservado como referência visual.

## Protótipo

Veja [instruções e limites do protótipo navegável](prototype/README.md). Baixe prototype/index.html e abra no navegador. Dados fictícios em memória, sem SSO, banco ou e-mail real.

## Aplicação em desenvolvimento

[Como executar, testar e conhecer os limites da fundação](docs/fundacao.md).

[Configurar identidade e provisionamento](docs/identidade.md). Nenhum tenant/banco real foi acessado nesta entrega.

## Documentação

- [Escopo e requisitos](docs/requisitos.md)
- [Decisões e pendências](docs/decisoes.md)
- [Backlog inicial](docs/backlog.md)
- [Arquitetura e ambientes](docs/arquitetura.md)
- [Modelo de dados](docs/modelo-dados.md)
- [Eventos e transições](docs/eventos-atendimento.md)
- [Catálogo de indicadores](docs/indicadores.md)
- [Telas e navegação por perfil](docs/telas-navegacao.md)
- [Fluxo de desenvolvimento](CONTRIBUTING.md)

## Etapas

1. ITSM: portal, chamados, grupos, permissões, SLA, notificações, conhecimento e dashboards.
2. CMDB e inventário, integrados ao ITSM.
3. Reservas, empréstimos e agendamentos.

Requisitos confirmados e propostas estão separados na documentação. A stack foi selecionada por delegação técnica; estimativas e regras de negócio pendentes não devem ser tratadas como aprovadas.

## Estado do repositório

Esta estrutura contém documentação e modelos de colaboração. Não configura GitHub Environments, proteção de branches, runners, bancos, SSO ou pipelines de implantação. Esses recursos serão preparados após as decisões de arquitetura e acesso.

Não publicar credenciais, dados pessoais, endereços internos, dumps ou arquivos de configuração reais.
