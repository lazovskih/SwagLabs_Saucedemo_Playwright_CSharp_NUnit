using Microsoft.Playwright;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages.Components;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Utilities;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages;

/// <summary>
/// Page Object representing the Checkout Step Two (<c>/checkout-step-two.html</c>) overview page.
/// </summary>
/// <param name="page">The Playwright page instance.</param>
public class CheckoutStepTwoPage(IPage page)
    : InternalPage(page, "/checkout-step-two.html", "Checkout: Overview")
{
    private ProductActionsComponent ProductActions { get; } = new(page);
    private readonly ILocator _summarySubtotal = page.Locator("[data-test='subtotal-label']");
    private readonly ILocator _summaryTax = page.Locator("[data-test='tax-label']");
    private readonly ILocator _summaryTotal = page.Locator("[data-test='total-label']");
    private readonly ILocator _finishButton = page.Locator("[data-test='finish']");

    /// <summary>
    /// Gets the locator matching all product item rows displayed in the checkout overview.
    /// </summary>
    public ILocator InventoryItems => ProductActions.InventoryItems;

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
    /// Retrieves the item quantity locator for the specified product, or all item quantities when <paramref name="product"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="product">Optional product data object.</param>
    /// <returns>Product item quantity locator.</returns>
    public ILocator GetItemQuantity(Product? product = null) => ProductActions.GetItemQuantity(product);

    /// <summary>
    /// Opens the selected product's detail page by clicking its item name link.
    /// </summary>
    /// <param name="product">Target product data object.</param>
    public Task OpenItemPageAsync(Product product) => ProductActions.GetItemName(product).ClickAsync();

    /// <summary>
    /// Retrieves and parses the summary subtotal into a numeric value.
    /// </summary>
    /// <returns>Numeric value of the subtotal.</returns>
    public async Task<decimal> GetSubtotalAsync()
    {
        var rawText = await _summarySubtotal.InnerTextAsync();
        return Helpers.ParseCurrencyToNumber(rawText);
    }

    /// <summary>
    /// Retrieves and parses the summary tax into a numeric value.
    /// </summary>
    /// <returns>Numeric value of the tax amount.</returns>
    public async Task<decimal> GetTaxAsync()
    {
        var rawText = await _summaryTax.InnerTextAsync();
        return Helpers.ParseCurrencyToNumber(rawText);
    }

    /// <summary>
    /// Retrieves and parses the summary total into a numeric value.
    /// </summary>
    /// <returns>Numeric value of the total amount.</returns>
    public async Task<decimal> GetTotalAsync()
    {
        var rawText = await _summaryTotal.InnerTextAsync();
        return Helpers.ParseCurrencyToNumber(rawText);
    }

    /// <summary>
    /// Completes the order by clicking the finish button.
    /// </summary>
    public Task FinishOrderAsync() => _finishButton.ClickAsync();
}
