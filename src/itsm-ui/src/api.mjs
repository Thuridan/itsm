/** @param {typeof fetch} fetcher @param {AbortSignal | undefined} signal */
export async function getSystem(fetcher = fetch, signal = undefined) {
  const response = await fetcher('/api/v1/system', { signal, cache: 'no-store' });
  if (!response.ok) throw new Error('Não foi possível consultar o serviço.');
  const data = await response.json();
  if (data.stage !== 'identity' || typeof data.authenticationEnabled !== 'boolean' ||
      typeof data.persistenceEnabled !== 'boolean' || data.acceptingTickets !== false)
    throw new Error('O serviço retornou uma configuração incompatível com esta versão.');
  return { stage:'identity', authenticationEnabled: data.authenticationEnabled, persistenceEnabled:data.persistenceEnabled, acceptingTickets:false };
}
/** @param {string} path @param {RequestInit} options */
export async function api(path, options = {}) {
  const response = await fetch(path, {credentials:'same-origin',cache:'no-store', ...options});
  if (!response.ok) throw new Error(response.status === 403 ? 'Você não tem permissão para esta operação.' : response.status === 409 ? 'O cadastro foi alterado. Atualize a lista.' : 'Não foi possível concluir a operação. Verifique sua sessão e tente novamente.');
  return response.status === 204 ? null : response.json();
}
/** @param {string} path @param {unknown} payload */
export async function mutate(path, payload = undefined) {
  const {token} = await api('/api/v1/antiforgery');
  return api(path, {method:'POST',headers:{'Content-Type':'application/json','X-CSRF-TOKEN':token},body:payload === undefined ? undefined : JSON.stringify(payload)});
}
