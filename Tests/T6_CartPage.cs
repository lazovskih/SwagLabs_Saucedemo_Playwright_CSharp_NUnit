using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Utilities;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Tests;

[Category("Smoke"), Category("CartPage")]
[TestFixtureSource(typeof(BrowserProjectSource), nameof(BrowserProjectSource.Projects))]
public class T6_CartPage(BrowserProjectConfig browserProject) : BaseTest(browserProject)
{
    [Test(Description = "6.1 Cart page: verify cart page shows correct status of selected items")]
    public async Task T6_1_Verifies_item_page_shows_correct_status_of_selected_items()
    {
        // Add product items
        Product[] productsList = [Products[0], Products[1], Products[2], Products[3]];
        await InventoryPage.AddProductsToCartAsync(productsList);

        // Open shopping cart
        await InventoryPage.ViewCartAsync();

        // Verify cart page title
        await Expect(CartPage.PageTitle, "Confirm cart page title is valid").ToHaveTextAsync(CartPage.PageTitleText);

        // Loop through specific products
        foreach (var product in productsList)
        {
            await Expect(InventoryItemPage.GetButton(product, CartButtonAction.Add), "Confirm Add button is NOT present for items removed from cart").ToBeHiddenAsync();
            await Expect(InventoryItemPage.GetButton(product, CartButtonAction.Remove), "Confirm Remove button is present for items removed from cart").ToBeVisibleAsync();
        }
    }

    [Test(Description = "6.2 Cart page: verify item page shows correct status of selected items")]
    public async Task T6_2_Verifies_item_page_shows_correct_status_of_selected_items()
    {
        // Add product items
        Product[] productsList = [Products[0], Products[1], Products[2], Products[3]];
        await InventoryPage.AddProductsToCartAsync(productsList);

        // Open shopping cart
        await InventoryPage.ViewCartAsync();

        // Verify cart page title
        await Expect(CartPage.PageTitle, "Confirm cart page title is valid").ToHaveTextAsync(CartPage.PageTitleText);

        // Loop through specific products
        foreach (var product in productsList)
        {
            // Open the product detail page for the selected product
            await CartPage.OpenItemPageAsync(product);

            await Expect(InventoryItemPage.GetButton(product, CartButtonAction.Add), "Confirm Add button is NOT present for items removed from cart").ToBeHiddenAsync();
            await Expect(InventoryItemPage.GetButton(product, CartButtonAction.Remove), "Confirm Remove button is present for items removed from cart").ToBeVisibleAsync();

            // Open shopping cart
            await InventoryPage.ViewCartAsync();
        }
    }

    [Test(Description = "6.3 Cart page: Verifies item name, quantity and description")]
    public async Task T6_3_Verifies_item_name_image_and_description()
    {
        // Add product items
        Product[] productsList = [Products[0], Products[1], Products[2], Products[3]];
        await InventoryPage.AddProductsToCartAsync(productsList);

        // Open shopping cart
        await InventoryPage.ViewCartAsync();

        // Verify cart page title
        await Expect(CartPage.PageTitle, "Confirm cart page title is valid").ToHaveTextAsync(CartPage.PageTitleText);

        // Loop through specific products
        foreach (var product in productsList)
        {
            await Expect(
                CartPage.GetItemName(product),
                $"Item name '{product.Name}' is matching the expected name")
                .ToHaveTextAsync(product.Name);

            await Expect(
                CartPage.GetItemPrice(product),
                $"Item price of '{product.Price}' is matching the expected price")
                .ToHaveTextAsync(FormattableString.Invariant($"${product.Price:F2}"));

            await Expect(
                CartPage.GetItemDescription(product),
                $"Item description of '{product.Description}' is matching the expected description")
                .ToHaveTextAsync(product.Description);

            await Expect(
                  CartPage.GetItemQuantity(product),
                  $"Item quantity of '{product.Name}' is matching the expected quantity")
                  .ToHaveTextAsync("1");
        }
    }
}
