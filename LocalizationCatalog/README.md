# LocalizationCatalog

Zentrale Bibliothek für Sprachmetadaten (BCP-47-Codes, Schrift, RTL, Pluralregeln,
Währung, Zeitzone) und Fallback-Auflösung. Keine Übersetzungen, keine
Web-Abhängigkeiten - nur der Vorrat an Metadaten, aus dem Anwendungen wählen.

- **Ziel-Framework:** `net10.0`
- **Externe Abhängigkeiten:** keine (nur BCL)
- **Enthaltene Sprachen:** 79 Locales (siehe `LocaleCatalog.All`)

## Zweck und Abgrenzung

Die Bibliothek beantwortet genau eine Frage: **„Wie sehen die Metadaten zu
einem BCP-47-Sprachcode aus?“**

Sie beantwortet *nicht* die Frage **„Welche Sprachen unterstützt meine
Anwendung?“** - das ist eine Produktentscheidung und gehört in die jeweilige
Anwendung, nicht in ein geteiltes NuGet-Paket.

| | `LocalizationCatalog` (diese Lib) | Anwendung / RCL |
|---|---|---|
| **Was** | Vorrat an Metadaten zu ~79 Locales | Auswahl der tatsächlich ausgelieferten Sprachen |
| **Wo** | `LocaleCatalog.All` | z. B. `AppLocales.SupportedLanguages` |
| **Änderungsrate** | selten (CLDR-Updates, Korrekturen) | pro Markt-Release |
| **Enthält** | Code, NativeName, Script, RTL, Pluralregeln, Währung, Zeitzone | Sprachliste, Flaggen-Map, `.resx`-Dateien, `IStringLocalizer`-Wiring |

Diese Trennung ist bewusst: Eine neue Sprache für *eine* Anwendung darf
kein Lib-Release erfordern; eine neue Locale im Katalog darf *keine*
Anwendung brechen.

## Installation

Aktuell als Project Reference:

```xml
<ProjectReference Include="..\LocalizationCatalog\LocalizationCatalog.csproj" />
```

Keine externen NuGet-Abhängigkeiten. Nutzbar in RCL, Console, Blazor Server/WA,
MAUI, WPF und Tests.

## Schnellstart

### 1. Anwendungssprachen aus dem Katalog wählen

**Nicht** `LocaleCatalog.All` direkt verwenden - das ist der Vorrat, nicht
die Auswahl. Stattdessen eine eigene Klasse in der Anwendung bzw. RCL:

```csharp
using System.Globalization;
using LocalizationCatalog;

namespace RclShared.Localization;

/// <summary>
/// Sprachen, die diese Anwendung tatsächlich ausliefert - die Auswahl aus
/// dem Vorrat in <see cref="LocaleCatalog"/>. Hier pflegen, nicht im Katalog.
/// </summary>
public static class AppLocales
{
    /// <summary>Sprachcodes (BCP-47), für die .resx-Dateien existieren.</summary>
    public static IReadOnlyList<string> SupportedLanguages { get; } =
    [
        "de", "en", "it", "ru", "ar"
    ];

    /// <summary>
    /// Formatting-Kultur für den ersten Besuch. Bewusst regionsspezifisch
    /// ("de-DE"), damit Datum/Zahlen korrekt formatiert werden - die UI-Texte
    /// fallen über IStringLocalizer auf Resources.de.resx zurück.
    /// </summary>
    public static string DefaultCulture { get; } = "de-DE";

    /// <summary>
    /// Optionale Flaggen pro Sprache. Bewusst NICHT im LocaleCatalog,
    /// weil Flagge -> Region eine App-spezifische Wahl ist
    /// (z. B. "ar" -> 🇸🇦, aber "ar" wird in 20+ Ländern gesprochen).
    /// </summary>
    public static IReadOnlyDictionary<string, string> FlagsByLanguage { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["de"] = "🇩🇪",
            ["en"] = "🇬🇧",
            ["it"] = "🇮🇹",
            ["ru"] = "🇷🇺",
            ["ar"] = "🇸🇦",
        };

    public static string? GetFlag(string code)
        => FlagsByLanguage.GetValueOrDefault(code);

    /// <summary>
    /// Prüft, ob die Sprache eines BCP-47-Codes unterstützt wird.
    /// Nutzt die BCL-Parent-Kette: "de-AT" -> "de" -> unterstützt.
    /// </summary>
    public static bool IsSupported(string? code)
    {
        if (string.IsNullOrWhiteSpace(code)) return false;
        try
        {
            var culture = CultureInfo.GetCultureInfo(code);
            while (culture != CultureInfo.InvariantCulture
                   && !string.IsNullOrEmpty(culture.Name))
            {
                if (SupportedLanguages.Contains(culture.Name, StringComparer.OrdinalIgnoreCase))
                    return true;
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
```

