# Arquitetura e ambientes

Este documento é um modelo de planejamento. Não há aplicação ou ambientes provisionados.

## Decisões a registrar

- Stack e versões suportadas.
- Componentes e responsabilidades.
- Persistência e migrações.
- Integrações e autenticação.
- Observabilidade e recuperação.

## Separação de ambientes

Desenvolvimento, homologação e produção devem ter configurações e credenciais próprias. Dados de testes devem ser sintéticos. Os detalhes de hosts e redes não pertencem à documentação pública.

## Entrega proposta

Branch de tarefa → pull request → verificações → artefato versionado → homologação → promoção controlada para produção.

GitHub mantém código e automações; ambientes em execução exigem infraestrutura própria. Branches não substituem isolamento de ambientes.

Pipelines, runners, proteção de branches e ambientes do GitHub ainda não foram configurados.
