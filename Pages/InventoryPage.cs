using Microsoft.Playwright;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages.Components;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages;

/// <summary>
/// Page Object representing the Swag Labs Products (Inventory) page.
/// </summary>
public class InventoryPage(IPage page)
    : InternalPage(page, "/inventory.html", "Products")
{
    private ProductActionsComponent ProductActions { get; } = new(page);
    private readonly ILocator _sortSelector = page.Locator("[data-test='product-sort-container']");

    /// <summary>
    /// Gets the locator matching all inventory item rows on the page.
    /// </summary>
    public ILocator InventoryItems => ProductActions.InventoryItems;

    /// <summary>
    /// Returns the 'Add to cart' or 'Remove' button locator for a specific product.
    /// </summary>
    /// <param name="product">Target product data object.</param>
    /// <param name="button">Cart button action type.</param>
    public ILocator GetButton(Product product, CartButtonAction button) => ProductActions.GetButton(product, button);

    /// <summary>
    /// Returns the image locator for a specific product on either the Inventory page
    /// (<c>inventory-item-&lt;id&gt;-img</c>) or the Inventory Item detail page (<c>item-&lt;id&gt;-img</c>).
    /// </summary>
    /// <param name="product">Target product data object.</param>
    public ILocator GetImage(Product product) => ProductActions.GetImage(product);

    /// <summary>
    /// Adds a specified product to the shopping cart and waits for the 'Remove' button to become visible.
    /// </summary>
    /// <param name="product">Product data object to add.</param>
    public Task AddProductToCartAsync(Product product) => ProductActions.AddProductToCartAsync(product);

    /// <summary>
    /// Sequentially adds multiple products to the shopping cart.
    /// </summary>
    /// <param name="products">Array of product data objects to add.</param>
    public Task AddProductsToCartAsync(Product[] products) => ProductActions.AddProductsToCartAsync(products);

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
    /// Retrieves the item image locator for the specified product, or all item images when <paramref name="product"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="product">Optional product data object.</param>
    /// <returns>Product item image locator.</returns>
    public ILocator GetItemImage(Product? product = null) => ProductActions.GetItemImage(product);
    
    /// <summary>
    /// Opens the selected product's detail page by clicking its item name link.
    /// </summary>
    /// <param name="product">Target product data object.</param>
    public Task OpenItemPageAsync(Product product) => ProductActions.GetItemName(product).ClickAsync();

    /// <summary>
    /// Selects a sort option from the product sort dropdown by its value (e.g., <c>"az"</c>, <c>"za"</c>, <c>"lohi"</c>, <c>"hilo"</c>).
    /// </summary>
    /// <param name="sortValue">The option value attribute to select.</param>
    public Task SelectSortOptionAsync(string sortValue) => _sortSelector.SelectOptionAsync(sortValue);
}