`LocaleCatalog.Get(code)` wirft eine `KeyNotFoundException`, wenn der Code
nicht im Katalog existiert. Das ist ein Feature: Tippfehler und veraltete
Sprachcodes fallen sofort beim App-Start auf, nicht erst zur Laufzeit im
Sprachwähler.

**Warum sind Flaggen nicht im Katalog?** Eine Flagge repräsentiert ein
Land, nicht eine Sprache. `ar` wird in über 20 Ländern gesprochen; `en`
in über 50. Die Wahl „🇸🇦 für Arabisch, 🇬🇧 für Englisch“ ist eine
App-spezifische Policy-Entscheidung, keine linguistische Metainformation.
Deshalb gehört sie in `AppLocales`, nicht in `LocaleDefinition`.

### 2. ASP.NET Core: `RequestLocalizationOptions` konfigurieren

```csharp
using LocalizationCatalog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLocalization();

var app = builder.Build();

var supportedCultures = AppLocales.SupportedLanguages.ToArray();

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("de")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

localizationOptions.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider());

app.UseRequestLocalization(localizationOptions);
```

**Hinweis:** `SetDefaultCulture` erwartet einen Code aus
`SupportedLanguages` (z. B. `"de"`), nicht den Formatting-Code
(`"de-DE"`). Der Formatting-Code wird vom `RequestLocalizationMiddleware`
nicht verwendet - der kümmert sich nur um `CurrentUICulture`.

### 3. Im Razor-Markup

```razor
@using LocalizationCatalog
@using RclShared.Localization
@inject IStringLocalizer<Resources> L

<h1>@L["WelcomeTitle"]</h1>

<div class="d-flex flex-wrap gap-2">
    @foreach (var locale in SupportedCultures)
    {
        <button class="btn @(IsCurrentCulture(locale.Code) ? "btn-primary" : "btn-outline-secondary")"
                dir="@(locale.IsRightToLeft ? "rtl" : "ltr")"
                title="@BuildTitle(locale)"
                @onclick="() => SetCultureAsync(locale.Code)">
            @if (AppLocales.GetFlag(locale.Code) is { Length: > 0 } flag)
            {
                <span style="margin-right:6px;">@flag</span>
            }
            @locale.NativeName
        </button>
    }
</div>

@code {
    private static readonly LocaleDefinition[] SupportedCultures =
        AppLocales.SupportedLanguages
            .Select(LocaleCatalog.Get)
            .ToArray();

    private static bool IsCurrentCulture(string code)
    {
        var current = CultureInfo.CurrentUICulture.Name;
        return string.Equals(code, current, StringComparison.OrdinalIgnoreCase)
            || current.StartsWith(code + "-", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildTitle(LocaleDefinition locale)
    {
        var title = locale.EnglishName;
        if (locale.IsRightToLeft) title += " - RTL";
        return title;
    }
}
```

### 4. Fallback in C#-Code

```csharp
// de-AT ist nicht im Katalog -> Fallback auf de, dann en
var chain = LocaleFallbackResolver.Resolve("de-AT", "en");
// -> ["de", "en"]

// pt-PT ist nicht im Katalog, pt schon
var chain2 = LocaleFallbackResolver.Resolve("pt-PT", "en");
// -> ["pt", "en"]
```

## Blazor WebAssembly: Kultur vor dem ersten Render setzen

Blazor WASM hat **keine `RequestLocalizationOptions` / `UseRequestLocalization`-Middleware** - die gibt es nur in ASP.NET Core. Die Kultur wird direkt über `CultureInfo` gesetzt. Das klingt einfach, hat aber eine Timing-Falle, die dazu führt, dass der Sprachwähler scheinbar nicht funktioniert.

### Warum der Zeitpunkt entscheidend ist

Blazor rendert die erste Komponente mit der Kultur, die zum Zeitpunkt von `RunAsync()` gesetzt ist. Wenn du die Kultur erst **nach** dem ersten Render aus `localStorage` liest (z. B. in `OnAfterRenderAsync` einer Komponente), siehst du folgendes Verhalten:

