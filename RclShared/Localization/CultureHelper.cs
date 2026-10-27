namespace RclShared.Localization;

public static class CultureHelper
{
    private static readonly HashSet<string> RtlLanguages = new(StringComparer.OrdinalIgnoreCase) { "ar", "he", "fa", "ur" };

    public static bool IsRtl(string? cultureCode)
    {
        if (string.IsNullOrWhiteSpace(cultureCode))
        {
            return false;
        }

        var lang = cultureCode.Split('-', '_')[0];
        return RtlLanguages.Contains(lang);
    }

    public static string Dir(string? cultureCode)
    {
        return IsRtl(cultureCode) ? "rtl" : "ltr";
    }
}