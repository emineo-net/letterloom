/* ================================================================
   LetterLoom - Web Interop
   - Theme-Toggle (Hell / Dunkel), persistiert in localStorage
   - Paddle Checkout (einmaliger Kauf 39 EUR), Konfiguration via Blazor
   - RTL / Culture
   ================================================================ */

window.letterLoom = window.letterLoom || {};

/* ================================================================
   Theme
   ================================================================ */

/**
 * Liefert das aktuell aktive Theme ("light" | "dark").
 */
window.letterLoom.getTheme = function () {
    return document.documentElement.getAttribute('data-theme') || 'light';
};

/**
 * Wechselt zwischen Hell und Dunkel.
 * Setzt data-theme (unsere CSS-Tokens) und data-bs-theme (Bootstrap),
 * aktualisiert color-scheme und speichert die Wahl in localStorage.
 *
 * @returns {string} Das neue Theme ("light" | "dark").
 */
window.letterLoom.toggleTheme = function () {
    var current = document.documentElement.getAttribute('data-theme') || 'light';
    var next = current === 'dark' ? 'light' : 'dark';

    document.documentElement.setAttribute('data-theme', next);
    document.documentElement.setAttribute('data-bs-theme', next);
    document.documentElement.style.colorScheme = next;

    try {
        localStorage.setItem('letterloom-theme', next);
    } catch (e) {
        // localStorage kann z. B. im privaten Modus gesperrt sein - ignorieren.
    }

    return next;
};

/* ================================================================
   Paddle Checkout (v2 / Paddle Billing)
   ----------------------------------------------------------------
   Die Konfiguration kommt aus Blazor (wwwroot/appsettings.json) -
   keine hartkodierten Seller-ID / Token / Price-ID mehr in JS.

   Aufruf aus C#:
     letterLoom.openCheckout({
         sellerId:    12345,
         clientToken: "live_...",
         priceId:     "pri_...",
         environment: "sandbox" | "production"
     })
   ================================================================ */

window.letterLoom.openCheckout = function (config) {
    if (typeof Paddle === 'undefined') {
        alert('Paddle Checkout konnte nicht geladen werden. Bitte prüfen Sie Ihre Internetverbindung.');
        return false;
    }

    if (!config || !config.clientToken || !config.priceId) {
        console.error('[LetterLoom] Paddle config incomplete:', config);
        alert('Der Kauf ist noch nicht freigeschaltet. Bitte später erneut versuchen.');
        return false;
    }

    try {
        // Paddle v2 (Paddle Billing): Initialize mit Client-Token
        if (typeof Paddle.Initialize === 'function') {
            Paddle.Initialize({
                token: config.clientToken,
                environment: config.environment || 'sandbox'
            });
        } else if (typeof Paddle.Setup === 'function') {
            // Fallback für ältere Paddle.js-Versionen (Classic)
            Paddle.Setup({
                seller: config.sellerId,
                token: config.clientToken
            });
        } else {
            console.error('[LetterLoom] Paddle.js API nicht erkannt.');
            alert('Der Checkout konnte nicht geöffnet werden.');
            return false;
        }

        Paddle.Checkout.open({
            items: [{ priceId: config.priceId, quantity: 1 }],
            settings: {
                // {transaction_id} wird von Paddle automatisch ersetzt.
                successUrl: window.location.origin +
                    '/purchase/success?transaction_id={transaction_id}'
            }
        });

        return true;
    } catch (e) {
        console.error('[LetterLoom] Paddle checkout failed:', e);
        alert('Der Checkout konnte nicht geöffnet werden. Bitte später erneut versuchen.');
        return false;
    }
};

/* ================================================================
   RTL / Culture
   ================================================================ */

/**
 * Setzt Sprache und Richtung der Seite.
 * Tauscht das Bootstrap-Stylesheet zwischen LTR- und RTL-Variante.
 * Persistiert die Kultur in localStorage.
 *
 * @param {string} lang  BCP-47-Kulturcode (z. B. "de", "en", "ar").
 * @param {string} dir   "ltr" oder "rtl".
 */
window.letterLoom.setCulture = function (lang, dir) {
    dir = (dir || 'ltr').toLowerCase() === 'rtl' ? 'rtl' : 'ltr';

    if (lang) document.documentElement.lang = lang;
    document.documentElement.dir = dir;

    var link = document.getElementById('bootstrap-css');
    if (link) {
        var href = link.getAttribute('href') || '';
        var next;

        if (dir === 'rtl') {
            next = href.indexOf('.rtl.') >= 0
                ? href
                : href.replace('bootstrap.min.css', 'bootstrap.rtl.min.css');
        } else {
            next = href.indexOf('.rtl.') < 0
                ? href
                : href.replace('bootstrap.rtl.min.css', 'bootstrap.min.css');
        }

        if (next !== href) link.setAttribute('href', next);
    }

    try { localStorage.setItem('letterloom-culture', lang || ''); } catch (e) { }
};

/**
 * Liefert die aktuell aktive Kultur.
 * Reihenfolge: localStorage -> <html lang> -> "en".
 */
window.letterLoom.getCulture = function () {
    try {
        var stored = localStorage.getItem('letterloom-culture');
        if (stored) return stored;
    } catch (e) { }

    return document.documentElement.lang || 'en';
};

/**
 * Liefert true, wenn die Seite aktuell RTL ist.
 */
window.letterLoom.isRtl = function () {
    return document.documentElement.dir === 'rtl';
};

/**
 * Erzwingt einen vollständigen Reload - genutzt vom Dev-Language-Switcher,
 * damit HTML lang/dir, Bootstrap-Stylesheet und Lokalisierung sauber neu
 * angewendet werden.
 */
window.letterLoom.reload = function () {
    location.reload();
};

window.letterLoom.getStoredCulture = function () {
    try {
        return localStorage.getItem('letterloom-culture') || '';
    } catch (e) {
        return '';
    }
};