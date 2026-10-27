using System.Globalization;

namespace LocalizationCatalog;

/// <summary>
///     Metadaten zu einer Sprache / Locale. Bewusst POCO (ohne <see cref="CultureInfo" />-Property),
///     damit der Typ serialisierbar und AOT-freundlich bleibt.
/// </summary>
public sealed record LocaleDefinition
{
    /// <summary>BCP-47-Tag, z. B. "de", "pt-BR", "zh-Hant".</summary>
    public required string Code { get; init; }

    /// <summary>Name in der Sprache selbst, z. B. "Deutsch", "中文".</summary>
    public required string NativeName { get; init; }

    /// <summary>Englischer Name, z. B. "German".</summary>
    public required string EnglishName { get; init; }

    /// <summary>ISO 15924-Schriftcode, z. B. "Latn", "Cyrl", "Arab", "Hans".</summary>
    public required string Script { get; init; }

    /// <summary>True für RTL-Sprachen (Arabisch, Hebräisch, Persisch, Urdu).</summary>
    public bool IsRightToLeft { get; init; }

    /// <summary>Für diese Sprache relevante CLDR-Pluralkategorien.</summary>
    public required IReadOnlySet<PluralCategory> PluralCategories { get; init; }

    /// <summary>ISO-4217-Währungscode, z. B. "EUR".</summary>
    public string? CurrencyCode { get; init; }

    /// <summary>Währungssymbol, z. B. "EUR".</summary>
    public string? CurrencySymbol { get; init; }

    /// <summary>IANA-Zeitzone, z. B. "Europe/Berlin".</summary>
    public string? DefaultTimeZoneId { get; init; }
}