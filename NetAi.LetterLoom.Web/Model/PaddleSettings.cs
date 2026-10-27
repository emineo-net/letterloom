namespace NetAi.LetterLoom.Web.Models;

public sealed record PaddleSettings
{
    public int SellerId { get; init; }
    public string ClientToken { get; init; } = string.Empty;
    public string PriceId { get; init; } = string.Empty;
    public string Environment { get; init; } = "sandbox";

    public bool IsConfigured =>
        SellerId > 0 && !string.IsNullOrWhiteSpace(ClientToken) &&
        !ClientToken.Contains("REPLACE_ME", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(PriceId) &&
        !PriceId.Contains("REPLACE_ME", StringComparison.OrdinalIgnoreCase);
}