/**
 * Live quotes for _includes/ticker.html (the fixed bar) and _includes/quotes.html
 * (a post strip).
 *
 * There is no snapshot in the page. Every container is rendered `hidden` and only
 * revealed after a successful response, so a failed request means no numbers at
 * all; it never means yesterday's numbers wearing a live label.
 *
 * Expected response:
 *   { "as_of": "2026-11-20T21:48:42Z",
 *     "quotes": [ { "symbol": "NVDA", "price": 125.53, "change_pct": -14.01,
 *                   "currency": "USD", "market_time": "2026-11-20T21:00:00Z" } ] }
 */
(function () {
  'use strict';

  var containers = Array.prototype.slice.call(document.querySelectorAll('[data-quotes]'));
  if (!containers.length) return;

  var endpoint = '';
  var refreshMs = 60000;

  containers.forEach(function (container) {
    if (!endpoint) {
      var candidate = (container.getAttribute('data-endpoint') || '').trim();
      if (candidate) endpoint = candidate;
    }
    var seconds = parseInt(container.getAttribute('data-refresh'), 10);
    if (seconds && seconds * 1000 < refreshMs) refreshMs = Math.max(15, seconds) * 1000;
  });

  var elements = Array.prototype.slice.call(document.querySelectorAll('[data-quote]'));
  if (!elements.length) return;

  var nodes = {};
  elements.forEach(function (el) {
    var symbol = (el.getAttribute('data-symbol') || '').toUpperCase();
    if (!symbol) return;
    (nodes[symbol] = nodes[symbol] || []).push(el);
  });
  var symbols = Object.keys(nodes);
  if (!symbols.length) return;

  var statuses = Array.prototype.slice.call(document.querySelectorAll('[data-quotes-status]'));
  var ticker = document.querySelector('[data-ticker]');
  var timer = null;

  // Nothing to fetch, nothing to show.
  if (!endpoint) return;

  function show() {
    containers.forEach(function (container) {
      container.hidden = false;
    });
    document.body.classList.add('ticker-visible');
  }

  function hide() {
    containers.forEach(function (container) {
      container.hidden = true;
    });
    document.body.classList.remove('ticker-visible');
  }

  function formatStamp(iso) {
    var d = new Date(iso);
    if (isNaN(d)) return '';
    return new Intl.DateTimeFormat('en', {
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      timeZone: 'UTC',
      timeZoneName: 'short'
    }).format(d);
  }

  function formatPrice(value, currency) {
    if (typeof value !== 'number') return '';
    try {
      return new Intl.NumberFormat('en', {
        style: 'currency',
        currency: currency || 'USD',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
      }).format(value);
    } catch (e) {
      return value.toFixed(2);
    }
  }

  function paint(el, quote) {
    var priceEl = el.querySelector('[data-quote-price]');
    var changeEl = el.querySelector('[data-quote-change]');
    var label = el.querySelector('.quote-name, .ticker-name');
    var currency = quote.currency || 'USD';

    if (priceEl) priceEl.textContent = formatPrice(quote.price, currency);
    if (label && quote.label) label.textContent = quote.label;

    if (!changeEl) return;
    var pct = typeof quote.change_pct === 'number' ? quote.change_pct : null;
    changeEl.classList.remove('is-up', 'is-down');
    if (pct === null) {
      changeEl.textContent = '';
      changeEl.removeAttribute('aria-label');
      return;
    }
    var rounded = Math.round(pct * 100) / 100;
    changeEl.classList.add(rounded < 0 ? 'is-down' : 'is-up');
    changeEl.textContent = (rounded > 0 ? '+' : '') + rounded.toFixed(2) + '%';
    changeEl.setAttribute(
      'aria-label',
      (rounded < 0 ? 'down ' : rounded > 0 ? 'up ' : 'unchanged ') + Math.abs(rounded).toFixed(2) + ' percent today'
    );
  }

  function setStatus(text) {
    statuses.forEach(function (el) {
      el.textContent = text;
    });
  }

  function update() {
    var url = endpoint + (endpoint.indexOf('?') === -1 ? '?' : '&') + 'symbols=' + encodeURIComponent(symbols.join(','));
    fetch(url, { headers: { Accept: 'application/json' }, credentials: 'omit' })
      .then(function (res) {
        if (!res.ok) throw new Error('HTTP ' + res.status);
        return res.json();
      })
      .then(function (data) {
        var list = (data && data.quotes) || [];
        if (!list.length) throw new Error('empty payload');
        list.forEach(function (quote) {
          (nodes[String(quote.symbol || '').toUpperCase()] || []).forEach(function (el) {
            paint(el, quote);
          });
        });
        setStatus('Updated ' + (data.as_of ? formatStamp(data.as_of) : 'just now'));
        show();
      })
      .catch(function () {
        // No numbers, no bar. A stale price is worse than a missing one.
        hide();
      });
  }

  function start() {
    if (timer) return;
    update();
    timer = window.setInterval(update, refreshMs);
  }

  function stop() {
    if (!timer) return;
    window.clearInterval(timer);
    timer = null;
  }

  document.addEventListener('visibilitychange', function () {
    if (document.hidden) stop();
    else start();
  });

  start();
})();
