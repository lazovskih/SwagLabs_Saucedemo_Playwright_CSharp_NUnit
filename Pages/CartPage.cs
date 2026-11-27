using Microsoft.Playwright;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages.Components;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages;

/// <summary>
/// Page Object representing the Swag Labs Shopping Cart (<c>/cart.html</c>) page.
/// </summary>
public sealed class CartPage(IPage page)
    : InternalPage(page, "/cart.html", "Your Cart")
{
    private ProductActionsComponent ProductActions { get; } = new(page);
    private readonly ILocator _checkoutButton = page.Locator("[data-test='checkout']");
    private readonly ILocator _continueShoppingButton = page.Locator("[data-test='continue-shopping']");

    /// <summary>
    /// Gets the locator matching all cart item rows on the page.
    /// </summary>
    public ILocator InventoryItems => ProductActions.InventoryItems;

    /// <summary>
    /// Returns the 'Remove' button locator for a specific product in the cart.
    /// </summary>
    /// <param name="product">Target product data object.</param>
    /// <param name="button">Cart button action type.</param>
    public ILocator GetButton(Product product, CartButtonAction button) => ProductActions.GetButton(product, button);

    /// <summary>
    /// Removes a specific product from the cart by clicking its remove button.
    /// </summary>
    /// <param name="product">Product data object to remove.</param>
    public Task RemoveProductAsync(Product product) => ProductActions.RemoveProductAsync(product);

    /// <summary>
    /// Retrieves the total count of item rows currently displayed on the page.
    /// </summary>
    public Task<int> GetItemCountAsync() => ProductActions.GetItemCountAsync();

    /// <summary>
    /// Retrieves a locator for a specific item row based on the product name, or all item rows when <paramref name="product"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="product">Optional product data object.</param>
    /// <returns>Product item locator.</returns>
    public ILocator GetProductItem(Product? product = null) => ProductActions.GetProductItem(product);

    /// <summary>
    /// Retrieves the item price locator for the specified product, or all item prices when <paramref name="product"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="product">Optional product data object.</param>
    /// <returns>Product item price locator.</returns>
    public ILocator GetItemPrice(Product? product = null) => ProductActions.GetItemPrice(product);

    /// <summary>
    /// Retrieves the item description locator for the specified product, or all item descriptions when <paramref name="product"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="product">Optional product data object.</param>
    /// <returns>Product item description locator.</returns>
    public ILocator GetItemDescription(Product? product = null) => ProductActions.GetItemDescription(product);

    /// <summary>
    /// Retrieves the item name locator for the specified product, or all item names when <paramref name="product"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="product">Optional product data object.</param>
    /// <returns>Product item name locator.</returns>
    public ILocator GetItemName(Product? product = null) => ProductActions.GetItemName(product);

    /// <summary>
    /// Opens the selected product's detail page by clicking its item name link.
    /// </summary>
    /// <param name="product">Target product data object.</param>
    public Task OpenItemPageAsync(Product product) => ProductActions.GetItemName(product).ClickAsync();

    /// <summary>
    /// Clicks the checkout button to initiate the checkout flow.
    /// </summary>
    public Task StartCheckoutAsync() => _checkoutButton.ClickAsync();

    /// <summary>
    /// Clicks the continue shopping button to return to the inventory page.
    /// </summary>
    public Task ContinueShoppingAsync() => _continueShoppingButton.ClickAsync();

    /// <summary>
    /// Retrieves the item quantity locator for the specified product, or all item quantities when <paramref name="product"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="product">Optional product data object.</param>
    /// <returns>Product item quantity locator.</returns>
    public ILocator GetItemQuantity(Product? product = null) => ProductActions.GetItemQuantity(product);
}
