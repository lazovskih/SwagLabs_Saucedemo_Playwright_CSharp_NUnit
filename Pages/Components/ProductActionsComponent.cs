using Microsoft.Playwright;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Utilities;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages.Components;

/// <summary>
/// Reusable component encapsulating product item locators and cart actions across
/// Inventory, Inventory Item Detail, and Shopping Cart pages.
/// </summary>
/// <param name="page">The Playwright page instance.</param>
public sealed class ProductActionsComponent(IPage page)
{
    private readonly IPage _page = page ?? throw new ArgumentNullException(nameof(page));
    private readonly ILocator _itemQuantity = page.Locator("[data-test='item-quantity']");
    public const string ItemPriceSelector = "[data-test='inventory-item-price']";
    public const string ItemNameSelector = "[data-test='inventory-item-name']";
    public const string ItemDescriptionSelector = "[data-test='inventory-item-desc']";
    public const string ItemImageSelector = "[data-test='inventory-item-img']";
    public ILocator InventoryItems { get; } = page.Locator("[data-test='inventory-item']");

    /// <summary>
    /// Returns the 'Add to cart' or 'Remove' button locator for a specific product on either
    /// the Inventory list page (<c>[data-test='add-to-cart-&lt;id&gt;']</c>) or the Product Detail page (<c>[data-test='add-to-cart']</c>).
    /// </summary>
    /// <param name="product">Target product data object.</param>
    /// <param name="button">Cart button action type.</param>
    /// <returns>Button <see cref="ILocator"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="button"/> is invalid.</exception>
    public ILocator GetButton(Product product, CartButtonAction button)
    {
        ArgumentNullException.ThrowIfNull(product);

        var buttonText = button switch
        {
            CartButtonAction.Add or CartButtonAction.AddToCart => "add-to-cart",
            CartButtonAction.Remove => "remove",
            _ => throw new ArgumentOutOfRangeException(nameof(button), button, "Invalid cart button action.")
        };

        var productId = Helpers.GetProductId(product);
        return _page.Locator($"[data-test='{buttonText}-{productId}']")
            .Or(_page.Locator($"[data-test='{buttonText}']"));
    }

    /// <summary>
    /// Returns the image locator for a specific product on either the Inventory page
    /// (<c>inventory-item-&lt;id&gt;-img</c>) or the Inventory Item detail page (<c>item-&lt;id&gt;-img</c>).
    /// </summary>
    /// <param name="product">Target product data object.</param>
    public ILocator GetImage(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        var productId = Helpers.GetProductId(product);
        return _page.Locator($"[data-test='inventory-item-{productId}-img']")
            .Or(_page.Locator($"[data-test='item-{productId}-img']"));
    }

    /// <summary>
    /// Adds a specified product to the shopping cart and waits for the 'Remove' button to become visible.
    /// </summary>
    /// <param name="product">Product data object to add.</param>
    public async Task AddProductToCartAsync(Product product)
    {
        await GetButton(product, CartButtonAction.Add).ClickAsync();
        await GetButton(product, CartButtonAction.Remove)
            .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }

    /// <summary>
    /// Sequentially adds multiple products to the shopping cart.
    /// </summary>
    /// <param name="products">Array of product data objects to add.</param>
    public async Task AddProductsToCartAsync(Product[] products)
    {
        ArgumentNullException.ThrowIfNull(products);

        foreach (var product in products)
        {
            await AddProductToCartAsync(product);
        }
    }

    /// <summary>
    /// Removes a specific product from the cart by clicking its remove button.
    /// </summary>
    /// <param name="product">Product data object to remove.</param>
    public Task RemoveProductAsync(Product product) => GetButton(product, CartButtonAction.Remove).ClickAsync();

    /// <summary>
    /// Retrieves the total count of item rows currently displayed on the page.
    /// </summary>
    public Task<int> GetItemCountAsync() => InventoryItems.CountAsync();

    /// <summary>
    /// Retrieves a locator for a specific item row based on the product name, or all item rows when <paramref name="product"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="product">Optional product data object.</param>
    /// <returns>Product item locator.</returns>
    public ILocator GetProductItem(Product? product = null) =>
        product is not null
            ? InventoryItems.Filter(new LocatorFilterOptions { HasTextString = product.Name })
            : InventoryItems;

    /// <summary>
    /// Retrieves the item name locator for the specified product, or all item names when <paramref name="product"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="product">Optional product data object.</param>
    /// <returns>Product item name locator.</returns>
    public ILocator GetItemName(Product? product = null) => GetProductItem(product).Locator(ItemNameSelector);

    /// <summary>
    /// Retrieves the item price locator for the specified product, or all item prices when <paramref name="product"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="product">Optional product data object.</param>
    /// <returns>Product item price locator.</returns>
    public ILocator GetItemPrice(Product? product = null) => GetProductItem(product).Locator(ItemPriceSelector);

    /// <summary>
    /// Retrieves the item description locator for the specified product, or all item descriptions when <paramref name="product"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="product">Optional product data object.</param>
    /// <returns>Product item description locator.</returns>
    public ILocator GetItemDescription(Product? product = null) => GetProductItem(product).Locator(ItemDescriptionSelector);

    /// <summary>
    /// Retrieves the item image locator for the specified product, or all item images when <paramref name="product"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="product">Optional product data object.</param>
    /// <returns>Product item image locator.</returns>
    public ILocator GetItemImage(Product? product = null) => GetProductItem(product).Locator(ItemImageSelector);

    /// <summary>
    /// Retrieves the item quantity locator for the specified product, or all item quantities when <paramref name="product"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="product">Optional product data object.</param>
    /// <returns>Product item quantity locator.</returns>
    public ILocator GetItemQuantity(Product? product = null) => GetProductItem(product).Locator(_itemQuantity);
}
