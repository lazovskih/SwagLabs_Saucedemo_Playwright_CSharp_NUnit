using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Utilities;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Tests;

[Category("Smoke"), Category("CartPage")]
[TestFixtureSource(typeof(BrowserProjectSource), nameof(BrowserProjectSource.Projects))]
public class T7_CartPage(BrowserProjectConfig browserProject) : BaseTest(browserProject)
{
    [Test(Description = "7.1 Completes checkout and verifies totals for multiple selected products")]
    public async Task T7_1_Completes_checkout_and_verifies_totals_for_multiple_selected_products()
    {
        Product[] productsList = [Products[0], Products[2], Products[4]];
        var productsListCount = productsList.Length;
        var productsListCountStr = productsListCount.ToString();

        // Add multiple products to cart
        await InventoryPage.AddProductsToCartAsync(productsList);

        // Verify cart badge count on 'Products' page
        await Expect(InventoryPage.CartBadge, "Confirm cart badge count is valid")
            .ToHaveTextAsync(productsListCountStr);

        // Open shopping cart
        await InventoryPage.ViewCartAsync();

        // Verify cart page title
        await Expect(CartPage.PageTitle, "Confirm page title is valid")
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
        await CheckoutStepOnePage.FillShippingInformationAsync(ShippingInfo[1]);
        await Expect(CheckoutStepOnePage.FirstNameField, "Confirm first name field is populated").ToHaveValueAsync(ShippingInfo[1].FirstName);
        await Expect(CheckoutStepOnePage.LastNameField, "Confirm last name field is populated").ToHaveValueAsync(ShippingInfo[1].LastName);
        await Expect(CheckoutStepOnePage.PostalCodeField, "Confirm postal code field is populated").ToHaveValueAsync(ShippingInfo[1].PostalCode);

        // Click continue checkout
        await CheckoutStepOnePage.ContinueCheckoutAsync();

        // Verify checkout step two page title
        await Expect(CheckoutStepTwoPage.PageTitle, "Confirm page title is valid")
            .ToHaveTextAsync(CheckoutStepTwoPage.PageTitleText);

        // 1. Verify Subtotal matches sum of product prices
        var expectedSubtotal = productsList.Sum(product => product.Price);
        Assert.That(
            await CheckoutStepTwoPage.GetSubtotalAsync(),
            Is.EqualTo(expectedSubtotal),
            "Subtotal is not correct.");

        // 2. Verify Tax matches expected tax calculation (8% of subtotal, rounded to 2 decimal places)
        var expectedTax = Math.Round(expectedSubtotal * Locale.TaxRate, 2, MidpointRounding.AwayFromZero);
        Assert.That(
            await CheckoutStepTwoPage.GetTaxAsync(),
            Is.EqualTo(expectedTax),
            "Tax is not correct.");

        // 3. Verify Total matches Subtotal + Tax
        var expectedTotal = expectedSubtotal + expectedTax;
        Assert.That(
            await CheckoutStepTwoPage.GetTotalAsync(),
            Is.EqualTo(expectedTotal),
            "Total is not correct.");
    }