1. Erster Render läuft mit `navigator.language` (z. B. `en-US`).
2. `IStringLocalizer<Resources>` sucht `Resources.en-US.resx` -> nicht vorhanden -> Fallback auf `Resources.resx` (invariant, englisch).
3. Dein Code liest `localStorage` und setzt `CultureInfo.CurrentUICulture` neu.
4. **Aber**: Der `IStringLocalizer` hat die Texte beim ersten Render bereits aufgelöst. Die Seite bleibt auf Englisch, bis ein Re-Render ausgelöst wird.

Ergebnis: Der Nutzer sieht die falsche Sprache, obwohl die richtige in `localStorage` steht.

### Korrektes Muster

Kultur **vor** `RunAsync()` lesen und setzen:

```csharp
using System.Globalization;
using LocalizationCatalog;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using RclShared.Localization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// Für @inject IStringLocalizer<Resources> Loc in _Imports.razor
builder.Services.AddLocalization();

var host = builder.Build();

// ---------------------------------------------------------------
// Kultur aus localStorage lesen - MUSS vor RunAsync passieren.
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

// Richtung aus dem Katalog ableiten und anwenden - VOR RunAsync.
var locale = LocaleCatalog.Find(culture.Name);
try
{
    await js.InvokeVoidAsync("letterLoom.setDirection",
        locale?.IsRightToLeft ?? false,
        locale?.Code ?? "en");
}
catch
{
    // JS-Interop noch nicht bereit - index.html-Default (ltr) bleibt.
}

await host.RunAsync();

// ---------------------------------------------------------------
// Lokale Auflösungslogik
// ---------------------------------------------------------------
static CultureInfo ResolveCulture(string? storedCulture)
{
    if (string.IsNullOrWhiteSpace(storedCulture))
        return CultureInfo.GetCultureInfo(AppLocales.DefaultCulture);

    if (!AppLocales.IsSupported(storedCulture))
        return CultureInfo.GetCultureInfo(AppLocales.DefaultCulture);

    try
    {
        return CultureInfo.GetCultureInfo(storedCulture);
    }
    catch (CultureNotFoundException)
    {
        return CultureInfo.GetCultureInfo(AppLocales.DefaultCulture);
    }
}
```

### Zwei `try`-Blöcke - warum?

**1. `IJSRuntime`-Aufruf (erster `try`)**
Vor `RunAsync()` ist das JS-Interop noch nicht vollständig initialisiert. `letterLoom.getStoredCulture` kann fehlschlagen oder - wenn das JS-Modul noch nicht geladen ist - eine `JSException` werfen. Der `catch`-Block lässt `storedCulture` auf `null` und der Default greift.

**2. `CultureInfo`-Konstruktion (zweiter `try`)**
`new CultureInfo("xx-YY")` wirft `CultureNotFoundException` nur bei wirklich ungültigen Codes. Aber `AppLocales.IsSupported("xx-YY")` prüft davor bereits gegen die App-Sprachliste - der zweite `try` ist die letzte Verteidigungslinie für den Fall, dass jemand später `IsSupported` aufweicht.

### Wie die Kultur in `localStorage` landet

Der Sprachwähler schreibt den Wert über eine JS-Interop-Funktion. Beispiel für `wwwroot/js/letterLoom.js`:

```javascript
window.letterLoom = {
    getStoredCulture: function () {
        try { return localStorage.getItem('letterLoom.culture'); }
        catch { return null; }
    },
    setCulture: function (culture) {
        try { localStorage.setItem('letterLoom.culture', culture); return true; }
        catch { return false; }
    },
    setDirection: function (isRtl, lang) {
        document.documentElement.dir = isRtl ? 'rtl' : 'ltr';
        document.documentElement.lang = lang;
    }
};
```

Und im Sprachwähler (Razor):

