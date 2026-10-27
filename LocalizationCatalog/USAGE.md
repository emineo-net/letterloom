# USAGE - Wofür die Properties von `LocaleDefinition` konkret nützlich sind

Dieses Dokument ergänzt das [README.md](README.md). Es zeigt an drei
Properties von `LocaleDefinition` - `NativeName`, `CurrencySymbol`,
`PluralCategories` - warum die Bibliothek **nicht nur Bequemlichkeit** ist,
sondern konkrete Fehler verhindert, die man ohne sie erst bemerkt, wenn ein
Nutzer sie sieht.

Wer die Lib nur einbindet und „irgendwie Lokalisierung“ machen will, braucht
dieses Dokument nicht. Wer wissen will, warum die Properties existieren und
wo sie den Unterschied machen, findet hier die Szenarien.

---

## 1. `NativeName` - Sprachwähler und Dokument-Listen

### Das Problem

Ein Sprachwähler soll dem Nutzer die Sprache in **seiner eigenen Sprache**
zeigen. „German“ ist für einen Deutschen die falsche Bezeichnung - er sucht
„Deutsch“.

Die BCL bietet `CultureInfo.GetCultureInfo("de").NativeName`, aber das
liefert **„Deutsch (Deutschland)“** - mit Regionszusatz. Für ein
`<select>`-Element ist das zu lang. Und für neutrale Kulturen wie `en`
liefert es „English“, aber `en-GB` liefert „English (United Kingdom)“,
obwohl der Nutzer vielleicht einfach „English“ erwartet.

Ohne die Lib müsstest du entweder:
- eine eigene Tabelle pflegen („German -> Deutsch, French -> Français, ...“), oder
- `NativeName.Split(' ')[0]` verwenden - was bei `zh-Hant` („中文 (繁體)“)
  oder `sr-Cyrl` („Српски (ћирилица)“) zu falschen Ergebnissen führt.

### Was die Lib liefert

`NativeName` ist **explizit** der kurze Eigenname, ohne Regionszusatz. Der
Regionszusatz steht nur dort, wo er zur Unterscheidung nötig ist:

| Code | `NativeName` | `CultureInfo.NativeName` |
|---|---|---|
| `de` | `Deutsch` | `Deutsch (Deutschland)` |
| `pt` | `Português` | `português (Portugal)` |
| `pt-BR` | `Português (Brasil)` | `português (Brasil)` |
| `zh` | `中文` | `中文 (中国)` |
| `zh-Hant` | `繁體中文` | `中文 (繁體)` |
| `sr` | `Српски` | `српски (Србија)` |

### Szenario A - Sprachwähler

```razor
<select @onchange="OnLanguageChanged"
        dir="@(LocaleCatalog.Get(CurrentLanguage).IsRightToLeft ? "rtl" : "ltr")">
    @foreach (var code in AppLocales.SupportedLanguages)
    {
        var locale = LocaleCatalog.Get(code);
        <option value="@code" selected="@(code == CurrentLanguage)">
            @locale.NativeName
        </option>
    }
</select>
```

Zwei Vorteile in einer Zeile: **`NativeName`** liefert die richtige
Beschriftung, **`IsRightToLeft`** sagt dir, ob das Element gespiegelt werden
muss. Ohne die Lib müsstest du beides manuell pro Sprache pflegen.

### Szenario B - Liste von Dokumenten in verschiedenen Sprachen

Eine Übersicht der eingereichten CVs, die in verschiedenen Sprachen
vorliegen:

```csharp
foreach (var cv in cvs)
{
    var locale = LocaleCatalog.Find(cv.DetectedLanguage);
    var languageLabel = locale?.NativeName ?? cv.DetectedLanguage;
    Console.WriteLine($"{cv.FileName} ({languageLabel})");
}
```

Ergebnis:

```
lebenslauf_mueller.pdf (Deutsch)
cv_smith.pdf (English)
resume_garcia.pdf (Español)
```

Der Nutzer erkennt sofort, in welcher Sprache das Dokument ist - ohne dass
du eine eigene Mapping-Tabelle pflegst.

### Szenario C - Sprachauswahl im Anschreiben-Editor

