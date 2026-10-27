using Microsoft.JSInterop;
using NetAi.LetterLoom.Web.Models;

namespace NetAi.LetterLoom.Web.Services;

public sealed class PaddleCheckoutService(IJSRuntime js, PaddleSettings settings)
{
    public bool IsConfigured => settings.IsConfigured;

    /// <summary>
    ///     Öffnet den Paddle-Checkout. Gibt false zurück, wenn die Konfiguration
    ///     unvollständig ist oder Paddle.js nicht geladen werden konnte.
    /// </summary>
    public async Task<bool> OpenCheckoutAsync()
    {
        if (!settings.IsConfigured)
        {
            return false;
        }

        return await js.InvokeAsync<bool>("letterLoom.openCheckout",
            new
            {
                sellerId = settings.SellerId,
                clientToken = settings.ClientToken,
                priceId = settings.PriceId,
                environment = settings.Environment
            });
    }
}