```razor
@inject IJSRuntime JS
@inject NavigationManager Nav

<select @onchange="OnLanguageChanged"
        dir="@(LocaleCatalog.Get(CurrentLanguage).IsRightToLeft ? "rtl" : "ltr")">
    @foreach (var locale in SupportedCultures)
    {
        <option value="@locale.Code" selected="@IsCurrentCulture(locale.Code)">
            @locale.NativeName
        </option>
    }
</select>

@code {
    private static readonly LocaleDefinition[] SupportedCultures =
        AppLocales.SupportedLanguages
            .Select(LocaleCatalog.Get)
            .ToArray();

    private string CurrentLanguage =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

    private static bool IsCurrentCulture(string code)
    {
        var current = CultureInfo.CurrentUICulture.Name;
        return string.Equals(code, current, StringComparison.OrdinalIgnoreCase)
            || current.StartsWith(code + "-", StringComparison.OrdinalIgnoreCase);
    }

    private async Task OnLanguageChanged(ChangeEventArgs e)
    {
        var culture = e.Value?.ToString();
        if (string.IsNullOrEmpty(culture)) return;

        var locale = LocaleCatalog.Get(culture);
        await JS.InvokeVoidAsync("letterLoom.setCulture", culture);
        await JS.InvokeVoidAsync("letterLoom.setDirection",
            locale.IsRightToLeft, locale.Code);

        // Vollständiger Reload, damit die Kultur vor dem ersten Render gesetzt wird.
        Nav.NavigateTo(Nav.Uri, forceLoad: true);
    }
}
```

**`forceLoad: true`** ist hier kein Versehen, sondern notwendig: Ohne vollen Reload rendert Blazor die Komponenten mit der **alten** `CurrentUICulture` weiter. Nur ein Neustart des WASM-Hosts führt durch den `Main()`-Code, der die Kultur vor `RunAsync()` setzt.

### Was in WASM **nicht** funktioniert

- **`builder.Services.AddLocalization(o => o.ResourcesPath = "Resources")`** - nur relevant, wenn deine `.resx` in einem Unterordner liegen. Bei `RclShared/Localization/Resources.resx` direkt neben `Resources.cs` brauchst du das nicht.
- **`UseRequestLocalization(...)`** - existiert in WASM nicht. Der Aufruf würde nicht kompilieren.
- **`CookieRequestCultureProvider`** - ASP.NET-Core-spezifisch. In WASM übernimmt das JS-Modul die Persistenz.
- **`Accept-Language`-Header lesen** - geht in WASM nur über `navigator.language` (via JS-Interop), nicht über HTTP-Header.

## WPF (inkl. BlazorWebView): Kultur im `App`-Konstruktor setzen

WPF hat - genau wie Blazor WASM - **keine `RequestLocalizationOptions`**. Die Kultur wird direkt über `CultureInfo` gesetzt. Der korrekte Ort dafür ist der **`App`-Konstruktor**, weil zu diesem Zeitpunkt noch kein `IStringLocalizer` aufgelöst wurde und die `MainWindow`-Erzeugung noch aussteht.

### Warum der Konstruktor, nicht `OnStartup`

`Application_Startup` läuft **nach** dem Konstruktor. Wenn du in `OnStartup` die Kultur setzt, hat der DI-Container zwar die Services gebaut, aber die `IStringLocalizer<Resources>`-Instanzen können bereits mit der Default-Kultur erzeugt worden sein. Das robusteste Muster: Kultur im **Konstruktor** setzen, direkt nach `BuildServiceProvider()`.

### Korrektes Muster

```csharp
using System.Globalization;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using RclShared.Localization;

namespace NetAi.LetterLoom;

public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider = null!;

    public App()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Culture MUSS vor dem ersten Zugriff auf IStringLocalizer gesetzt sein.
        ApplyCulture(LoadCulture());
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // ---- Localization ----
        services.AddLocalization();

        // ---- Blazor WebView ----
        services.AddWpfBlazorWebView();
#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif

        // ---- App-Services ----
        services.AddSingleton(_ => AppSettings.Load());
        // ... weitere Services ...

        services.AddSingleton<MainWindow>();
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    // ============================================================
    // Culture
    // ============================================================

    /// <summary>
    /// Ermittelt die zu verwendende Kultur in dieser Reihenfolge:
    /// 1. Gespeicherte Kultur aus den AppSettings - nur wenn unterstützt.
    /// 2. OS-Kultur - nur wenn unterstützt (de-AT, en-GB, ... via Parent-Kette).
    /// 3. <see cref="AppLocales.DefaultCulture"/> als letzter Anker.
    /// </summary>
    private static CultureInfo LoadCulture()
    {
        try
        {
            var settings = AppSettings.Load();
            if (AppLocales.IsSupported(settings.Culture))
            {
                return CultureInfo.GetCultureInfo(settings.Culture);
            }
        }
        catch
        {
            /* fallback */
        }

        var osCulture = CultureInfo.CurrentUICulture;
        if (AppLocales.IsSupported(osCulture.Name))
        {
            return osCulture;
        }

        return CultureInfo.GetCultureInfo(AppLocales.DefaultCulture);
    }

    private static void ApplyCulture(CultureInfo culture)
    {
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }
}
```

