using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Utilities;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Tests;

[Category("Smoke"), Category("ItemPage")]
[TestFixtureSource(typeof(BrowserProjectSource), nameof(BrowserProjectSource.Projects))]
public class T3_InventoryItemPage(BrowserProjectConfig browserProject) : BaseTest(browserProject)
{
    [Test(Description = "3.1 Inventory item page: Adds selected products to the cart and verifies cart contents")]
    public async Task T3_1_Adds_selected_products_to_the_cart_and_verifies_cart_contents()
    {
        // Verify cart link on 'Products' page has no numbers when opened first time
        await Expect(InventoryPage.CartLink, "Confirm cart link is empty on 'Products' page").ToHaveTextAsync(string.Empty);
        
        Product[] productsList = [Products[0], Products[2], Products[4]];

        foreach (var product in productsList)
        {
            // Open the product detail page for the selected product
            await InventoryPage.OpenItemPageAsync(product);
            
            // Add products to cart
            await InventoryItemPage.AddProductToCartAsync(product);

            // Return to the products page after adding the product to the cart
            await InventoryItemPage.BackToProductsAsync();
        }

        // Verify cart badge count on 'Products' page
        await Expect(InventoryPage.CartBadge, "Confirm cart badge count is valid on 'Products' page").ToHaveTextAsync(productsList.Length.ToString());

        // Open shopping cart
        await InventoryPage.ViewCartAsync();

        // Verify cart badge count on 'Shopping Cart' page
        await Expect(CartPage.CartBadge, "Confirm cart badge count is valid on 'Shopping Cart' page").ToHaveTextAsync(productsList.Length.ToString());

        // Verify cart page title
        await Expect(CartPage.PageTitle, "Confirm cart page title is valid").ToHaveTextAsync(CartPage.PageTitleText);

        // Verify total cart items count on the page
        await Expect(CartPage.InventoryItems, "Confirm total cart items count.").ToHaveCountAsync(productsList.Length);
        
        // Verify specific products present on the shopping cart page
        foreach (var product in productsList)
        {
            var productItem = CartPage.GetProductItem(product);
            await Expect(productItem, $"Product item for '{product.Name}' is present").ToHaveCountAsync(1);
        }
    }

    [Test(Description = "3.2 Inventory item page: Button changes from 'Add to cart' to 'Remove' when clicked")]
    public async Task T3_2_Button_changes_from_Add_to_cart_to_Remove_when_clicked()
    {
        // Loop through all products
        foreach (var product in Products)
        {
            // Open the product detail page for the selected product
            await InventoryPage.OpenItemPageAsync(product);

            var addToCartButton = InventoryItemPage.GetButton(product, CartButtonAction.Add);
            var removeButton = InventoryItemPage.GetButton(product, CartButtonAction.Remove);

            // Verify initial state
            await Expect(addToCartButton, $"Add button for product: '{product.Name}' is visible").ToBeVisibleAsync();
            await Expect(removeButton, $"Remove button for product: '{product.Name}' is hidden").ToBeHiddenAsync();

            // Click add to cart
            await addToCartButton.ClickAsync();

            // Verify state changed
            await Expect(addToCartButton, $"Add button for product: '{product.Name}' is hidden").ToBeHiddenAsync();
            await Expect(removeButton, $"Remove button for product: '{product.Name}' is visible").ToBeVisibleAsync();

            // Return to the products page after adding the product to the cart
            await InventoryItemPage.BackToProductsAsync();
        }
    }

    [Test(Description = "3.3 Inventory item page: Button changes from 'Remove' to 'Add to cart' when clicked")]
    public async Task T3_3_Button_changes_from_Remove_to_Add_to_cart()
    {
        // Loop through all products
        foreach (var product in Products)
        {
            // Open the product detail page for the selected product
            await InventoryPage.OpenItemPageAsync(product);

            var addToCartButton = InventoryItemPage.GetButton(product, CartButtonAction.Add);
            var removeButton = InventoryItemPage.GetButton(product, CartButtonAction.Remove);

            // Add to cart first
            await addToCartButton.ClickAsync();
            await Expect(removeButton, $"Remove button for product: '{product.Name}' is visible").ToBeVisibleAsync();

            // Click remove
            await removeButton.ClickAsync();

            // Verify state changed back
            await Expect(removeButton, $"Remove button for product: '{product.Name}' is hidden").ToBeHiddenAsync();
            await Expect(addToCartButton, $"Add button for product: '{product.Name}' is visible").ToBeVisibleAsync();

            // Return to the products page after adding the product to the cart
            await InventoryItemPage.BackToProductsAsync();
        }
    }

