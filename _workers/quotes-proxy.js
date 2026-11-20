/**
 * Quote proxy for the blog's live quote strip.
 *
 * Deploy with wrangler, then set the route in _config.yml:
 *
 *   quotes:
 *     endpoint: "https://quotes.example.workers.dev/"
 *
 * Why a proxy and not a direct call from the page:
 *   - the provider key never reaches the browser;
 *   - readers' IP addresses never reach the provider;
 *   - one edge-cached upstream call serves every reader for the cache window.
 *
 * Required binding: QUOTES_API_KEY, the provider token (Finnhub by default).
 * Optional vars: ALLOWED_ORIGIN (default https://ypyl.github.io), PROVIDER.
 *
 * Response shape matches assets/js/quotes.js:
 *   { "as_of": "...", "quotes": [ { symbol, price, change_pct, currency, market_time, market_state } ] }
 */
'use strict';

const CACHE_SECONDS = 60;
const MAX_SYMBOLS = 12;

// Hardcoded allowlist, so a plain `wrangler deploy` is always safe. Keep it in
// step with site.quotes.tickers in _config.yml: that list decides what the bar
// shows, this one decides what the Worker will spend a provider call on. A name
// missing here is dropped from the response, and the bar renders "n/a" for it.
// Deploy with --var TICKERS=... to override without touching the code.
const DEFAULT_TICKERS = 'NVDA,ANT,MSFT,GOOGL,AMZN,META,AMD,AVGO,TSM,PLTR,ORCL,ARM,SMCI';

export default {
  async fetch(request, env, ctx) {
    // ALLOWED_ORIGIN takes a comma-separated list so production and a local
    // preview can both call the Worker. The matching origin is echoed back;
    // anything else gets the first entry, which the browser then rejects.
    const allowed = (env.ALLOWED_ORIGIN || 'https://ypyl.github.io')
      .split(',')
      .map((value) => value.trim())
      .filter(Boolean);
    const requestOrigin = request.headers.get('Origin') || '';
    const origin = allowed.indexOf(requestOrigin) === -1 ? allowed[0] : requestOrigin;
    const cors = {
      'Access-Control-Allow-Origin': origin,
      'Access-Control-Allow-Methods': 'GET, OPTIONS',
      'Access-Control-Allow-Headers': 'Accept',
      'Vary': 'Origin',
    };

    if (request.method === 'OPTIONS') return new Response(null, { status: 204, headers: cors });
    if (request.method !== 'GET') return json({ error: 'method_not_allowed' }, 405, cors);

    const url = new URL(request.url);
    const requested = (url.searchParams.get('symbols') || '')
      .split(',')
      .map((s) => s.trim().toUpperCase())
      .filter((s) => /^[A-Z][A-Z0-9.\-]{0,9}$/.test(s))
      .slice(0, MAX_SYMBOLS);

    // Allowlist. Without it any caller could invent symbol strings, miss the
    // cache every time and spend the provider quota on lookups nobody asked for.
    // TICKERS is set at deploy time (--var TICKERS=NVDA,MSFT,...).
    const allowedTickers = (env.TICKERS || DEFAULT_TICKERS)
      .split(',')
      .map((s) => s.trim().toUpperCase())
      .filter(Boolean);
    const symbols = allowedTickers.length
      ? requested.filter((s) => allowedTickers.indexOf(s) !== -1)
      : requested;

    if (!symbols.length) return json({ error: 'no_valid_symbols' }, 400, cors);
    if (!env.QUOTES_API_KEY) return json({ error: 'missing_provider_key' }, 500, cors);

    // The edge cache is what keeps the provider happy: one upstream call per minute
    // per symbol set, not one per reader.
    const cacheKey = new Request('https://quotes.internal/' + symbols.join(',') + '?v=1', { method: 'GET' });
    const cache = caches.default;
    const hit = await cache.match(cacheKey);
    if (hit) {
      const body = await hit.text();
      return new Response(body, {
        headers: { ...cors, 'Content-Type': 'application/json; charset=utf-8', 'X-Cache': 'HIT' },
      });
    }

    try {
      const quotes = await Promise.all(symbols.map((symbol) => fetchQuote(symbol, env)));
      const payload = { as_of: new Date().toISOString(), quotes: quotes.filter(Boolean) };
      const response = json(payload, 200, { ...cors, 'Cache-Control': `public, max-age=${CACHE_SECONDS}` });
      ctx.waitUntil(cache.put(cacheKey, response.clone()));
      return new Response(JSON.stringify(payload), {
        headers: { ...cors, 'Content-Type': 'application/json; charset=utf-8', 'X-Cache': 'MISS' },
      });
    } catch (err) {
      return json({ error: 'upstream_failed', detail: String(err && err.message || err) }, 502, cors);
    }
  },
};

async function fetchQuote(symbol, env) {
  // Finnhub: free tier, real-time US equities, /quote returns c, d, dp, t.
  const provider = env.PROVIDER || 'finnhub';
  if (provider !== 'finnhub') throw new Error('unsupported provider: ' + provider);

  const res = await fetch(
    'https://finnhub.io/api/v1/quote?symbol=' + encodeURIComponent(symbol) + '&token=' + env.QUOTES_API_KEY,
    { headers: { Accept: 'application/json' } }
  );
  if (!res.ok) throw new Error(symbol + ': HTTP ' + res.status);
  const data = await res.json();
  if (typeof data.c !== 'number' || data.c === 0) return null;

  return {
    symbol,
    price: Number(data.c.toFixed(2)),
    change_pct: typeof data.dp === 'number' ? Number(data.dp.toFixed(2)) : null,
    currency: 'USD',
    market_time: data.t ? new Date(data.t * 1000).toISOString() : null,
    market_state: null,
  };
}

function json(payload, status, headers) {
  return new Response(JSON.stringify(payload), {
    status,
    headers: { 'Content-Type': 'application/json; charset=utf-8', ...headers },
  });
}