### Warum die Validierung wichtig ist

Im naiven Ansatz steht `new CultureInfo(settings.Culture)` ohne Prüfung. Das Problem: `new CultureInfo("fr-FR")` wirft nicht - es erzeugt einfach eine französische `CultureInfo`. Der `IStringLocalizer<Resources>` sucht dann `Resources.fr-FR.resx` -> nicht vorhanden -> fällt auf `Resources.resx` (invariant) zurück -> **stiller Fehler**: Der Nutzer sieht plötzlich englische Texte, obwohl das UI deutsch sein sollte.

Mit `AppLocales.IsSupported(settings.Culture)` greift stattdessen sauber der Default. Kein stiller Fallback mehr.

Dasselbe gilt für die OS-Kultur: Ein französisches Windows liefert `fr-FR` als `CurrentUICulture`. Ohne Validierung würde die App auf Englisch umschalten, obwohl sie nur `de` und `en` unterstützt.

### Sprachwechsel zur Laufzeit

In WPF gibt es zwei Ansätze, um die Sprache zur Laufzeit zu wechseln:

**A) Neustart (empfohlen für BlazorWebView)**
Der Sprachwähler schreibt die neue Kultur in `AppSettings` und ruft `Application.Current.Shutdown()` auf. Beim nächsten Start greift `LoadCulture` den neuen Wert. Das ist robust, weil der Blazor-WebView-Host ohnehin neu initialisiert werden muss, um die Kultur korrekt zu übernehmen.

**B) Kultur neu setzen + Re-Render**
Für reine WPF-Apps (ohne BlazorWebView) kann man `CultureInfo.DefaultThreadCurrentUICulture` zur Laufzeit neu setzen und die UI über `INotifyPropertyChanged` oder einen `ResourceDictionary`-Wechsel aktualisieren. Bei BlazorWebView ist das deutlich fummeliger, weil der WebView-Prozess die Kultur beim Start cacht.

Für BlazorWebView-Projekte ist **Neustart** die pragmatische Lösung:

```csharp
private async Task RestartAppAsync()
{
    var exePath = Environment.ProcessPath;
    if (string.IsNullOrWhiteSpace(exePath)) return;

    try
    {
        Process.Start(new ProcessStartInfo { FileName = exePath, UseShellExecute = true });
        Application.Current.Shutdown();
    }
    catch (Exception ex)
    {
        await ToastAsync($"Restart failed: {ex.Message}", "error");
    }
}
```

`Environment.ProcessPath` ist der richtige Weg (nicht `Assembly.GetExecutingAssembly().Location`), weil bei Single-File-Publish letzteres leer sein kann.

### RTL in WPF anwenden

Nach dem Neustart muss die Richtung **auf zwei Ebenen** gesetzt werden:

**Ebene 1: HTML-Dokument (BlazorWebView)** - in `MainLayout.razor`:

```csharp
@inject IJSRuntime JS

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        var culture = CultureInfo.CurrentUICulture;
        var locale = LocaleCatalog.Find(culture.Name);

        try
        {
            await JS.InvokeVoidAsync("letterLoom.setDirection",
                locale?.IsRightToLeft ?? false,
                locale?.Code ?? "en");
        }
        catch { /* JS nicht bereit - LTR bleibt */ }
    }
}
```

**Ebene 2: WPF-Shell (falls eigene Menüs/Chrome vorhanden)** - in `MainWindow.xaml.cs`:

```csharp
var culture = CultureInfo.CurrentUICulture;
var locale = LocaleCatalog.Find(culture.Name);
FlowDirection = locale?.IsRightToLeft == true
    ? FlowDirection.RightToLeft
    : FlowDirection.LeftToRight;
```

Wenn die WPF-Shell nur ein nackter Frame für die BlazorWebView ist, reicht Ebene 1.

### Was in WPF **nicht** funktioniert

- **`UseRequestLocalization(...)`** - existiert nur in ASP.NET Core.
- **`RequestLocalizationOptions`** - gehört zur ASP.NET-Core-Middleware, nicht zur BCL.
- **`IStringLocalizer` mit `CultureInfo.CurrentUICulture`** - funktioniert, aber nur, wenn die Kultur auf dem **UI-Thread** gesetzt ist (`Thread.CurrentThread.CurrentUICulture`), nicht nur `DefaultThreadCurrentUICulture`. Deshalb setzt `ApplyCulture` beide.
- **`CultureInfo.GetCultureInfo("de")` und dann Datum formatieren** - ergibt invariantes Format, weil neutrale Kulturen keine regionsspezifischen Formate haben. Deshalb `AppLocales.DefaultCulture = "de-DE"` (regionsspezifisch) und nicht `"de"`.

