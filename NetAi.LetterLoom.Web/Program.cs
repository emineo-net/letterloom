using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using NetAi.LetterLoom.Web.Components;
using NetAi.LetterLoom.Web.Models;
using NetAi.LetterLoom.Web.Services;
using RclShared.Localization;
using RclShared.Services;

namespace NetAi.LetterLoom.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddScoped<DownloadService>();

        var paddleSettings = builder.Configuration.GetSection("Paddle").Get<PaddleSettings>() ?? new PaddleSettings();

        builder.Services.AddSingleton(paddleSettings);
        builder.Services.AddScoped<PaddleCheckoutService>();

        // Für @inject IStringLocalizer<Resources> Loc in _Imports.razor
        builder.Services.AddLocalization();
        builder.Services.AddScoped<IContactFormService, HttpContactFormService>();

        var host = builder.Build();

        // ---------------------------------------------------------------
        // UI-Kultur aus localStorage lesen (wird vom DevLanguageSwitcher
        // via letterLoom.setCulture gesetzt). Muss VOR RunAsync passieren,
        // damit der erste Render schon die korrekte IStringLocalizer-
        // Kultur sieht.
        // ---------------------------------------------------------------
        var js = host.Services.GetRequiredService<IJSRuntime>();
        string? storedCulture = null;
        try
        {
            storedCulture = await js.InvokeAsync<string?>("letterLoom.getStoredCulture");
        }
        catch
        {
            // JS-Interop noch nicht bereit oder Funktion fehlt - Fallback greift.
        }

        var culture = ResolveCulture(storedCulture);

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        await host.RunAsync();
    }

    /// <summary>
    ///     Liefert die zu verwendende <see cref="CultureInfo" />. Gespeicherte
    ///     Kultur wird gegen <see cref="AppLocales.SupportedLanguages" /> geprüft;
    ///     bei unbekannter oder nicht unterstützter Sprache greift der App-Default.
    /// </summary>
    private static CultureInfo ResolveCulture(string? storedCulture)
    {
        // Kein gespeicherter Wert (erster Besuch) -> Default.
        if (string.IsNullOrWhiteSpace(storedCulture))
        {
            return CultureInfo.GetCultureInfo(AppLocales.DefaultCulture);
        }

        // Sprache nicht unterstützt (z. B. "fr" oder "xx-YY") -> Default.
        if (!AppLocales.IsSupported(storedCulture))
        {
            return CultureInfo.GetCultureInfo(AppLocales.DefaultCulture);
        }

        // Unterstützt: Original beibehalten, damit z. B. "de-AT" für Formatting
        // erhalten bleibt (IStringLocalizer fällt auf Resources.de.resx zurück).
        try
        {
            return CultureInfo.GetCultureInfo(storedCulture);
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.GetCultureInfo(AppLocales.DefaultCulture);
        }
    }
}