Wenn der Nutzer ein Anschreiben in einer bestimmten Sprache erzeugen will,
soll die Auswahl die Sprache korrekt benennen:

```csharp
var targetLanguage = LocaleCatalog.Get(selectedCode);
var prompt = $"Erzeuge ein Anschreiben auf {targetLanguage.NativeName}.";
// -> "Erzeuge ein Anschreiben auf Polski."
//    (nicht "auf Polish" - der Nutzer soll die Sprache erkennen)
```

Für den Prompt an das LLM ist `EnglishName` übrigens besser geeignet
(„Polish“ statt „Polski“), weil die Modelle mit englischen Sprachnamen
zuverlässiger arbeiten. Beide Properties sind deshalb im Record - je nach
Kontext die richtige.

---

## 2. `CurrencySymbol` - Gehalt in Stellenanzeigen

### Das Problem

Du willst ein Gehalt anzeigen, das in einer Stellenanzeige steht. Die Anzeige
ist auf Polnisch, das Gehalt ist „8000 PLN“.

Die naheliegende Lösung: `CultureInfo.GetCultureInfo("pl-PL").NumberFormat.CurrencySymbol`
-> `"zł"`. Funktioniert. **Aber**:

- Bei **neutralen Kulturen** (`pl`, `de`, `pt`) ist das Verhalten der BCL
  **nicht zuverlässig**. `new RegionInfo("pl")` wirft eine
  `ArgumentException`. `NumberFormat.CurrencySymbol` liefert je nach
  .NET-Version und ICU-Version unterschiedliche Ergebnisse - manchmal das
  Symbol der „Heimat“-Region, manchmal einen leeren String. Das ist keine
  stabile Grundlage.
- Der `CurrencySymbol` allein reicht **nicht** für die Formatierung. Ob
  „8000 zł“ oder „zł 8000“ oder „8 000,00 zł“ hängt von der **Position** und
  den **Trennzeichen** ab - das kommt aus `CultureInfo.NumberFormat`.
  Die Lib liefert das Symbol; die Formatierung macht `CultureInfo`.

### Was die Lib liefert

`CurrencyCode` (ISO-4217) und `CurrencySymbol` sind **explizit auch für
neutrale Kulturen** gesetzt:

| Code | `CurrencyCode` | `CurrencySymbol` | Anmerkung |
|---|---|---|---|
| `de` | `EUR` | `EUR` | neutral, BCL liefert hier unzuverlässig |
| `pl` | `PLN` | `zł` | neutral, `RegionInfo("pl")` wirft |
| `pt` | `EUR` | `EUR` | Portugal |
| `pt-BR` | `BRL` | `R$` | Brasilien - eigene Locale |
| `en` | `USD` | `$` | US-zentrischer Default |
| `en-GB` | - (nicht im Katalog) | - | Consumer wählt `en-GB` bewusst, wenn GBP nötig |

Der `CurrencyCode` ist die **stabilere** Information - er ändert sich nicht
mit der ICU-Version und ist ISO-standardisiert. Der `CurrencySymbol` ist für
die Anzeige.

**Bewusste Einschränkung:** `en` -> `USD` ist eine US-zentrische
Default-Annahme. Wer eine britische oder australische App baut, sollte in
`AppLocales.SupportedLanguages` explizit `en-GB` statt `en` wählen - dann
liefert die Lib korrekt `GBP` / `£`, sobald `en-GB` im Katalog ist. Das ist
eine Policy-Entscheidung, die die Lib dem Consumer überlässt.

### Szenario A - Gehaltsanzeige in einer Stellenanzeige

```csharp
public string FormatSalary(decimal amount, string displayLanguage)
{
    // 1. Anzeigesprache auflösen (mit Fallback)
    var locale = LocaleCatalog.Find(displayLanguage)
                 ?? LocaleCatalog.Get("en");

    // 2. Formatierungskultur für Trennzeichen/Position
    var formatCulture = CultureInfo.GetCultureInfo(displayLanguage);
    if (formatCulture.IsNeutralCulture)
        formatCulture = CultureInfo.GetCultureInfo("en"); // pragmatischer Fallback

    // 3. Währungssymbol explizit setzen - nicht aus CultureInfo ableiten
    var numberFormat = (NumberFormatInfo)formatCulture.NumberFormat.Clone();
    numberFormat.CurrencySymbol = locale.CurrencySymbol ?? "¤";

    return amount.ToString("C", numberFormat);
}
```

