# Identidade Microsoft e provisionamento inicial

Status: implementação para homologação; não conectada ao tenant ou banco do colégio. Atendimento permanece desabilitado.

## Fluxo

1. Login OpenID Connect com authorization code e PKCE, no tenant único configurado.
2. Validação padrão de assinatura/issuer/audience, mais conferência explícita de tid e oid.
3. Identidade local única por tenant + object ID. E-mail e nome não concedem permissões.
4. Primeiro acesso cria usuário Pending e auditoria, sem área/unidade/privilégios.
5. Administrador explicitamente provisionado seleciona área e unidade existentes e ativa o colaborador pela interface.
6. Cada operação administrativa consulta o estado atual do banco. Desativação ou remoção de vínculo bloqueia o acesso mesmo com cookie válido.

Ativação nesta entrega concede somente acesso de colaborador. Gestão de técnicos, gestores de área, grupos e histórico organizacional completo será uma próxima entrega. As tabelas de área/unidade são referências mínimas iniciais, não implementam ainda a regra de um gestor por área.

## Configuração

Identity:Enabled é false por padrão. Quando true, a inicialização exige TenantId e ClientId GUIDs, ClientSecret, PublicOrigin HTTPS, caminho absoluto persistente de chaves e ConnectionStrings:Identity.

- Registrar um aplicativo Web single-tenant no Microsoft Entra ID, separado por ambiente.
- Callback exato: https://HOST-DO-AMBIENTE/signin-oidc. Não precisa expor o portal na internet; o navegador deve alcançar o callback na rede interna/VPN.
- PublicOrigin contém apenas a origem HTTPS, sem path/query. A aplicação fixa esse callback e o retorno interno '/', não aceita returnUrl arbitrário.
- Backend precisa alcançar os endpoints Microsoft de descoberta, chaves e troca de código.
- Escopos openid/profile; sem acesso ao Microsoft Graph ou permissão de leitura de diretório nesta entrega.
- Cliente recebe somente cookie HttpOnly/Secure; tokens não são persistidos no navegador nem salvos no cookie da aplicação.
- Sessão local com oito horas sem renovação deslizante, valor técnico inicial a validar. Desativação no ITSM é reavaliada por consulta; desativação exclusivamente no Entra pode depender do término/revalidação da sessão local.

Esta implementação usa o handler OpenID Connect oficial do ASP.NET Core diretamente; não inclui Microsoft.Identity.Web, pois não chama Graph nem mantém cache de tokens. Esta é uma concretização técnica da autenticação, não mudança do provedor escolhido.

## Banco e administrador inicial

Criar banco exclusivo do ITSM e aplicar deploy/sql/001_identity.sql explicitamente com conta de implantação. Não há EnsureCreated/Migrate no startup da aplicação. Script baseline não deve ser reaplicado. Mudanças seguintes precisam de scripts/migrações versionados.

Em cópia local do script 002, informar tenant, object ID do administrador autorizado, IDs de unidade/área e nomes. O script bloqueia GUIDs vazios e nova execução quando já há administrador no tenant. Nenhuma pessoa ganha administração por ser o primeiro login, ter e-mail específico ou claim roles.

Unidades/áreas adicionais devem ser cadastradas por operação administrativa no banco enquanto a tela completa de organização não for entregue. Não inserir dados reais no Git. Credencial runtime deve ter somente SELECT/INSERT/UPDATE nas tabelas necessárias, sem CREATE/ALTER/DROP ou administração do SQL Server. Auditoria deve permitir INSERT/SELECT, não UPDATE/DELETE ao runtime.

## TLS, cookies e chaves

HTTPS é obrigatório para login e cookies. Configurar TLS no ingress interno/Kestrel; o callback usa PublicOrigin explícito. Não habilitar confiança irrestrita em cabeçalhos forwarded. Proxies/callbacks reais precisam de validação no ambiente.

Criar diretório persistente de chaves com permissão apenas ao usuário do container. Separar o volume por ambiente e proteger/backupear as chaves. PersistKeysToFileSystem não cifra as chaves em repouso por si só: prover proteção do volume e avaliar certificado/KMS antes de produção. A chave perdida invalida sessões; comprometida pode permitir forjá-las.

O override compose.identity.yaml lê deploy/.identity.env (ignorado no Git) e monta um volume; preparar proprietário/permissões para APP_UID do container antes de ativar. Configuração padrão de loopback HTTP não é suficiente para SSO. Não publicar porta adicional para contornar isso.

Logout é POST protegido por antiforgery e remove a sessão local. Não encerra a sessão Microsoft no navegador. API retorna 401/403, nunca HTML de login em resposta a autorização negada. Não há login de teste ou impersonação na aplicação.

## Endpoints

- GET /auth/login: challenge Microsoft, ou 503 quando desabilitado.
- GET /api/v1/me: estado da sessão e do cadastro local; anônimo recebe authenticated=false.
- GET /api/v1/antiforgery: token para sessão autenticada, sem cache.
- POST /auth/logout: cookie e X-CSRF-TOKEN válidos.
- GET /api/v1/admin/users?page=1: lista paginada por tenant, somente administrador ativo.
- GET /api/v1/admin/directory: referências organizacionais ativas, somente administrador.
- POST /api/v1/admin/users/{id}/activate: unidade, área, versão rowversion; somente Pending, com proteção contra atualização concorrente e auditoria atômica.

Readiness geral continua 503 porque atendimento ainda não foi entregue. persistenceEnabled indica recurso configurado, não saúde do SQL Server.

## Validação e pendências

CI verifica compilação, testes de contrato, host HTTP sem identidade habilitada e identidade/provisionamento em banco SQL Server descartável. Principals sintéticos existem somente no executável de testes. Login real, MFA/Conditional Access, TLS, cookies no navegador e retorno Entra precisam de teste em homologação.

A estratégia Pending é a proposta de implementação de P09 para revisão; não habilitar produção antes de validar essa regra e o administrador inicial com o responsável pelo produto. Retenção de cadastro/auditoria e duração de sessão também precisam ser definidas.
