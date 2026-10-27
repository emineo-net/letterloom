using System.Globalization;

namespace LocalizationCatalog;

/// <summary>
///     Löst einen angeforderten Sprachcode in eine Fallback-Kette auf, die nur Codes enthält,
///     die tatsächlich im <see cref="LocaleCatalog" /> vorhanden sind.
/// </summary>
public static class LocaleFallbackResolver
{
    /// <summary>
    ///     Beispiel: <c>Resolve("de-AT", "en")</c> liefert <c>["de", "en"]</c>,
    ///     falls der Katalog <c>de</c> kennt, aber nicht <c>de-AT</c>.
    /// </summary>
    /// <param name="requestedCode">Angeforderter BCP-47-Code, z. B. "de-AT".</param>
    /// <param name="defaultCode">Fallback am Ende der Kette, z. B. "en".</param>
    public static IReadOnlyList<string> Resolve(string requestedCode, string? defaultCode = "en")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requestedCode);

        var chain = new List<string>(4);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 1. BCL-Parent-Kette: "de-AT" -> "de" -> "" (invariant)
        CultureInfo? culture;
        try
        {
            culture = CultureInfo.GetCultureInfo(requestedCode);
        }
        catch (CultureNotFoundException)
        {
            culture = null;
        }

        while (culture is not null && !string.IsNullOrEmpty(culture.Name))
        {
            if (LocaleCatalog.TryGet(culture.Name, out _) && seen.Add(culture.Name))
            {
                chain.Add(culture.Name);
            }

            culture = culture.Parent;
        }

        // 2. Default als letzter Anker
        if (defaultCode is not null && LocaleCatalog.TryGet(defaultCode, out _) && seen.Add(defaultCode))
        {
            chain.Add(defaultCode);
        }

        return chain;
    }
}