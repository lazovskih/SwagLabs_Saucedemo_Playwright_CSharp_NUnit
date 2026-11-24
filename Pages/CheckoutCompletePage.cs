using Microsoft.Playwright;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages;

/// <summary>
/// Page Object representing the Checkout Complete (<c>/checkout-complete.html</c>) confirmation page.
/// </summary>
/// <param name="page">The Playwright page instance.</param>
public class CheckoutCompletePage(IPage page)
    : InternalPage(page, "/checkout-complete.html", "Checkout: Complete!")
{
    private readonly ILocator _backHomeButton = page.Locator("[data-test='back-home']");

    public ILocator CompleteTextElement { get; } = page.Locator("[data-test='complete-text']");
    public ILocator CompleteHeader { get; } = page.Locator("[data-test='complete-header']");

    public readonly string CompleteHeaderText = "Thank you for your order!";
    public readonly string CompleteText = "Your order has been dispatched, and will arrive just as fast as the pony can get there!";

    /// <summary>
    /// Clicks the 'Back Home' button to return to the inventory page.
    /// </summary>
    public Task BackHomeAsync() => _backHomeButton.ClickAsync();
}