    [Test(Description = "3.4 Inventory item page: Cart badge updates quantity correctly when items are added and removed")]
    public async Task T3_4_Cart_badge_updates_quantity_correctly_when_items_are_added_and_removed()
    {
        var expectedCount = 0;

        // 1. Add all items and verify badge count increments
        foreach (var product in Products)
        {
            // Open the product detail page for the selected product
            await InventoryPage.OpenItemPageAsync(product);

            await InventoryItemPage.AddProductToCartAsync(product);
            expectedCount++;

            await Expect(InventoryItemPage.CartBadge, "Confirm cart badge count is valid on 'Products' page").ToHaveTextAsync(expectedCount.ToString());

            // Return to the products page after adding the product to the cart
            await InventoryItemPage.BackToProductsAsync();
        }

        // 2. Remove all but the last item and verify badge count decrements (5 -> 1)
        foreach (var product in Products[..^1])
        {
            // Open the product detail page for the selected product
            await InventoryPage.OpenItemPageAsync(product);

            await InventoryItemPage.RemoveProductAsync(product);
            expectedCount--;

            await Expect(InventoryItemPage.CartBadge, "Confirm cart badge count is valid on 'Products' page").ToHaveTextAsync(expectedCount.ToString());

            // Return to the products page after adding the product to the cart
            await InventoryItemPage.BackToProductsAsync();
        }

        // Open the product detail page for the selected product
        await InventoryPage.OpenItemPageAsync(Products[^1]);

        // 3. Remove the final item and verify the badge disappears completely
        await InventoryItemPage.RemoveProductAsync(Products[^1]);
        await Expect(InventoryItemPage.CartBadge, "Confirm cart link is hidden on 'Products' page").ToBeHiddenAsync();

        // Return to the products page after adding the product to the cart
        await InventoryItemPage.BackToProductsAsync();

        // Verify cart link on 'Products' page has no numbers when all items are removed
        await Expect(InventoryPage.CartLink, "Confirm cart link is empty on 'Products' page").ToHaveTextAsync(string.Empty);
    }

    [Test(Description = "3.5 Inventory item page: Remove button on products page should not be present for items removed from cart")]
    public async Task T3_5_Remove_button_on_products_page_should_not_be_present_for_items_removed_from_cart()
    {
        // Add product items
        Product[] productsList = [Products[0], Products[1], Products[2], Products[3]];
        await InventoryPage.AddProductsToCartAsync(productsList);

        // Open shopping cart
        await InventoryPage.ViewCartAsync();

        // Verify cart page title
        await Expect(CartPage.PageTitle, "Confirm cart page title is valid").ToHaveTextAsync(CartPage.PageTitleText);

        // Remove 2 items from the cart
        await CartPage.RemoveProductAsync(productsList[0]);
        await CartPage.RemoveProductAsync(productsList[1]);

        // Go back to products list
        await CartPage.ContinueShoppingAsync();

        // Verify that the "Remove" button is NOT present for those removed items
        for (var i = 0; i < 2; i++)
        {
            // Open the product detail page for the selected product
            await InventoryPage.OpenItemPageAsync(productsList[i]);

            await Expect(InventoryItemPage.GetButton(productsList[i], CartButtonAction.Remove), "Confirm Remove button is NOT present for items removed from cart").ToBeHiddenAsync();
            await Expect(InventoryItemPage.GetButton(productsList[i], CartButtonAction.Add), "Confirm Add button is present for items removed from cart").ToBeVisibleAsync();

            // Return to the products page after adding the product to the cart
            await InventoryItemPage.BackToProductsAsync();
        }
    }

    [Test(Description = "3.6 Inventory item page: Adds product to the cart and verifies products page")]
    public async Task T3_6_Adds_product_to_the_cart_and_verifies_products_page()
    {
        // Add product items
        Product[] productsList = [Products[0], Products[1], Products[2], Products[3]];

        // Loop through selected products
        foreach (var product in productsList)
        {
            // Open product item page
            await InventoryPage.OpenItemPageAsync(product);

            await Expect(InventoryItemPage.BackToProductsButton, "Confirm 'Inventory Item' page is opened").ToBeVisibleAsync();
            await InventoryItemPage.AddProductToCartAsync(product);


            // Return to the products page after adding the product to the cart
            await InventoryItemPage.BackToProductsAsync();

            var addToCartButton = InventoryPage.GetButton(product, CartButtonAction.Add);
            var removeButton = InventoryPage.GetButton(product, CartButtonAction.Remove);

            // Verify changed state
            await Expect(addToCartButton, $"Add button for product: '{product.Name}' is hidden").ToBeHiddenAsync();
            await Expect(removeButton, $"Remove button for product: '{product.Name}' is visible").ToBeVisibleAsync();
        }
    }

    [Test(Description = "3.7 Inventory item page: Verifies item name, image and description")]
    public async Task T3_7_Verifies_item_name_image_and_description()
    {
        // Loop through all products
        foreach (var product in Products)
        {
            // Open product item page
            await InventoryPage.OpenItemPageAsync(product);
            await Expect(InventoryItemPage.BackToProductsButton, "Confirm 'Inventory Item' page is opened").ToBeVisibleAsync();

            await Expect(
                InventoryItemPage.GetItemName(product),
                $"Item name '{product.Name}' is matching the expected name")
                .ToHaveTextAsync(product.Name);

            await Expect(
                InventoryItemPage.GetItemPrice(product),
                $"Item price of '{product.Name}' is matching the expected price")
                .ToHaveTextAsync(FormattableString.Invariant($"${product.Price:F2}"));

            await Expect(
                InventoryItemPage.GetImage(product),
                $"Item image of '{product.Name}' is matching the expected image URL")
                .ToHaveAttributeAsync("src", product.ImageUrl);

            await Expect(
                InventoryItemPage.GetItemDescription(product),
                $"Item description of '{product.Name}' is matching the expected description")
                .ToHaveTextAsync(product.Description);

            // Return to the products page for the next iteration
            await InventoryItemPage.BackToProductsAsync();
        }
    }
}
