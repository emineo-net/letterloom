using System.ComponentModel.DataAnnotations;

namespace RclShared.Services;

public sealed class ContactRequest
{
    [Required(ErrorMessage = "Bitte geben Sie Ihren Namen an.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Der Name muss 2-100 Zeichen lang sein.")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Bitte geben Sie Ihre E-Mail-Adresse an.")]
    [EmailAddress(ErrorMessage = "Bitte geben Sie eine gültige E-Mail-Adresse an.")]
    [StringLength(200)]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Bitte wählen Sie einen Betreff.")]
    [StringLength(150)]
    public string Subject { get; set; } = "";

    [Required(ErrorMessage = "Bitte geben Sie eine Nachricht ein.")]
    [StringLength(4000, MinimumLength = 10, ErrorMessage = "Die Nachricht muss 10-4000 Zeichen lang sein.")]
    public string Message { get; set; } = "";

    // Honeypot - muss leer bleiben (siehe Contact.razor)
    public string? Website { get; set; }

    // Zeitstempel des Formularaufrufs für Zeitprüfung
    public DateTimeOffset OpenedAt { get; set; } = DateTimeOffset.UtcNow;
}