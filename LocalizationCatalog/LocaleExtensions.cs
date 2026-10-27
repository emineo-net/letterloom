using System.Globalization;

namespace LocalizationCatalog;

/// <summary>
/// </summary>
public static class LocaleExtensions
{
    /// <summary>
    ///     Liefert das <see cref="CultureInfo" /> für diesen Code. Funktioniert auch für neutrale
    ///     Kulturen (z. B. "de"). Wird von der BCL gecacht.
    /// </summary>
    public static CultureInfo GetCulture(this LocaleDefinition locale)
    {
        return CultureInfo.GetCultureInfo(locale.Code);
    }

    /// <summary>
    ///     Liefert die <see cref="RegionInfo" />, oder <c>null</c> bei neutralen Kulturen
    ///     (RegionInfo akzeptiert keine neutralen Kulturen). Achtung: wird nicht gecacht -
    ///     nicht in Hot Loops aufrufen.
    /// </summary>
    public static RegionInfo? GetRegion(this LocaleDefinition locale)
    {
        var culture = locale.GetCulture();
        return culture.IsNeutralCulture ? null : new RegionInfo(locale.Code);
    }
}