Aufruf:

```csharp
FormatSalary(8000m, "pl");     // "8 000,00 zł"
FormatSalary(8000m, "pl-PL");  // "8 000,00 zł" (pl-PL -> pl)
FormatSalary(8000m, "en");     // "$8,000.00"
FormatSalary(8000m, "de");     // "8.000,00 EUR"
```

Ohne die Lib müsstest du bei `pl` selbst wissen, dass die Währung `PLN`/`zł`
ist. Und bei `de` das gleiche Problem. Und bei `pt` müsstest du entscheiden,
ob `EUR`/`EUR` (Portugal) oder `BRL`/`R$` (Brasilien) - die Lib setzt
`pt` -> `EUR` und `pt-BR` -> `BRL`, damit du diese Entscheidung nicht jedes
Mal neu treffen musst.

### Szenario B - Gehaltsvorstellung im Anschreiben

```csharp
var locale = LocaleCatalog.Get("pl");
var expected = $"Oczekiwania finansowe: {salary:N0} {locale.CurrencyCode}";
// -> "Oczekiwania finansowe: 8 000 PLN"
```

Der ISO-Code (`PLN`) ist hier besser als das Symbol, weil er in formellen
Anschreiben üblich ist. Beide Informationen kommen aus einem Objekt.

### Szenario C - Vergleich von Gehältern über Sprachen

Wenn der Nutzer Stellenanzeigen aus mehreren Ländern vergleicht, will er
die Gehälter **in einer einheitlichen Währung** sehen, aber mit dem
Hinweis, welche Währung die Originalanzeige hatte:

```csharp
var originalLocale = LocaleCatalog.Get(job.SourceLanguage);

Console.WriteLine($"Anzeige: {job.Salary:N0} {originalLocale.CurrencyCode}");
// -> "Anzeige: 8 000 PLN"

Console.WriteLine($"Umgerechnet: {ConvertToEur(job.Salary):N0} EUR");
// -> "Umgerechnet: 1 870 EUR"
```

Ohne die Lib müsstest du beim Einlesen der Anzeige raten, in welcher Währung
das Gehalt angegeben ist - oder du fragst den Nutzer, was bei
automatisch eingelesenen Anzeigen nicht praktikabel ist.

---

## 3. `PluralCategories` - der eigentliche Grund für die Lib

Das ist der Punkt, bei dem die Lib **nicht nur Bequemlichkeit** ist,
sondern einen echten grammatikalischen Fehler verhindert.

### Das Problem

`.resx` und `IStringLocalizer` haben **keinen** eingebauten Plural-Support.
Du musst Plurale manuell über Key-Konventionen lösen:

```xml
<data name="ApplicationsCount_One">
    <value>{0} Bewerbung</value>
</data>
<data name="ApplicationsCount_Other">
    <value>{0} Bewerbungen</value>
</data>
```

Und im Code:

```csharp
var key = count == 1 ? "ApplicationsCount_One" : "ApplicationsCount_Other";
var text = L[key, count];
```

Für Deutsch und Englisch funktioniert das. **Für Polnisch nicht.**

Polnisch hat **vier** Pluralkategorien:

| Anzahl | Kategorie | Form |
|---|---|---|
| 1 | `One` | 1 aplikacja |
| 2-4, 22-24, 32-34, ... | `Few` | 2 aplikacje |
| 5-21, 25-31, 35-41, ... | `Many` | 5 aplikacji |
| Dezimalzahlen | `Other` | 1,5 aplikacji |

Mit nur `One`/`Other` bekommt der polnische Nutzer bei „5 Bewerbungen“
**„5 aplikacja“** - grammatikalisch falsch. Bei „3 Bewerbungen“ ebenfalls.

