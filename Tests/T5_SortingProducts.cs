using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Utilities;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Tests;

[Category("Smoke"), Category("Sorting"), Category("ProductsPage")]
[TestFixtureSource(typeof(BrowserProjectSource), nameof(BrowserProjectSource.Projects))]
public class T5_SortingProducts(BrowserProjectConfig browserProject) : BaseTest(browserProject)
{
    [Test(Description = "5.1. Sort product items by Name (A to Z)")]
    public async Task T5_1_Sort_product_items_by_Name_A_to_Z()
    {
        // Build expected product names sorted alphabetically (A to Z)
        var expectedNames = Products
            .OrderBy(product => product.Name, StringComparer.Ordinal)
            .Select(product => product.Name)
            .ToArray();

        // Select "Name (A to Z)" sort option
        await InventoryPage.SelectSortOptionAsync("az");

        // Verify displayed item names match expected alphabetical order
        await Expect(InventoryPage.GetItemName(), "Products should be sorted alphabetically from A to Z")
            .ToHaveTextAsync(expectedNames);
    }

    [Test(Description = "5.2 Sort product items by Name (Z to A)")]
    public async Task T5_2_Sort_product_items_by_Name_Z_to_A()
    {
        // Build expected product names sorted in reverse alphabetical order (Z to A)
        var expectedNames = Products
            .OrderByDescending(product => product.Name, StringComparer.Ordinal)
            .Select(product => product.Name)
            .ToArray();

        // Select "Name (Z to A)" sort option
        await InventoryPage.SelectSortOptionAsync("za");

        // Verify displayed item names match expected reverse alphabetical order
        await Expect(InventoryPage.GetItemName(), "Products should be sorted alphabetically from Z to A")
            .ToHaveTextAsync(expectedNames);
    }

    [Test(Description = "5.3 Sort product items by Price (low to high)")]
    public async Task T5_3_Sort_product_items_by_Price_Low_to_High()
    {
        // Sort products by price ascending, then by name ascending for ties
        var sortedProducts = Products
            .OrderBy(product => product.Price)
            .ThenBy(product => product.Name, StringComparer.Ordinal)
            .ToArray();

        var expectedNames = sortedProducts
            .Select(product => product.Name)
            .ToArray();

        var expectedPrices = sortedProducts
            .Select(product => FormattableString.Invariant($"${product.Price:F2}"))
            .ToArray();

        // Select "Price (low to high)" sort option
        await InventoryPage.SelectSortOptionAsync("lohi");

        // Verify displayed item names and prices match expected low-to-high order
        await Expect(InventoryPage.GetItemName(), "Products should be sorted by price from low to high")
            .ToHaveTextAsync(expectedNames);

        await Expect(InventoryPage.GetItemPrice(), "Product prices should be sorted from low to high")
            .ToHaveTextAsync(expectedPrices);
    }

    [Test(Description = "5.4 Sort product items by Price (high to low)")]
    public async Task T5_4_Sort_product_items_by_Price_High_to_Low()
    {
        // Sort products by price descending, then by name ascending for ties
        var sortedProducts = Products
            .OrderByDescending(product => product.Price)
            .ThenBy(product => product.Name, StringComparer.Ordinal)
            .ToArray();

        var expectedNames = sortedProducts
            .Select(product => product.Name)
            .ToArray();

        var expectedPrices = sortedProducts
            .Select(product => FormattableString.Invariant($"${product.Price:F2}"))
            .ToArray();

        // Select "Price (high to low)" sort option
        await InventoryPage.SelectSortOptionAsync("hilo");

        // Verify displayed item names and prices match expected high-to-low order
        await Expect(InventoryPage.GetItemName(), "Products should be sorted by price from high to low")
            .ToHaveTextAsync(expectedNames);

        await Expect(InventoryPage.GetItemPrice(), "Product prices should be sorted from high to low")
            .ToHaveTextAsync(expectedPrices);
    }
}
