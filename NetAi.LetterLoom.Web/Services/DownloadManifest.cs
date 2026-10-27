using System.Net.Http.Json;

namespace NetAi.LetterLoom.Web.Services;

public sealed record DownloadManifest(string Version, string Setup, string Portable);

public class DownloadService
{
    private readonly HttpClient _http;
    private DownloadManifest? _cached;

    public DownloadService(HttpClient http)
    {
        _http = http;
    }

    public async Task<DownloadManifest> GetManifestAsync()
    {
        if (_cached is not null)
        {
            return _cached;
        }

        try
        {
            // Cache-Buster, damit ein neuer Build sofort sichtbar ist
            var url = $"downloads/downloads.json?v={DateTime.UtcNow.Ticks}";
            _cached = await _http.GetFromJsonAsync<DownloadManifest>(url) ?? throw new InvalidOperationException("Manifest leer");
        }
        catch
        {
            _cached = new DownloadManifest("0.0.0", "", "");
        }

        return _cached;
    }
}