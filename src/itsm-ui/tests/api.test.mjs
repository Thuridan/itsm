import test from 'node:test';
import assert from 'node:assert/strict';
import { getSystem } from '../src/api.mjs';
const body = { stage:'identity', authenticationEnabled:false, persistenceEnabled:false, acceptingTickets:false };
test('consults same-origin API without caching', async () => {
  const result = await getSystem(async (url, options) => {
    assert.equal(url, '/api/v1/system'); assert.equal(options.cache, 'no-store');
    return new Response(JSON.stringify(body));
  });
  assert.deepEqual(result, body);
});
test('does not report backend errors as connected', async () => {
  await assert.rejects(getSystem(async () => new Response('', {status:503})));
});
test('rejects incompatible or incomplete response', async () => {
  for (const data of [{}, {...body, acceptingTickets:true}, { ...body, authenticationEnabled:'yes' }])
    await assert.rejects(getSystem(async () => new Response(JSON.stringify(data))));
});
test('propagates cancellation', async () => {
  const controller = new AbortController(); controller.abort();
  await assert.rejects(getSystem(async (_, {signal}) => { signal.throwIfAborted(); }, controller.signal), {name:'AbortError'});
});

test('configured identity remains distinct from ticket readiness', async()=>{
 const configured={...body, authenticationEnabled:true,persistenceEnabled:true};
 assert.deepEqual(await getSystem(async()=>new Response(JSON.stringify(configured))),configured);
});
