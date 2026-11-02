# Fundação da aplicação — primeira entrega

## O que está implementado

- Host ASP.NET Core 10 com endpoints HTTP reais para estado da plataforma e saúde.
- Frontend React/TypeScript compilado por Vite, com logo original e paleta Arbos aprovada.
- Consulta real do frontend a /api/v1/system, incluindo erro, cancelamento e nova tentativa.
- Shell institucional sem troca fictícia de perfil e sem dados pessoais ou credenciais.
- Build integrado em Docker multi-stage, executado com usuário não root.
- Modelos separados de configuração para homologação/produção; não provisionam servidores.
- CI de build, testes, smoke HTTP e construção de imagem, sem implantação automática.

O protótipo completo permanece em prototype/ como referência de UX. A aplicação real ainda implementa apenas a entrada institucional e a consulta ao serviço; as cinco telas do protótipo serão portadas em entregas incrementais.

## Limites deliberados

SSO, autorização de negócio, EF Core/SQL Server, abertura de chamados, dashboards e migrações ainda não estão implementados. Nenhum endpoint de negócio é exposto. /health/live confirma somente o processo HTTP; /health/ready retorna 503 até a integração operacional ser entregue. Não configurar produção real com esta versão.

A shell e /api/v1/system são públicos e retornam apenas capacidades desabilitadas, sem configurações privadas. Rotas /api/* desconhecidas retornam 404 e não o HTML da SPA. Health de prontidão não é liveness: um 503 esperado não deve causar reinício automático do processo.

## Executar para desenvolvimento

Pré-requisitos: SDK .NET indicado em global.json, Node 24 e npm.

Terminal 1, na raiz:

    dotnet run --project src/Itsm.Web --urls http://127.0.0.1:5080

Terminal 2:

    cd src/itsm-ui
    npm ci
    npm run dev

Abrir o endereço informado pelo Vite. Ele encaminha /api e /health ao backend local. Nenhuma conexão ao SQL Server é necessária nesta entrega.

## Build e testes

    cd src/itsm-ui
    npm ci
    npm test
    npm run build
    cd ../..
    mkdir -p src/Itsm.Web/wwwroot
    cp -R src/itsm-ui/dist/. src/Itsm.Web/wwwroot/
    dotnet publish src/Itsm.Web/Itsm.Web.csproj -c Release -o artifacts/web
    python3 tests/smoke_backend.py

O teste HTTP inicializa o host publicado, verifica endpoints, erro de API, resposta da interface e cabeçalhos, e encerra o processo ao terminar. Exige porta local 5197 disponível. Não testa SSO ou banco.

## Docker e separação de ambientes

    docker compose -p itsm-hml --env-file deploy/homologacao.env.example -f deploy/compose.yaml up --build -d

Acesso local pela porta 8081. O arquivo de produção é apenas um modelo para etapa futura. Projetos Compose e portas distintas evitam colisão local; isolamento entre hosts, contas, dados e recursos deve ser definido na implantação real.

A porta é vinculada a loopback por padrão. TLS, DNS, proxy e acesso da rede interna/VPN são pendências operacionais; não modificar exposição de rede por conveniência. Nenhum banco ou runner interno foi criado.

SDK e pacotes npm estão fixados; npm possui lockfile. As imagens Node 24 e runtime .NET 10 seguem tags de linha para receber patches: antes de produção, fixar também digests revisados e instituir atualização controlada. Não afirmar build integralmente reproduzível enquanto essas tags forem móveis.

## CI e estado de validação

Workflow .github/workflows/foundation.yml verifica PRs/pushes sem segredos ou acesso à rede interna. Jobs não implantam nem aplicam migrações.

As evidências efetivamente executadas e limitações do ambiente serão registradas no pull request. Workflow criado não equivale a execução bem-sucedida. QA visual em navegador continua pendente enquanto a prévia do ambiente estiver indisponível.

## Próxima entrega

Identidade Microsoft Entra e provisionamento organizacional com autorização no servidor. Antes de habilitar usuários reais: definir tenant/callbacks, primeiro administrador e tratamento de usuários sem área/unidade. Só então avançar para entidades persistidas e fluxo real de chamados.

## Evidências locais desta entrega

- npm test: quatro testes passaram.
- npm run build: TypeScript e Vite compilaram com sucesso.
- TypeScript 6.0.3 fixado: a versão 7 nativa não executou neste ambiente por ausência de /proc/self/exe.
- SDK .NET 10.0.403 instalado no workspace, mas CoreCLR não inicia (HRESULT 0x8007000E). Compilação e smoke HTTP do backend não executados localmente.
- Docker indisponível localmente; imagem não validada neste ambiente.
- CI configurada para realizar as verificações restantes. Manter PR em rascunho até evidência de sucesso.