Arabisch hat **sechs** Kategorien: `Zero`, `One`, `Two`, `Few` (3-10),
`Many` (11-99), `Other` (100+). Wenn du nur `One`/`Other` hast, bekommst du
für 0, 2, 3-10, 11-99 und 100+ jeweils die falsche Form.

Russisch, Tschechisch, Slowakisch, Kroatisch, Serbisch, Litauisch,
Lettisch, Irisch, Maltesisch - alle haben mehr als zwei Kategorien.

### Was die Lib liefert

`locale.PluralCategories` sagt dir **vorab**, welche Formen du für eine
Sprache brauchst:

```csharp
LocaleCatalog.Get("de").PluralCategories;
// -> { One, Other }                        -> 2 .resx-Einträge

LocaleCatalog.Get("pl").PluralCategories;
// -> { One, Few, Many, Other }             -> 4 .resx-Einträge

LocaleCatalog.Get("ar").PluralCategories;
// -> { Zero, One, Two, Few, Many, Other }  -> 6 .resx-Einträge
```

Das hat drei konkrete Konsequenzen.

### Konsequenz 1 - Du weißt, welche Keys du anlegen musst

Statt zu raten, welche Pluralformen Polnisch hat, liest du sie aus dem
Katalog ab. Wenn du `.resx`-Dateien für `pl` anlegst, weißt du sofort:
es braucht `_One`, `_Few`, `_Many`, `_Other` - nicht nur `_One`/`_Other`.

### Konsequenz 2 - Du kannst die Vollständigkeit prüfen

Ein Startup-Check (oder Test) kann verifizieren, dass deine `.resx`-Dateien
für jede Sprache alle nötigen Plural-Keys enthalten:

```csharp
public static class PluralKeyValidator
{
    public static IReadOnlyList<string> FindMissingPluralKeys(
        IEnumerable<string> supportedLanguages,
        Func<string, IEnumerable<string>> keysForLanguage)
    {
        var missing = new List<string>();

        foreach (var code in supportedLanguages)
        {
            var locale = LocaleCatalog.Get(code);
            var existingKeys = keysForLanguage(code).ToHashSet(StringComparer.Ordinal);

            // Konvention: ApplicationsCount_One, ApplicationsCount_Few, ...
            foreach (var category in locale.PluralCategories)
            {
                var expectedKey = $"ApplicationsCount_{category}";
                if (!existingKeys.Contains(expectedKey))
                {
                    missing.Add($"{code}: {expectedKey}");
                }
            }
        }

        return missing;
    }
}
```

Aufruf beim App-Start:

```csharp
var missing = PluralKeyValidator.FindMissingPluralKeys(
    AppLocales.SupportedLanguages,
    code => LoadResxKeys($"Resources.{code}.resx"));

if (missing.Count > 0)
{
    throw new InvalidOperationException(
        $"Fehlende Plural-Keys:\n{string.Join("\n", missing)}");
}
```

Ohne die Lib müsstest du diese Liste pro Sprache **manuell** pflegen - und
sie würde mit jeder neuen Sprache, die du hinzufügst, veralten.

### Konsequenz 3 - Du weißt, wann einfache Key-Auswahl nicht reicht

Die `count == 1 ? _One : _Other`-Logik funktioniert **nur** für Sprachen mit
genau `{One, Other}`. Für Polnisch, Arabisch, Russisch brauchst du eine
echte Plural-Selektion - entweder **ICU MessageFormat** (via
`MessageFormat.NET` oder `Pathoschild.LanguageData`) oder eine eigene
Implementierung, die die CLDR-Regeln pro Sprache kennt.

Die Lib **sagt dir, wann das der Fall ist**:

```csharp
public static string FormatApplicationsCount(
    int count, string language, IStringLocalizer<Resources> L)
{
    var locale = LocaleCatalog.Find(language) ?? LocaleCatalog.Get("en");

    // Sprachen mit {One, Other} -> einfache Auswahl reicht
    var simpleCategories = new HashSet<PluralCategory>
    {
        PluralCategory.One, PluralCategory.Other
    };

    if (locale.PluralCategories.SetEquals(simpleCategories))
    {
        var key = count == 1 ? "ApplicationsCount_One" : "ApplicationsCount_Other";
        return L[key, count];
    }

    // Komplexe Sprache: ICU MessageFormat nötig.
    // Bis das integriert ist: bewusst auf Englisch zurückfallen,
    // statt grammatikalisch falsches Polnisch/Arabisch auszuliefern.
    // (Besser als "5 aplikacja" zu zeigen.)
    return L["ApplicationsCount_Other", count];
}
```

