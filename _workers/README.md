# Quote proxy

`quotes-proxy.js` is the Cloudflare Worker behind the live quote strip. It is **not** deployed by
Jekyll: the leading underscore keeps the folder out of the build.

## What it does

`GET /?symbols=JNJ,PG,GIS`

```json
{ "as_of": "2026-11-20T20:08:08Z",
  "quotes": [ { "symbol": "JNJ", "price": 271.46, "change_pct": 1.62, "currency": "USD",
                "market_time": "2026-11-20T20:08:02Z", "market_state": null } ] }
```

- Validates symbols (1 to 10 uppercase characters, at most 12 per request) before spending a provider call.
- Rejects anything outside the allowlist, which is hardcoded as `DEFAULT_TICKERS` in `quotes-proxy.js` and
  listed here for reference: NVDA, ANT, MSFT, GOOGL, AMZN, META, AMD, AVGO, TSM, PLTR, ORCL, ARM, SMCI.
  Keep it in step with `site.quotes.tickers` in `_config.yml`: that list decides what the bar shows, this
  one decides what the Worker will pay for. A plain `wrangler deploy` keeps the allowlist, which is the
  point of hardcoding it; `--var TICKERS=...` overrides without a code change.
- Caches the answer at the edge for 60 seconds, so a thousand readers cost one upstream call a minute.
- Answers CORS only for `ALLOWED_ORIGIN`, so another site cannot lift your quota.
- Never exposes the provider key: it lives in the Worker secret.

## Deploy

```bash
cd _workers
npx wrangler init --yes quotes-proxy 2>/dev/null || true   # once, if there is no wrangler.toml
npx wrangler deploy quotes-proxy.js --name blog-quotes
npx wrangler secret put QUOTES_API_KEY                     # Finnhub token
npx wrangler secret put ALLOWED_ORIGIN                     # optional, defaults to https://ypyl.github.io
```

Then point the site at it in `_config.yml`:

```yaml
quotes:
  endpoint: "https://blog-quotes.<account>.workers.dev/"
```

With `endpoint` empty the strip renders the build-time snapshot and never makes a network call, which
is the right default for a fork or a machine with no deploy.

## Provider notes

Finnhub's free tier covers US equities and is enough for a reading list of dividend names. Free tiers
change; check the current limits before a post depends on one. Real-time consolidated data for every
exchange needs a licensed feed, and no free tier has that.

Finnhub is the only provider implemented. `PROVIDER` exists so a second one can be added without
touching the Worker's public shape.

## Observability

`wrangler tail` shows each request with its `X-Cache` header. The site renders no prices of its own:
`assets/js/quotes.js` keeps every container hidden until a response arrives and hides it again when a
request fails, so a broken Worker, a missing secret or an exhausted quota all look the same, no bar
and no numbers.