## API-Übersicht

| Typ | Zweck |
|---|---|
| `LocaleDefinition` | POCO mit `Code`, `NativeName`, `EnglishName`, `Script`, `IsRightToLeft`, `PluralCategories`, `CurrencyCode`, `CurrencySymbol`, `DefaultTimeZoneId` |
| `LocaleCatalog` | Statischer Katalog: `All`, `Get(code)`, `Find(code)`, `TryGet(code, out)` |
| `LocaleFallbackResolver` | Löst `de-AT` -> `de` -> `en` auf, gefiltert auf tatsächlich im Katalog vorhandene Codes |
| `PluralCategory` | CLDR-Enum (`Zero`, `One`, `Two`, `Few`, `Many`, `Other`) |
| `LocaleExtensions` | `GetCulture()`, `GetRegion()` als Extension Methods auf `LocaleDefinition` |

### `LocaleCatalog`

```csharp
// Alle 79 Locales in Deklarationsreihenfolge (nach Region gruppiert)
IReadOnlyList<LocaleDefinition> all = LocaleCatalog.All;

// Lookup, wirft bei unbekanntem Code
LocaleDefinition de = LocaleCatalog.Get("de");

// Lookup mit null bei unbekanntem Code
LocaleDefinition? maybe = LocaleCatalog.Find("xx-YY");

// Lookup mit bool-Rückgabe
if (LocaleCatalog.TryGet("pt-BR", out var ptBr)) { /* ... */ }
```

Lookup ist **case-insensitive** (`de`, `DE`, `De` liefern dasselbe).
`LocaleCatalog.All` ist eine `IReadOnlyList` - mehrfaches Enumerieren ist
günstig, die Liste wird nicht neu aufgebaut.

### `LocaleDefinition` und CultureInfo

`CultureInfo` und `RegionInfo` sind **nicht** als Properties am Record,
sondern als Extension Methods. Dadurch bleibt `LocaleDefinition`
serialisierbar (JSON, EF Core) und AOT-freundlich.

```csharp
var locale = LocaleCatalog.Get("de");

CultureInfo culture = locale.GetCulture();
// -> CultureInfo("de"), IsNeutralCulture == true

RegionInfo? region = locale.GetRegion();
// -> null, weil "de" eine neutrale Kultur ist

var ptBr = LocaleCatalog.Get("pt-BR");
RegionInfo? brRegion = ptBr.GetRegion();
// -> RegionInfo("BR"), CurrencySymbol == "R$"
```

**Achtung:** `RegionInfo` akzeptiert **keine** neutralen Kulturen
(`de`, `en`, `fr`, `pt`, ...). `GetRegion()` gibt in diesem Fall `null`
zurück. Wer Währung oder Zeitzone braucht, nutzt die Properties
`CurrencyCode`, `CurrencySymbol` und `DefaultTimeZoneId` direkt - diese
sind bewusst auch für neutrale Locales gesetzt.

### `LocaleFallbackResolver`

```csharp
IReadOnlyList<string> Resolve(string requestedCode, string? defaultCode = "en")
```

Beispiel:

| Input | Katalog enthält | Ergebnis |
|---|---|---|
| `"de"` | `de`, `en` | `["de", "en"]` |
| `"de-AT"` | `de`, `en` | `["de", "en"]` (de-AT nicht im Katalog) |
| `"pt-BR"` | `pt`, `pt-BR`, `en` | `["pt-BR", "en"]` |
| `"pt-PT"` | `pt`, `pt-BR`, `en` | `["pt", "en"]` |
| `"xx-YY"` | - | `["en"]` |
| `"xx-YY"` mit `defaultCode: null` | - | `[]` |

Der Resolver nutzt die `CultureInfo.Parent`-Kette der BCL
(`de-AT` -> `de` -> invariant) und filtert jeden Schritt gegen den
Katalog. Am Ende wird der Default angehängt, falls er im Katalog existiert.

**Bewusste Einschränkung:** `Resolve("pt")` liefert `["pt", "en"]`, nicht
`["pt", "pt-BR", "en"]`. „Welche regionale Variante ist der richtige
Fallback für eine neutrale Sprache?“ ist eine App-Policy-Frage, keine
generische. Wenn du das brauchst, gehört ein explizites Mapping in
`AppLocales`, nicht in die Lib.

