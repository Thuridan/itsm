# Desenvolvimento

## Fluxo proposto

1. Detalhar uma necessidade em issue com objetivo, regras, permissões e critérios de aceite.
2. Implementar em branch curta: feat/, fix/ ou docs/.
3. Abrir pull request com resumo, evidências de validação e riscos relevantes.
4. Integrar após revisão e verificações aplicáveis.
5. Validar o artefato em homologação antes da promoção para produção.

Quadro proposto: A detalhar → Pronto para desenvolver → Em desenvolvimento → Em validação → Concluído. Não há GitHub Project configurado ainda.

## Pronto para desenvolver

Objetivo, critérios, dependências e permissões suficientemente definidos; propostas relevantes aprovadas explicitamente.

## Concluído

Implementação revisada, testes aplicáveis aprovados, documentação atualizada e comportamento validado em homologação. Alterações operacionais incluem migração e recuperação quando necessárias.

## Regras

- Não incluir segredos ou dados reais em código, exemplos, testes ou logs publicados.
- Respeitar requisitos confirmados; registrar propostas e dúvidas em docs/decisoes.md.
- Testar autorização no servidor, não apenas ocultar ações na interface.
- Documentar fórmulas dos indicadores e regras temporais.
- Não incluir funcionalidades de CMDB/reservas no MVP sem mudança explícita de escopo.

Proteção de main, verificações obrigatórias e políticas de implantação ainda dependem de configuração administrativa.
