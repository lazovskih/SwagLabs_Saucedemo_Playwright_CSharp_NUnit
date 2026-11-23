using Microsoft.Playwright;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages;

/// <summary>
/// Page Object representing the Checkout Step One (<c>/checkout-step-one.html</c>) customer information page.
/// </summary>
/// <param name="page">The Playwright page instance.</param>
public class CheckoutStepOnePage(IPage page)
    : InternalPage(page, "/checkout-step-one.html", "Checkout: Your Information")
{
    public readonly ILocator FirstNameField = page.Locator("[data-test='firstName']");
    public readonly ILocator LastNameField = page.Locator("[data-test='lastName']");
    public readonly ILocator PostalCodeField = page.Locator("[data-test='postalCode']");
    public readonly ILocator ContinueButton = page.Locator("[data-test='continue']");

    /// <summary>
    /// Fills the shipping information form fields (first name, last name, and postal code).
    /// </summary>
    /// <param name="shippingData">Customer shipping information object.</param>
    public async Task FillShippingInformationAsync(ShippingData shippingData)
    {
        ArgumentNullException.ThrowIfNull(shippingData);

        await FirstNameField.FillAsync(shippingData.FirstName);
        await LastNameField.FillAsync(shippingData.LastName);
        await PostalCodeField.FillAsync(shippingData.PostalCode);
    }

    /// <summary>
    /// Clicks the continue button to proceed to the checkout overview step.
    /// </summary>
    public Task ContinueCheckoutAsync() => ContinueButton.ClickAsync();
}
