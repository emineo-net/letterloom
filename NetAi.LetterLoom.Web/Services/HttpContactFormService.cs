using System.Net.Http.Json;
using RclShared.Services;

namespace NetAi.LetterLoom.Web.Services;

public sealed class HttpContactFormService(HttpClient http) : IContactFormService
{
    // Formspree-Endpoint eintragen (Free für kleine Volumen):
    // https://formspree.io  ->  neues Formular anlegen  ->  Endpoint kopieren
    private const string Endpoint = "https://formspree.io/f/mkjodgrr";

    public async Task<ContactResult> SendAsync(ContactRequest request, CancellationToken ct = default)
    {
        try
        {
            var payload = new { name = request.Name, email = request.Email, subject = request.Subject, message = request.Message };

            var resp = await http.PostAsJsonAsync(Endpoint, payload, ct);
            return resp.IsSuccessStatusCode ? new ContactResult(true) : new ContactResult(false, $"Serverfehler: {(int)resp.StatusCode}");
        }
        catch (Exception ex)
        {
            return new ContactResult(false, ex.Message);
        }
    }
}