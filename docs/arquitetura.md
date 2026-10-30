# Arquitetura e ambientes — proposta

## Direção

Aplicação em Linux/Docker, com SQL Server existente como candidato inicial. Proposta de monólito modular: identidade/organização, atendimento, SLA, notificações, conhecimento e indicadores. CMDB e reservas não serão implementados agora.

Linguagem, frameworks, bibliotecas, versões e dimensionamento ainda não foram escolhidos.

## Ambientes

| Ambiente | Finalidade | Isolamento proposto |
| --- | --- | --- |
| Desenvolvimento | Implementação e testes | Dados sintéticos e banco descartável |
| Homologação | Validação funcional e de migrações | Aplicação, banco, anexos e credenciais próprios |
| Produção | Atendimento real | Recursos e credenciais exclusivos; acesso controlado |

Preferir hosts separados para homologação e produção. Bancos diferentes na mesma instância não isolam falhas nem consumo de recursos. A topologia final depende da infraestrutura disponível.

GitHub armazena código/documentação/automações; não hospeda por si só os ambientes internos. A implantação dependerá de executor autorizado a acessar os destinos internos. Nenhum runner, GitHub Environment ou pipeline está configurado por esta documentação.

## Dados e integrações

- SSO corporativo autentica usuários; permissões e vínculos organizacionais pertencem à aplicação.
- Segredos externos ao Git; configurações exemplificadas apenas com valores fictícios.
- Credenciais distintas por ambiente e acesso de banco limitado às operações necessárias.
- Anexos com autorização independente de conhecer a URL; armazenamento e retenção a definir.
- Migrações de schema versionadas. Não executar migrações em produção automaticamente antes de definir recuperação e compatibilidade.

## Entrega proposta

Branch de tarefa → pull request → verificações → artefato versionado → homologação → promoção controlada do mesmo artefato para produção.

Branch não equivale a ambiente. Não é necessário criar uma branch permanente por ambiente.

## Robustez a especificar

Persistência confiável, tratamento de concorrência, repetição controlada de notificações, logs e métricas, verificação de saúde, backup e restauração. Metas de disponibilidade e recuperação permanecem pendentes; Docker sozinho não representa alta disponibilidade.