Das ist die **ehrliche Variante**: keine vorgetäuschte Plural-Korrektheit,
sondern ein klarer Punkt im Code, an dem du entscheiden musst, ob du ICU
integrierst oder die Sprache bewusst noch nicht auslieferst. Die Lib liefert
die Information, an der diese Entscheidung hängt.

### Was die Lib **nicht** macht (und was du wissen musst)

`PluralCategories` sagt dir **welche** Kategorien eine Sprache hat, nicht
**wie** du sie zur Laufzeit auswählst. Die Zuordnung „Anzahl -> Kategorie“
ist CLDR-Logik und gehört in eine Pluralisierungs-Bibliothek wie ICU
MessageFormat.

Die Lib ist also die **Grundlage** für korrekte Pluralisierung - sie liefert
die Metadaten, die du brauchst, um entweder:

- eine einfache Sprache (DE, EN, ES, IT, ...) mit Key-Konventionen zu
  behandeln, oder
- eine komplexe Sprache (PL, AR, RU, ...) an ICU MessageFormat zu übergeben.

Ohne die Lib müsstest du diese Zuordnung für jede Sprache manuell aus der
CLDR-Dokumentation heraussuchen - und würdest sie beim nächsten Projekt
wieder heraussuchen.

### Empfohlene Reihenfolge für die Plural-Integration

1. **Starte mit `{One, Other}`-Sprachen.** Stufe 1 aus dem Haupt-README
   (`en`, `de`, `fr`, `es`, `it`, `pt-BR`, `nl`, `pl`). Achtung: `pl` ist
   in Stufe 1, hat aber vier Kategorien - entweder `pl` bewusst
   zurückstellen oder Plural-Logik für `pl` schon in Schritt 1 einbauen.
2. **Baue den Startup-Check** aus Konsequenz 2, sobald die erste
   Nicht-`{One, Other}`-Sprache dazukommt.
3. **Integriere ICU MessageFormat**, wenn eine komplexe Sprache
   dazukommt. Der Startup-Check aus Schritt 2 ist die Grundlage dafür.
4. **Teste mit echten Zahlen** pro Sprache: 0, 1, 2, 3, 5, 11, 21, 22, 25,
   100, 101, 1.5 - das deckt alle Kategorien ab.

---

## Zusammenfassung: Was die drei Properties konkret sparen

| Property | Ohne Lib | Mit Lib |
|---|---|---|
| `NativeName` | `CultureInfo.NativeName` liefert „Deutsch (Deutschland)“ - Regionszusatz muss manuell entfernt werden, bei `zh-Hant`/`sr-Cyrl` scheitert das | Kurzer Eigenname, ohne Regionszusatz, explizit gepflegt |
| `CurrencySymbol` | BCL-Verhalten für neutrale Kulturen unzuverlässig, ICU-versionsabhängig, `RegionInfo` wirft | Explizit gesetzt auch für `de`, `pl`, `pt` - stabil über .NET-Versionen |
| `PluralCategories` | Manuell aus CLDR-Doku heraussuchen; pro Sprache und pro Projekt erneut | Aus dem Katalog ablesbar; Grundlage für `.resx`-Vollständigkeits-Check und ICU-Entscheidung |

Das sind die drei Properties, bei denen die Lib **nicht nur Bequemlichkeit**
ist, sondern einen konkreten Fehler verhindert, den man ohne sie erst
bemerkt, wenn ein polnischer Nutzer „5 aplikacja“ liest.

---

## Siehe auch

- [README.md](README.md) - Installation, API-Übersicht, Plattform-spezifische Muster (ASP.NET Core, Blazor WASM, WPF)
- `LocaleDefinition.cs` - XML-Docs der Properties
- `LocaleCatalog.cs` - die 79 Locales mit ihren Metadaten