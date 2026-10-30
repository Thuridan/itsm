# Arquitetura e ambientes — proposta

## Direção

Aplicação em Linux/Docker, com recomendação de reutilizar o SQL Server 2022 Standard existente. Proposta de monólito modular: identidade/organização, atendimento, SLA, notificações, conhecimento e indicadores. CMDB e reservas não serão implementados agora.

A stack de referência proposta está detalhada abaixo. Versões exatas de dependências e dimensionamento serão definidos na implementação.

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
 
## Banco identificado

SQL Server 2022 Standard Edition (64-bit), conforme resultado de consulta fornecido pelo responsável pelo produto. Nenhuma conexão ao servidor foi realizada por este projeto.

A edição não impõe uma limitação funcional identificada para o MVP descrito. Desempenho depende de modelagem, consultas, índices, simultaneidade, armazenamento e recursos livres na instância. Não pressupor capacidade a partir do total de colaboradores.

Referência: https://learn.microsoft.com/en-us/sql/sql-server/editions-and-components-of-sql-server-2022

Próxima verificação operacional: CPU, RAM, armazenamento, outras cargas e recuperação. O planejamento da aplicação pode avançar enquanto esses dados são levantados.

## Stack de referência proposta

Proposta técnica para implementação, sem atribuir aprovação explícita do produto a escolhas ainda não revisadas.

| Camada | Escolha proposta | Motivo |
| --- | --- | --- |
| Backend | C# / ASP.NET Core 10 LTS | Regras de negócio, APIs, autenticação e tarefas de fundo no mesmo ecossistema |
| Persistência | Entity Framework Core 10, provider SQL Server | Migrações versionadas e suporte ao banco existente |
| Interface | React com TypeScript e Vite | Componentes reutilizáveis, formulários, tabelas e dashboards |
| Identidade | Microsoft Entra ID, OpenID Connect e Microsoft.Identity.Web | SSO corporativo; autorização organizacional permanece na aplicação |
| Hospedagem | Container Linux; Docker Compose por ambiente inicialmente | Implantação reproduzível na infraestrutura disponível |
| Dados | SQL Server 2022 Standard | Reutilização da plataforma existente |

.NET 10 tem suporte LTS até novembro de 2028. Fixar SDK, imagens e versões exatas de pacotes no início da implementação e atualizar por PR. React, Vite e demais dependências também terão versões fixadas e lockfile; não usar tags latest em produção.

### Componentes e limites

- Uma aplicação ASP.NET Core modular, com frontend compilado servido na mesma origem e API versionada em /api/v1.
- Organização: usuários, áreas, unidades, vínculos e políticas de acesso.
- Atendimento: chamados, categorias, grupos, atribuição, mensagens, anexos e histórico.
- SLA: calendário, políticas, pausas e ciclos de atendimento.
- Conhecimento: artigos, visibilidade, versões e busca.
- Notificações: caixa de saída persistente no SQL Server e processamento em segundo plano com tentativas limitadas e registro de falhas.
- Indicadores: consultas autorizadas e paginadas; definições de métricas documentadas.
- Auditoria: ator, horário, operação e alterações relevantes; não confundir com logs de diagnóstico.

O monólito modular mantém responsabilidades separadas com uma implantação principal. CMDB e reservas poderão ser incorporadas futuramente por contratos internos e APIs, sem implementação antecipada desses módulos.

### Autenticação e autorização

Proposta de sessão web no servidor: login OpenID Connect, cookie Secure/HttpOnly, proteção contra CSRF em mutações e tokens fora do armazenamento JavaScript do navegador. Persistir e proteger as chaves de Data Protection entre reinícios. Configurar tenant e callbacks por ambiente. Provisionamento inicial e concessão do primeiro administrador precisam de desenho explícito; não promover automaticamente o primeiro usuário.

O navegador e os componentes de autenticação precisam alcançar os endpoints necessários do provedor de identidade. O acesso interno ao portal não significa operação offline do SSO.

### Persistência e operação

- Transações e controle de concorrência na assunção e alterações de chamados.
- Gravar eventos de notificação junto à alteração de negócio na mesma transação; envio com repetição controlada e deduplicação quando possível, sem promessa de entrega exatamente uma vez.
- Anexos fora da imagem do container, com metadados no banco e download autorizado. Backup consistente de banco e arquivos.
- Segredos por ambiente; nenhuma informação secreta no bundle do frontend.
- Pipeline de PR: build, análise estática e testes relevantes. Integração com SQL Server descartável, nunca com produção.
- Implantação interna depende de runner autorizado e configuração de rede. Não expor acesso de produção a workflows de PR não confiáveis.

### Estrutura de código planejada

| Caminho | Responsabilidade |
| --- | --- |
| src/Itsm.Web/ | Host ASP.NET Core, endpoints, sessão e composição |
| src/Itsm.Core/ | Módulos, regras e casos de uso organizados por funcionalidade |
| src/Itsm.Infrastructure/ | EF Core, armazenamento, e-mail e integrações |
| src/itsm-ui/ | Interface React/TypeScript |
| tests/ | Testes de regras, integração e fluxos de usuário |
| deploy/ | Docker e modelos de configuração sem segredos |

Esses diretórios ainda não foram criados: esta etapa especifica a arquitetura.

### Primeira entrega técnica proposta

1. Criar solução, frontend, lockfiles e imagens reproduzíveis.
2. Estabelecer build/testes em CI e configuração separada por ambiente.
3. Implementar organização, identidade e autorização com testes de isolamento.
4. Entregar um fluxo vertical: abrir → encaminhar → assumir → responder → encerrar → reabrir.
5. Acrescentar SLA, notificações, conhecimento e indicadores conforme backlog.

### Fontes oficiais consultadas

- https://dotnet.microsoft.com/en-us/platform/support/policy
- https://learn.microsoft.com/en-us/ef/core/providers/sql-server/
- https://learn.microsoft.com/en-us/aspnet/core/security/authentication/configure-oidc-web-authentication
- https://react.dev/learn/typescript
- https://vite.dev/guide/build
