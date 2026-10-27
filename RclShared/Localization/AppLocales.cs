using System.Globalization;
using LocalizationCatalog;

namespace RclShared.Localization;

/// <summary>
///     Sprachen, die diese Anwendung tatsächlich ausliefert - die Auswahl aus
///     dem Vorrat in <see cref="LocaleCatalog" />. Hier pflegen, nicht im Katalog.
/// </summary>
public static class AppLocales
{
    /// <summary>Sprachcodes (BCP-47), für die .resx-Dateien existieren.</summary>
    public static IReadOnlyList<string> SupportedLanguages { get; } =
    [
        "en", "ru", "ar", "zh-Hant"
    ];

    /// <summary>
    ///     Alle im <see cref="LocaleCatalog" /> definierten Sprachcodes.
    ///     Reihenfolge: die zuvor explizit unterstützten Sprachen zuerst, danach alphabetisch.
    /// </summary>
    public static IReadOnlyList<string> SupportedLanguagesXXXXXXXXXXXXX { get; } =
    [
        // Zuvor explizit unterstützt (Reihenfolge unverändert)
        "de", "en", "it", "ru", "ar", "fr",

        // Restliche Katalog-Sprachen, alphabetisch gruppiert
        // Europa
        "bg", "bs", "ca", "cs", "da", "el", "es", "et", "eu", "fi",
        "ga", "gl", "hr", "hu", "is", "lb", "lt", "lv", "mk", "mt",
        "nb", "nl", "pl", "pt", "pt-BR", "ro", "si", "sk", "sl", "sq",
        "sr", "sv",
        // Eurasien / Naher Osten
        "af", "am", "az", "be", "fa", "he", "hy", "ka", "kk", "mn",
        "sw", "tr", "uk", "uz",
        // Asien
        "bn", "gu", "hi", "id", "ja", "km", "kn", "ko", "lo", "ml",
        "mr", "ms", "my", "ne", "pa", "ta", "te", "th", "tl", "ur",
        "vi", "zh", "zh-Hant"
    ];

    /// <summary>
    ///     Formatting-Kultur für den ersten Besuch. Bewusst regionsspezifisch
    ///     ("de-DE"), damit Datum/Zahlen korrekt formatiert werden - die UI-Texte
    ///     fallen über IStringLocalizer auf Resources.de.resx zurück.
    /// </summary>
    public static string DefaultCulture { get; } = "en-US";

    /// <summary>
    ///     Optionale Flaggen pro Sprache. Bewusst NICHT im LocaleCatalog,
    ///     weil Flagge -> Region eine App-spezifische Wahl ist
    ///     (z. B. "ar" -> 🇸🇦, aber "ar" wird in 20+ Ländern gesprochen).
    /// </summary>
    public static IReadOnlyDictionary<string, string> FlagsByLanguage { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["de"] = "🇩🇪",
            ["en"] = "🇬🇧",
            ["it"] = "🇮🇹",
            ["ru"] = "🇷🇺",
            ["ar"] = "🇸🇦",
            ["fr"] = "🇫🇷"
        };

    /// <summary>Liefert die Flagge für einen Sprachcode, oder null wenn keine hinterlegt ist.</summary>
    public static string? GetFlag(string code)
    {
        return FlagsByLanguage.GetValueOrDefault(code);
    }

    /// <summary>
    ///     Prüft, ob die Sprache eines BCP-47-Codes unterstützt wird.
    ///     Nutzt die BCL-Parent-Kette: "de-AT" -> "de" -> unterstützt.
    /// </summary>
    public static bool IsSupported(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        try
        {
            var culture = CultureInfo.GetCultureInfo(code);
            while (culture != CultureInfo.InvariantCulture && !string.IsNullOrEmpty(culture.Name))
            {
                if (SupportedLanguages.Contains(culture.Name, StringComparer.OrdinalIgnoreCase))
                {
                    return true;
                }

                culture = culture.Parent;
            }

            return false;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }
}