## Welche Sprachen soll meine Anwendung unterstützen?

Die Lib liefert Metadaten für 79 Locales. Die Anwendung wählt daraus aus.
Das ist eine Produktentscheidung - hier eine Entscheidungshilfe.

### Entscheidungskriterien

1. **Wer sind die Nutzer?** Sprachen, in denen die tatsächlichen
   Arbeitsdokumente (z. B. Stellenanzeigen, CVs) vorkommen. Empirisch aus
   Logs / Analytics bestimmen, nicht raten.

2. **Wer sind die Kunden?** B2B-Kunden fragen nach den Sprachen, in denen
   ihre eigenen Mitarbeiter arbeiten. Ein deutscher Mittelständler braucht
   selten Japanisch, ein internationaler Konzern schon.

3. **Was kostet eine Sprache wirklich?** Nicht die `.resx`-Pflege (bei
   sauberem Setup günstig), sondern:
   - **Übersetzungsqualität** - bei `ja`, `ko`, `ar`, `hi` ist reine
     Maschinenübersetzung riskant. Höflichkeitsform ist in geschäftlichen
     Texten ein Qualitätsmerkmal.
   - **QA** - Layout (DE +30 % Textlänge, JA/KO kompakt, RTL-Spiegelung),
     Datums- und Zahlenformate, Pluralregeln.
   - **Satellite-Assembly-Größe** - jede Sprache ist eine eigene DLL.
     Bei 79 Sprachen wächst ein NuGet-Paket spürbar.

4. **Was ist die Fallback-Sprache?** `en` ist Standard. Das ist
   akzeptabel, solange die englische Version vollständig ist.

### Empfohlene Stufen

Die Stufen sind eine Entscheidungshilfe, keine Vorschrift. Der Katalog
enthält alle Codes - die Stufen helfen nur bei der Auswahl, welche
`.resx`-Dateien du tatsächlich anlegst.

#### Stufe 1 - Core

```json
["en", "de", "fr", "es", "it", "pt-BR", "nl", "pl"]
```

Westeuropa und wichtigste Märkte. Übersetzungsqualität gut verfügbar,
QA-Aufwand überschaubar.

**Achtung:** `pl` hat vier Pluralkategorien (`One`, `Few`, `Many`, `Other`).
Wenn du `pl` in Stufe 1 aufnimmst, brauchst du Plural-Logik von Anfang an -
oder du schiebst `pl` bewusst auf Stufe 2.

#### Stufe 2 - Wachstum

```json
["tr", "ru", "uk", "ro", "cs", "sv", "da", "nb", "fi", "el", "hu", "bg", "sk", "hr"]
```

Osteuropa und Nordics. Für EU-Recruiting meist relevant, sobald Kunden
aus diesen Märkten kommen.

#### Stufe 3 - Nach Bedarf

```json
["ar", "he", "zh", "zh-Hant", "ja", "ko", "hi", "id", "vi", "th"]
```

Nicht-lateinische Schriften, teils RTL, teils stark abweichende Textlängen.
Sinnvoll erst, wenn ein konkreter Kunde oder Markt das fordert. Ab hier
lohnt professionelle Übersetzung statt Maschinenübersetzung.

#### Nicht ins Startset

`lb`, `ca`, `eu`, `gl`, `is`, `ga`, `mt`, `cy`, `af`, `sw`, `am`, `ha`,
`yo`, `ig`, `zu`, `bn`, `ur`, `ta`, `te`, `mr`, `gu`, `kn`, `ml`, `pa`,
`si`, `ne`, `ms`, `tl`, `km`, `lo`, `my`, `az`, `kk`, `uz`, `ka`, `hy`,
`mn`, `be`, `mk`, `bs`, `sq`, `fa`.

Nur auf konkrete Anforderung. Für ein europäisches B2B-Tool praktisch
nie nötig.

### Faustregeln

- **Lieber 8 Sprachen mit 100 % Abdeckung als 25 mit 70 %.**
  Fehlende Keys sind schlimmer als fehlende Sprachen.
- **Regionale Varianten nur bei echten Textunterschieden.**
  `de-AT` erbt von `de` - eine eigene `.resx` nur, wenn österreichische
  Begriffe tatsächlich abweichen.
- **Fallback-Kette dokumentieren.** `de-AT` -> `de` -> `en` ist die
  Standard-Erwartung; Abweichungen gehören ins README der Anwendung.
