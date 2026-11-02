/** @param {typeof fetch} fetcher @param {AbortSignal | undefined} signal */
export async function getSystem(fetcher = fetch, signal = undefined) {
  const response = await fetcher('/api/v1/system', { signal, cache: 'no-store' });
  if (!response.ok) throw new Error('Não foi possível consultar o serviço.');
  const data = await response.json();
  if (data.stage !== 'foundation' || data.authenticationEnabled !== false ||
      data.persistenceEnabled !== false || data.acceptingTickets !== false) {
    throw new Error('O serviço retornou uma configuração incompatível com esta versão.');
  }
  return { stage: 'foundation', authenticationEnabled: false, persistenceEnabled: false, acceptingTickets: false };
}
