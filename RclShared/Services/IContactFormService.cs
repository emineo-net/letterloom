namespace RclShared.Services;

public sealed record ContactResult(bool Success, string? ErrorMessage = null);

public interface IContactFormService
{
    Task<ContactResult> SendAsync(ContactRequest request, CancellationToken cancellationToken = default);
}