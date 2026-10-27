using System.Collections.Frozen;

namespace LocalizationCatalog;

/// <summary>
///     Zentrale, unveränderliche Liste bekannter Locales (Vorrat, aus dem Anwendungen wählen).
/// </summary>
/// <remarks>
///     Die Liste deckt bewusst sowohl globale Lingua Franca (z. B. en, zh, es, de, fr, ar)
///     als auch Sprachen ab, die primär für <b>lokale Arbeitsmärkte</b> relevant sind
///     (z. B. lb, is, ga, mt, sl, sq, mk, ka, hy, mn, km, lo, my, am, sw).
///     Für ein weltweit rekrutierendes System ist beides erforderlich, da viele Stellen
///     lokal explizit lokale Sprachkenntnisse voraussetzen.
///     <para>
///         Bewusste Auslassung: yo (Yoruba), ig (Igbo), ha (Hausa), zu (Zulu).
///         Diese Sprachen sind lokal relevant, werden aber als <b>UI-Sprache</b> kaum
///         gewählt - Sprecher nutzen in der Regel Englisch, etablierte UI-Terminologie
///         fehlt. Der Pflegeaufwand für .resx-Dateien steht in keinem Verhältnis zum Nutzen.
///     </para>
/// </remarks>
public static class LocaleCatalog
{
    private static readonly LocaleDefinition[] AllArray =
    [
        // ---------- Westeuropa ----------
        new()
        {
            Code = "en",
            NativeName = "English",
            EnglishName = "English",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "USD",
            CurrencySymbol = "$",
            DefaultTimeZoneId = "America/New_York"
        },
        new()
        {
            Code = "de",
            NativeName = "Deutsch",
            EnglishName = "German",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Berlin"
        },
        new()
        {
            Code = "fr",
            NativeName = "Français",
            EnglishName = "French",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Paris"
        },
        new()
        {
            Code = "es",
            NativeName = "Español",
            EnglishName = "Spanish",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Madrid"
        },
        new()
        {
            Code = "it",
            NativeName = "Italiano",
            EnglishName = "Italian",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Rome"
        },
        new()
        {
            Code = "pt",
            NativeName = "Português",
            EnglishName = "Portuguese",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Lisbon"
        },
        new()
        {
            Code = "pt-BR",
            NativeName = "Português (Brasil)",
            EnglishName = "Portuguese (Brazil)",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "BRL",
            CurrencySymbol = "R$",
            DefaultTimeZoneId = "America/Sao_Paulo"
        },
        new()
        {
            Code = "nl",
            NativeName = "Nederlands",
            EnglishName = "Dutch",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Amsterdam"
        },
        new()
        {
            Code = "lb",
            NativeName = "Lëtzebuergesch",
            EnglishName = "Luxembourgish",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Luxembourg"
        },

        // ---------- Mittel-/Osteuropa ----------
        new()
        {
            Code = "pl",
            NativeName = "Polski",
            EnglishName = "Polish",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories =
                new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Few, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "PLN",
            CurrencySymbol = "zł",
            DefaultTimeZoneId = "Europe/Warsaw"
        },
        new()
        {
            Code = "cs",
            NativeName = "Čeština",
            EnglishName = "Czech",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories =
                new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Few, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "CZK",
            CurrencySymbol = "Kč",
            DefaultTimeZoneId = "Europe/Prague"
        },
        new()
        {
            Code = "sk",
            NativeName = "Slovenčina",
            EnglishName = "Slovak",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories =
                new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Few, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Bratislava"
        },
        new()
        {
            Code = "hu",
            NativeName = "Magyar",
            EnglishName = "Hungarian",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "HUF",
            CurrencySymbol = "Ft",
            DefaultTimeZoneId = "Europe/Budapest"
        },
        new()
        {
            Code = "ro",
            NativeName = "Română",
            EnglishName = "Romanian",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Few, PluralCategory.Other },
            CurrencyCode = "RON",
            CurrencySymbol = "lei",
            DefaultTimeZoneId = "Europe/Bucharest"
        },
        new()
        {
            Code = "bg",
            NativeName = "Български",
            EnglishName = "Bulgarian",
            Script = "Cyrl",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "BGN",
            CurrencySymbol = "лв",
            DefaultTimeZoneId = "Europe/Sofia"
        },
        new()
        {
            Code = "hr",
            NativeName = "Hrvatski",
            EnglishName = "Croatian",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Few, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Zagreb"
        },
        new()
        {
            Code = "sl",
            NativeName = "Slovenščina",
            EnglishName = "Slovenian",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories =
                new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Two, PluralCategory.Few, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Ljubljana"
        },
        new()
        {
            Code = "sr",
            NativeName = "Српски",
            EnglishName = "Serbian",
            Script = "Cyrl",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Few, PluralCategory.Other },
            CurrencyCode = "RSD",
            CurrencySymbol = "дин",
            DefaultTimeZoneId = "Europe/Belgrade"
        },
        new()
        {
            Code = "bs",
            NativeName = "Bosanski",
            EnglishName = "Bosnian",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Few, PluralCategory.Other },
            CurrencyCode = "BAM",
            CurrencySymbol = "KM",
            DefaultTimeZoneId = "Europe/Sarajevo"
        },
        new()
        {
            Code = "mk",
            NativeName = "Македонски",
            EnglishName = "Macedonian",
            Script = "Cyrl",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "MKD",
            CurrencySymbol = "ден",
            DefaultTimeZoneId = "Europe/Skopje"
        },
        new()
        {
            Code = "sq",
            NativeName = "Shqip",
            EnglishName = "Albanian",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "ALL",
            CurrencySymbol = "L",
            DefaultTimeZoneId = "Europe/Tirane"
        },
        new()
        {
            Code = "el",
            NativeName = "Ελληνικά",
            EnglishName = "Greek",
            Script = "Grek",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Athens"
        },

        // ---------- Nordeuropa + Baltikum ----------
        new()
        {
            Code = "sv",
            NativeName = "Svenska",
            EnglishName = "Swedish",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "SEK",
            CurrencySymbol = "kr",
            DefaultTimeZoneId = "Europe/Stockholm"
        },
        new()
        {
            Code = "da",
            NativeName = "Dansk",
            EnglishName = "Danish",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "DKK",
            CurrencySymbol = "kr",
            DefaultTimeZoneId = "Europe/Copenhagen"
        },
        new()
        {
            Code = "nb",
            NativeName = "Norsk bokmål",
            EnglishName = "Norwegian Bokmål",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "NOK",
            CurrencySymbol = "kr",
            DefaultTimeZoneId = "Europe/Oslo"
        },
        new()
        {
            Code = "fi",
            NativeName = "Suomi",
            EnglishName = "Finnish",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Helsinki"
        },
        new()
        {
            Code = "is",
            NativeName = "Íslenska",
            EnglishName = "Icelandic",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "ISK",
            CurrencySymbol = "kr",
            DefaultTimeZoneId = "Atlantic/Reykjavik"
        },
        new()
        {
            Code = "lt",
            NativeName = "Lietuvių",
            EnglishName = "Lithuanian",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories =
                new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Few, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Vilnius"
        },
        new()
        {
            Code = "lv",
            NativeName = "Latviešu",
            EnglishName = "Latvian",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.Zero, PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Riga"
        },
        new()
        {
            Code = "et",
            NativeName = "Eesti",
            EnglishName = "Estonian",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Tallinn"
        },

        // ---------- Regionale EU-Sprachen ----------
        new()
        {
            Code = "ga",
            NativeName = "Gaeilge",
            EnglishName = "Irish",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory>
            {
                PluralCategory.One,
                PluralCategory.Two,
                PluralCategory.Few,
                PluralCategory.Many,
                PluralCategory.Other
            },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Dublin"
        },
        new()
        {
            Code = "mt",
            NativeName = "Malti",
            EnglishName = "Maltese",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory>
            {
                PluralCategory.One,
                PluralCategory.Two,
                PluralCategory.Few,
                PluralCategory.Many,
                PluralCategory.Other
            },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Malta"
        },
        new()
        {
            Code = "ca",
            NativeName = "Català",
            EnglishName = "Catalan",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Andorra"
        },
        new()
        {
            Code = "eu",
            NativeName = "Euskara",
            EnglishName = "Basque",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Madrid"
        },
        new()
        {
            Code = "gl",
            NativeName = "Galego",
            EnglishName = "Galician",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "EUR",
            CurrencySymbol = "EUR",
            DefaultTimeZoneId = "Europe/Madrid"
        },

        // ---------- Osteuropa / Eurasien ----------
        new()
        {
            Code = "ru",
            NativeName = "Русский",
            EnglishName = "Russian",
            Script = "Cyrl",
            IsRightToLeft = false,
            PluralCategories =
                new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Few, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "RUB",
            CurrencySymbol = "₽",
            DefaultTimeZoneId = "Europe/Moscow"
        },
        new()
        {
            Code = "uk",
            NativeName = "Українська",
            EnglishName = "Ukrainian",
            Script = "Cyrl",
            IsRightToLeft = false,
            PluralCategories =
                new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Few, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "UAH",
            CurrencySymbol = "₴",
            DefaultTimeZoneId = "Europe/Kyiv"
        },
        new()
        {
            Code = "be",
            NativeName = "Беларуская",
            EnglishName = "Belarusian",
            Script = "Cyrl",
            IsRightToLeft = false,
            PluralCategories =
                new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Few, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "BYN",
            CurrencySymbol = "Br",
            DefaultTimeZoneId = "Europe/Minsk"
        },
        new()
        {
            Code = "tr",
            NativeName = "Türkçe",
            EnglishName = "Turkish",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "TRY",
            CurrencySymbol = "₺",
            DefaultTimeZoneId = "Europe/Istanbul"
        },
        new()
        {
            Code = "az",
            NativeName = "Azərbaycan dili",
            EnglishName = "Azerbaijani",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "AZN",
            CurrencySymbol = "₼",
            DefaultTimeZoneId = "Asia/Baku"
        },
        new()
        {
            Code = "kk",
            NativeName = "Қазақ тілі",
            EnglishName = "Kazakh",
            Script = "Cyrl",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "KZT",
            CurrencySymbol = "₸",
            DefaultTimeZoneId = "Asia/Almaty"
        },
        new()
        {
            Code = "uz",
            NativeName = "Oʻzbekcha",
            EnglishName = "Uzbek",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "UZS",
            CurrencySymbol = "soʻm",
            DefaultTimeZoneId = "Asia/Tashkent"
        },
        new()
        {
            Code = "ka",
            NativeName = "ქართული",
            EnglishName = "Georgian",
            Script = "Geor",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "GEL",
            CurrencySymbol = "₾",
            DefaultTimeZoneId = "Asia/Tbilisi"
        },
        new()
        {
            Code = "hy",
            NativeName = "Հայերեն",
            EnglishName = "Armenian",
            Script = "Armn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "AMD",
            CurrencySymbol = "֏",
            DefaultTimeZoneId = "Asia/Yerevan"
        },
        new()
        {
            Code = "mn",
            NativeName = "Монгол",
            EnglishName = "Mongolian",
            Script = "Cyrl",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "MNT",
            CurrencySymbol = "₮",
            DefaultTimeZoneId = "Asia/Ulaanbaatar"
        },

        // ---------- Naher Osten ----------
        new()
        {
            Code = "ar",
            NativeName = "العربية",
            EnglishName = "Arabic",
            Script = "Arab",
            IsRightToLeft = true,
            PluralCategories = new HashSet<PluralCategory>
            {
                PluralCategory.Zero,
                PluralCategory.One,
                PluralCategory.Two,
                PluralCategory.Few,
                PluralCategory.Many,
                PluralCategory.Other
            },
            CurrencyCode = "SAR",
            CurrencySymbol = "ر.س",
            DefaultTimeZoneId = "Asia/Riyadh"
        },
        new()
        {
            Code = "he",
            NativeName = "עברית",
            EnglishName = "Hebrew",
            Script = "Hebr",
            IsRightToLeft = true,
            PluralCategories =
                new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Two, PluralCategory.Many, PluralCategory.Other },
            CurrencyCode = "ILS",
            CurrencySymbol = "₪",
            DefaultTimeZoneId = "Asia/Jerusalem"
        },
        new()
        {
            Code = "fa",
            NativeName = "فارسی",
            EnglishName = "Persian",
            Script = "Arab",
            IsRightToLeft = true,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "IRR",
            CurrencySymbol = "﷼",
            DefaultTimeZoneId = "Asia/Tehran"
        },

        // ---------- Ostasien ----------
        new()
        {
            Code = "zh",
            NativeName = "中文",
            EnglishName = "Chinese",
            Script = "Hans",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.Other },
            CurrencyCode = "CNY",
            CurrencySymbol = "¥",
            DefaultTimeZoneId = "Asia/Shanghai"
        },
        new()
        {
            Code = "zh-Hant",
            NativeName = "繁體中文",
            EnglishName = "Chinese (Traditional)",
            Script = "Hant",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.Other },
            CurrencyCode = "TWD",
            CurrencySymbol = "NT$",
            DefaultTimeZoneId = "Asia/Taipei"
        },
        new()
        {
            Code = "ja",
            NativeName = "日本語",
            EnglishName = "Japanese",
            Script = "Jpan",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.Other },
            CurrencyCode = "JPY",
            CurrencySymbol = "¥",
            DefaultTimeZoneId = "Asia/Tokyo"
        },
        new()
        {
            Code = "ko",
            NativeName = "한국어",
            EnglishName = "Korean",
            Script = "Kore",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.Other },
            CurrencyCode = "KRW",
            CurrencySymbol = "₩",
            DefaultTimeZoneId = "Asia/Seoul"
        },

        // ---------- Südasien ----------
        new()
        {
            Code = "hi",
            NativeName = "हिन्दी",
            EnglishName = "Hindi",
            Script = "Deva",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "INR",
            CurrencySymbol = "₹",
            DefaultTimeZoneId = "Asia/Kolkata"
        },
        new()
        {
            Code = "bn",
            NativeName = "বাংলা",
            EnglishName = "Bengali",
            Script = "Beng",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "BDT",
            CurrencySymbol = "৳",
            DefaultTimeZoneId = "Asia/Dhaka"
        },
        new()
        {
            Code = "ur",
            NativeName = "اردو",
            EnglishName = "Urdu",
            Script = "Arab",
            IsRightToLeft = true,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "PKR",
            CurrencySymbol = "₨",
            DefaultTimeZoneId = "Asia/Karachi"
        },
        new()
        {
            Code = "ta",
            NativeName = "தமிழ்",
            EnglishName = "Tamil",
            Script = "Taml",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "INR",
            CurrencySymbol = "₹",
            DefaultTimeZoneId = "Asia/Kolkata"
        },
        new()
        {
            Code = "te",
            NativeName = "తెలుగు",
            EnglishName = "Telugu",
            Script = "Telu",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "INR",
            CurrencySymbol = "₹",
            DefaultTimeZoneId = "Asia/Kolkata"
        },
        new()
        {
            Code = "mr",
            NativeName = "मराठी",
            EnglishName = "Marathi",
            Script = "Deva",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "INR",
            CurrencySymbol = "₹",
            DefaultTimeZoneId = "Asia/Kolkata"
        },
        new()
        {
            Code = "gu",
            NativeName = "ગુજરાતી",
            EnglishName = "Gujarati",
            Script = "Gujr",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "INR",
            CurrencySymbol = "₹",
            DefaultTimeZoneId = "Asia/Kolkata"
        },
        new()
        {
            Code = "kn",
            NativeName = "ಕನ್ನಡ",
            EnglishName = "Kannada",
            Script = "Knda",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "INR",
            CurrencySymbol = "₹",
            DefaultTimeZoneId = "Asia/Kolkata"
        },
        new()
        {
            Code = "ml",
            NativeName = "മലയാളം",
            EnglishName = "Malayalam",
            Script = "Mlym",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "INR",
            CurrencySymbol = "₹",
            DefaultTimeZoneId = "Asia/Kolkata"
        },
        new()
        {
            Code = "pa",
            NativeName = "ਪੰਜਾਬੀ",
            EnglishName = "Punjabi",
            Script = "Guru",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "INR",
            CurrencySymbol = "₹",
            DefaultTimeZoneId = "Asia/Kolkata"
        },
        new()
        {
            Code = "si",
            NativeName = "සිංහල",
            EnglishName = "Sinhala",
            Script = "Sinh",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "LKR",
            CurrencySymbol = "රු",
            DefaultTimeZoneId = "Asia/Colombo"
        },
        new()
        {
            Code = "ne",
            NativeName = "नेपाली",
            EnglishName = "Nepali",
            Script = "Deva",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "NPR",
            CurrencySymbol = "रू",
            DefaultTimeZoneId = "Asia/Kathmandu"
        },

        // ---------- Südostasien ----------
        new()
        {
            Code = "id",
            NativeName = "Bahasa Indonesia",
            EnglishName = "Indonesian",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.Other },
            CurrencyCode = "IDR",
            CurrencySymbol = "Rp",
            DefaultTimeZoneId = "Asia/Jakarta"
        },
        new()
        {
            Code = "ms",
            NativeName = "Bahasa Melayu",
            EnglishName = "Malay",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.Other },
            CurrencyCode = "MYR",
            CurrencySymbol = "RM",
            DefaultTimeZoneId = "Asia/Kuala_Lumpur"
        },
        new()
        {
            Code = "vi",
            NativeName = "Tiếng Việt",
            EnglishName = "Vietnamese",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "VND",
            CurrencySymbol = "₫",
            DefaultTimeZoneId = "Asia/Ho_Chi_Minh"
        },
        new()
        {
            Code = "th",
            NativeName = "ไทย",
            EnglishName = "Thai",
            Script = "Thai",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.Other },
            CurrencyCode = "THB",
            CurrencySymbol = "฿",
            DefaultTimeZoneId = "Asia/Bangkok"
        },
        new()
        {
            Code = "tl",
            NativeName = "Tagalog",
            EnglishName = "Tagalog",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "PHP",
            CurrencySymbol = "₱",
            DefaultTimeZoneId = "Asia/Manila"
        },
        new()
        {
            Code = "km",
            NativeName = "ខ្មែរ",
            EnglishName = "Khmer",
            Script = "Khmr",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.Other },
            CurrencyCode = "KHR",
            CurrencySymbol = "៛",
            DefaultTimeZoneId = "Asia/Phnom_Penh"
        },
        new()
        {
            Code = "lo",
            NativeName = "ລາວ",
            EnglishName = "Lao",
            Script = "Laoo",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.Other },
            CurrencyCode = "LAK",
            CurrencySymbol = "₭",
            DefaultTimeZoneId = "Asia/Vientiane"
        },
        new()
        {
            Code = "my",
            NativeName = "မြန်မာ",
            EnglishName = "Burmese",
            Script = "Mymr",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.Other },
            CurrencyCode = "MMK",
            CurrencySymbol = "K",
            DefaultTimeZoneId = "Asia/Yangon"
        },

        // ---------- Afrika ----------
        // Bewusst entfernt (Stufe 3): "yo" (Yoruba), "ig" (Igbo), "ha" (Hausa), "zu" (Zulu).
        // Begründung: lokal relevant, aber als UI-Sprache kaum gewählt - Sprecher
        // nutzen überwiegend Englisch; etablierte UI-Terminologie fehlt. Der .resx-
        // Pflegeaufwand steht in keinem Verhältnis zum erwarteten Nutzen.
        new()
        {
            Code = "sw",
            NativeName = "Kiswahili",
            EnglishName = "Swahili",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "TZS",
            CurrencySymbol = "TSh",
            DefaultTimeZoneId = "Africa/Dar_es_Salaam"
        },
        new()
        {
            Code = "af",
            NativeName = "Afrikaans",
            EnglishName = "Afrikaans",
            Script = "Latn",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "ZAR",
            CurrencySymbol = "R",
            DefaultTimeZoneId = "Africa/Johannesburg"
        },
        new()
        {
            Code = "am",
            NativeName = "አማርኛ",
            EnglishName = "Amharic",
            Script = "Ethi",
            IsRightToLeft = false,
            PluralCategories = new HashSet<PluralCategory> { PluralCategory.One, PluralCategory.Other },
            CurrencyCode = "ETB",
            CurrencySymbol = "Br",
            DefaultTimeZoneId = "Africa/Addis_Ababa"
        }
    ];

    private static readonly FrozenDictionary<string, LocaleDefinition> ByCode =
        AllArray.ToFrozenDictionary(l => l.Code, StringComparer.OrdinalIgnoreCase);

    /// <summary>Alle definierten Locales in Deklarationsreihenfolge.</summary>
    public static IReadOnlyList<LocaleDefinition> All => AllArray;

    /// <summary>Schneller Lookup per BCP-47-Code (case-insensitive).</summary>
    public static bool TryGet(string code, out LocaleDefinition locale)
    {
        return ByCode.TryGetValue(code, out locale!);
    }

    /// <summary>Lookup per BCP-47-Code, <c>null</c> wenn unbekannt.</summary>
    public static LocaleDefinition? Find(string code)
    {
        return ByCode.GetValueOrDefault(code);
    }

    /// <summary>Lookup per BCP-47-Code, wirft bei unbekanntem Code.</summary>
    public static LocaleDefinition Get(string code)
    {
        return ByCode[code];
    }
}