- **Sprache entfernen ist ein Breaking Change.** Wenn ein Code aus
  `AppLocales.SupportedLanguages` verschwindet, brechen Cookie-,
  `localStorage`- und `AppSettings`-Werte von Bestandsnutzern.
  Hinzufügen ist dagegen unkritisch.

## Design-Entscheidungen

- **Katalog ≠ App-Policy.** Die Lib liefert Metadaten für alle bekannten
  Locales; die Anwendung entscheidet, welche sie ausliefert. Der Katalog
  ist Vorrat, nicht Auswahl.
- **Flaggen sind App-Policy, nicht Linguistik.** `LocaleDefinition` hat
  bewusst kein `Flag`-Property. Eine Flagge repräsentiert ein Land, nicht
  eine Sprache - `ar` -> 🇸🇦 ist eine Wahl, keine Metainformation. Deshalb
  lebt die Flaggen-Map in `AppLocales`.
- **Keine ASP.NET-Core-Abhängigkeit.** `RequestLocalizationOptions` und
  Middleware gehören in die Anwendung. Die Lib liefert nur Daten und
  Fallback-Logik.
- **Keine `.resx`-Dateien in der Lib.** Ressourcen gehören zur Anwendung.
  Die Lib weiß nichts von `IStringLocalizer`.
- **BCP-47 statt ISO 639-1.** `pt-BR`, `pt`, `zh`, `zh-Hant` sind
  unterschiedliche Sprachen im Sinne der Lokalisierung; die Zwei-Buchstaben-
  Codes reichen nicht.
- **POCO + Extension Methods.** `LocaleDefinition` ist ein reines Record
  ohne `CultureInfo`-Property. `GetCulture()` / `GetRegion()` sind
  Extension Methods - das hält den Typ serialisierbar und AOT-freundlich.
- **`RegionInfo` ist nullable.** `RegionInfo` akzeptiert keine neutralen
  Kulturen. Für `de`, `en`, `fr`, `pt` gibt `GetRegion()` daher `null`
  zurück; Währung und Zeitzone sind über eigene Properties gesetzt.
- **`All` ist `IReadOnlyList`, nicht `IEnumerable`.** Consumer dürfen
  mehrfach enumerieren, ohne dass die Liste neu aufgebaut wird.
- **`FrozenDictionary` für Lookup.** O(1)-Lookup, read-only,
  AOT-freundlich.
- **`DefaultCulture` ist regionsspezifisch.** `"de-DE"` statt `"de"`,
  damit Datums- und Zahlenformatierung korrekt ist. Der
  `IStringLocalizer` fällt automatisch auf `Resources.de.resx` zurück,
  wenn `Resources.de-DE.resx` nicht existiert.

## Was die Bibliothek nicht enthält

Bewusst nicht enthalten, weil anwendungsspezifisch:

- `.resx`-Dateien und `IStringLocalizer`-Wiring
- `RequestLocalizationOptions`-Konfiguration (ASP.NET Core)
- Middleware (`UseRequestLocalization`)
- JS-Interop für `localStorage` und `setDirection` (Blazor WASM)
- `App`-Konstruktor-Wiring und Prozess-Neustart (WPF)
- Flaggen-Map (gehört in `AppLocales`)
- Übersetzungs-Tooling
- Synchronisations-Checks zwischen Katalog und `.resx` (gehört als
  Startup-Check oder Test in die Anwendung)

## Versionierung

Solange die Bibliothek als Project Reference verwendet wird, ist
Versionierung unkritisch. Vor einer Veröffentlichung als NuGet-Paket
gilt:

| Änderung | Breaking? |
|---|---|
| Neue Locale in `LocaleCatalog` | Nein (additiv) |
| Locale aus `LocaleCatalog` entfernen | Ja (Consumer mit gespeicherten Codes) |
| `PluralCategories` einer Locale korrigieren | Theoretisch (Plural-Logik) |
| Neue Property an `LocaleDefinition` | Nein (bei `init`) |
| Signatur von `LocaleFallbackResolver.Resolve` ändern | Ja |

Neue Sprachen im Katalog sind **additiv** und sollten als Minor-Release
erscheinen. Entfernen oder Umbenennen von Codes ist ein Major-Release.

## Siehe auch

- [USAGE.md](USAGE.md) - warum `NativeName`, `CurrencySymbol` und
  `PluralCategories` einen konkreten Fehler verhindern und wo sie in
  echten Szenarien eingesetzt werden.

## Lizenz

[intern / noch festlegen]