    [Test(Description = "7.2 Completes checkout and verifies totals for no selected products")]
    public async Task T7_2_Completes_checkout_and_verifies_totals_for_no_selected_products()
    {
        // Open shopping cart
        await InventoryPage.ViewCartAsync();

        // Verify cart page title
        await Expect(CartPage.PageTitle, "Confirm page title is valid")
            .ToHaveTextAsync(CartPage.PageTitleText);

        // Verify cart badge is hidden on 'Shopping Cart' page
        await Expect(CartPage.CartBadge).ToBeHiddenAsync();

        // Verify cart link on 'Shopping Cart' page has no numbers when no items are in the cart
        await Expect(CartPage.CartLink, "Confirm cart link is empty on 'Shopping Cart' page")
            .ToHaveTextAsync(string.Empty);

        // Start checkout
        await CartPage.StartCheckoutAsync();

        // Verify checkout step one page title
        await Expect(CheckoutStepOnePage.PageTitle, "Confirm page title is valid")
            .ToHaveTextAsync(CheckoutStepOnePage.PageTitleText);

        // Fill shipping information and verify values are populated
        await CheckoutStepOnePage.FillShippingInformationAsync(ShippingInfo[1]);
        await Expect(CheckoutStepOnePage.FirstNameField, "Confirm first name field is populated").ToHaveValueAsync(ShippingInfo[1].FirstName);
        await Expect(CheckoutStepOnePage.LastNameField, "Confirm last name field is populated").ToHaveValueAsync(ShippingInfo[1].LastName);
        await Expect(CheckoutStepOnePage.PostalCodeField, "Confirm postal code field is populated").ToHaveValueAsync(ShippingInfo[1].PostalCode);

        // Click continue checkout
        await CheckoutStepOnePage.ContinueCheckoutAsync();

        // Verify checkout step two page title
        await Expect(CheckoutStepTwoPage.PageTitle, "Confirm page title is valid")
            .ToHaveTextAsync(CheckoutStepTwoPage.PageTitleText);

        // 1. Verify Subtotal is 0.00
        const decimal expectedSubtotal = 0m;
        Assert.That(
            await CheckoutStepTwoPage.GetSubtotalAsync(),
            Is.EqualTo(expectedSubtotal),
            "Subtotal is not correct.");

        // 2. Verify Tax is 0.00
        const decimal expectedTax = 0m;
        Assert.That(
            await CheckoutStepTwoPage.GetTaxAsync(),
            Is.EqualTo(expectedTax),
            "Tax is not correct.");

        // 3. Verify Total is 0.00
        const decimal expectedTotal = 0m;
        Assert.That(
            await CheckoutStepTwoPage.GetTotalAsync(),
            Is.EqualTo(expectedTotal),
            "Total is not correct.");
    }

    [Test(Description = "7.3 Cart page: Verifies item name, quantity and description")]
    public async Task T7_3_Verifies_item_name_image_and_description()
    {
        // Add product items
        Product[] productsList = [Products[0], Products[1], Products[2], Products[3]];
        await InventoryPage.AddProductsToCartAsync(productsList);
        var productsListCount = productsList.Length;
        string productsListCountStr = productsListCount.ToString();

        // Open shopping cart
        await InventoryPage.ViewCartAsync();

        // Verify cart page title
        await Expect(CartPage.PageTitle, "Confirm page title is valid")
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
        await CheckoutStepOnePage.FillShippingInformationAsync(ShippingInfo[1]);
        await Expect(CheckoutStepOnePage.FirstNameField, "Confirm first name field is populated").ToHaveValueAsync(ShippingInfo[1].FirstName);
        await Expect(CheckoutStepOnePage.LastNameField, "Confirm last name field is populated").ToHaveValueAsync(ShippingInfo[1].LastName);
        await Expect(CheckoutStepOnePage.PostalCodeField, "Confirm postal code field is populated").ToHaveValueAsync(ShippingInfo[1].PostalCode);

        // Click continue checkout
        await CheckoutStepOnePage.ContinueCheckoutAsync();

        // Verify checkout step two page title
        await Expect(CheckoutStepTwoPage.PageTitle, "Confirm page title is valid")
            .ToHaveTextAsync(CheckoutStepTwoPage.PageTitleText);

        // Verify cart page title
        await Expect(CheckoutStepTwoPage.PageTitle, "Confirm cart page title is valid").ToHaveTextAsync(CheckoutStepTwoPage.PageTitleText);

        // Loop through specific products
        foreach (var product in productsList)
        {
            await Expect(
                CheckoutStepTwoPage.GetItemName(product),
                $"Item name '{product.Name}' is matching the expected name")
                .ToHaveTextAsync(product.Name);

            await Expect(
                CheckoutStepTwoPage.GetItemPrice(product),
                $"Item price of '{product.Price}' is matching the expected price")
                .ToHaveTextAsync(FormattableString.Invariant($"${product.Price:F2}"));

            await Expect(
                CheckoutStepTwoPage.GetItemDescription(product),
                $"Item description of '{product.Description}' is matching the expected description")
                .ToHaveTextAsync(product.Description);

            await Expect(
              CheckoutStepTwoPage.GetItemQuantity(product),
              $"Item quantity of '{product.Name}' is matching the expected quantity")
              .ToHaveTextAsync("1");
        }
    }
}
