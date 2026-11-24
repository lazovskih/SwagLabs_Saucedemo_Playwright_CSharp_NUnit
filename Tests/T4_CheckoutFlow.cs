using NUnit.Framework;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Utilities;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Tests;

[Category("Smoke"), Category("Checkout")]
[TestFixtureSource(typeof(BrowserProjectSource), nameof(BrowserProjectSource.Projects))]
public class T4_CheckoutFlow(BrowserProjectConfig browserProject) : BaseTest(browserProject)
{
    [Test(Description = "4.1 Completes checkout for a selected product")]
    public async Task T4_1_Completes_checkout_for_a_selected_product()
    {
        Product[] productsList = [Products[0], Products[2], Products[4]];
        var productsListCount = productsList.Length;
        var productsListCountStr = productsListCount.ToString();

        // Add products to cart
        await InventoryPage.AddProductsToCartAsync(productsList);

        // Verify cart badge count on 'Products' page
        await Expect(InventoryPage.CartBadge, "Confirm cart badge count is valid on 'Products' page")
            .ToHaveTextAsync(productsListCountStr);

        // Open shopping cart
        await InventoryPage.ViewCartAsync();

        // Verify cart page title
        await Expect(CartPage.PageTitle, "Confirm cart page title is valid")
            .ToHaveTextAsync(CartPage.PageTitleText);

        // Verify cart badge count on 'Shopping Cart' page
        await Expect(CartPage.CartBadge, "Confirm cart badge count is valid on 'Shopping Cart' page")
            .ToHaveTextAsync(productsListCountStr);

        // Verify list of cart items count
        await Expect(CartPage.InventoryItems, "Confirm list of cart items count is valid")
            .ToHaveCountAsync(productsListCount);

        // Start checkout
        await CartPage.StartCheckoutAsync();

        // Verify checkout step one page title
        await Expect(CheckoutStepOnePage.PageTitle, "Confirm page title is valid")
            .ToHaveTextAsync(CheckoutStepOnePage.PageTitleText);

        // Fill shipping information and verify values are populated
        await CheckoutStepOnePage.FillShippingInformationAsync(ShippingInfo[0]);
        await Expect(CheckoutStepOnePage.FirstNameField, "Confirm first name field is populated").ToHaveValueAsync(ShippingInfo[0].FirstName);
        await Expect(CheckoutStepOnePage.LastNameField, "Confirm last name field is populated").ToHaveValueAsync(ShippingInfo[0].LastName);
        await Expect(CheckoutStepOnePage.PostalCodeField, "Confirm postal code field is populated").ToHaveValueAsync(ShippingInfo[0].PostalCode);

        // Click continue checkout
        await CheckoutStepOnePage.ContinueCheckoutAsync();

        // Verify checkout step two page title
        await Expect(CheckoutStepTwoPage.PageTitle, "Confirm page title is valid")
            .ToHaveTextAsync(CheckoutStepTwoPage.PageTitleText);

        // Finish order
        await CheckoutStepTwoPage.FinishOrderAsync();

        // Verify checkout complete header and text
        await Expect(CheckoutCompletePage.CompleteHeader, "Confirm complete header is valid")
            .ToHaveTextAsync(CheckoutCompletePage.CompleteHeaderText);
        await Expect(CheckoutCompletePage.CompleteTextElement, "Confirm complete text is valid")
            .ToHaveTextAsync(CheckoutCompletePage.CompleteText);
    }
}
