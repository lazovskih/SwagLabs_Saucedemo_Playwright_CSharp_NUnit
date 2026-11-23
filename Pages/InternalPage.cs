using Microsoft.Playwright;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages;

/// <summary>
/// Represents an authenticated internal application page with shared header navigation,
/// sidebar menu controls, and shopping cart access.
/// </summary>
/// <param name="page">The Playwright page instance.</param>
/// <param name="pageUrl">Optional relative URL path for the page.</param>
/// <param name="pageTitleText">Optional expected title text for the page.</param>
public class InternalPage(IPage page, string pageUrl = "", string pageTitleText = "")
    : BasePage(page, pageUrl, pageTitleText)
{
    // Sidebar menu locators
    private readonly ILocator _mainMenuButton = page.Locator("#react-burger-menu-btn");
    private readonly ILocator _sideMenu = page.Locator(".bm-menu");
    private readonly ILocator _allItemsMenu = page.Locator("[data-test='inventory-sidebar-link']");
    private readonly ILocator _aboutMenu = page.Locator("[data-test='about-sidebar-link']");
    private readonly ILocator _logoutMenu = page.Locator("[data-test='logout-sidebar-link']");

    /// <summary>
    /// Gets the shopping cart badge locator displaying the item count.
    /// </summary>
    public ILocator CartBadge { get; } = page.Locator("[data-test='shopping-cart-badge']");

    /// <summary>
    /// Gets the shopping cart icon link locator in the header.
    /// </summary>
    public ILocator CartLink { get; } = page.Locator("[data-test='shopping-cart-link']");

    /// <summary>
    /// Opens the main sidebar menu using Playwright's <see cref="ILocator.DispatchEventAsync"/>
    /// to directly trigger the burger button's click handler (bypassing the overlapping sibling icon image)
    /// and verifies the sidebar drawer is visible.
    /// </summary>
    public async Task ClickMenuButtonAsync()
    {
        await Assertions.Expect(_mainMenuButton).ToBeVisibleAsync();
        // Because <img class="bm-icon" data-test="open-menu"> is a sibling of <button id="react-burger-menu-btn"> (not a child inside <button>),
        // and the React onClick listener is bound only to <button id="react-burger-menu-btn">, a coordinate-based simulated mouse click (ClickAsync())
        // can hit the overlapping sibling <img> element depending on browser stacking/layout timing. When the click hits <img>, the event bubbles
        // up to <div class="bm-burger-button"> and completely misses the sibling <button id="react-burger-menu-btn">, leaving .bm-menu hidden.
        await _mainMenuButton.DispatchEventAsync("click");
        await Assertions.Expect(_sideMenu).ToBeVisibleAsync();
    }

    /// <summary>
    /// Opens the sidebar menu and clicks the "Logout" link once it is interactive.
    /// </summary>
    public async Task ClickLogoutMenuAsync()
    {
        await ClickMenuButtonAsync();
        await Assertions.Expect(_logoutMenu).ToBeVisibleAsync();
        await _logoutMenu.ClickAsync();
    }

    /// <summary>
    /// Opens the sidebar menu and clicks the "All Items" link once it is interactive.
    /// </summary>
    public async Task ClickAllItemsMenuAsync()
    {
        await ClickMenuButtonAsync();
        await Assertions.Expect(_allItemsMenu).ToBeVisibleAsync();
        await _allItemsMenu.ClickAsync();
    }

    /// <summary>
    /// Opens the sidebar menu and clicks the "About" link once it is interactive.
    /// </summary>
    public async Task ClickAboutMenuAsync()
    {
        await ClickMenuButtonAsync();
        await Assertions.Expect(_aboutMenu).ToBeVisibleAsync();
        await _aboutMenu.ClickAsync();
    }

    /// <summary>
    /// Clicks the shopping cart link to navigate to the cart page.
    /// </summary>
    public Task ViewCartAsync() => CartLink.ClickAsync();

    /// <summary>
    /// Gets the numeric count displayed on the header shopping cart badge (returns 0 when hidden).
    /// </summary>
    public async Task<int> GetCartBadgeCountAsync()
    {
        if (await CartBadge.CountAsync() == 0)
        {
            return 0;
        }

        var badgeText = await CartBadge.InnerTextAsync();
        return int.TryParse(badgeText, out var itemCount) ? itemCount : 0;
    }
}
