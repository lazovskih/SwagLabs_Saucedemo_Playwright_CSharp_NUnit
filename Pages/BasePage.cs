using Microsoft.Playwright;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages;

/// <summary>
/// Abstract base class providing core page navigation, state verification, and element interaction helpers.
/// </summary>
public abstract class BasePage
{
    private const string DefaultTitleSelector = "[data-test='title']";

    /// <summary>
    /// Gets the underlying Playwright <see cref="IPage"/> instance.
    /// </summary>
    protected IPage Page { get; }

    /// <summary>
    /// Gets the relative URL path for the page (e.g., <c>"/inventory.html"</c>).
    /// </summary>
    public virtual string PageUrl { get; protected init; } = string.Empty;

    /// <summary>
    /// Gets the expected visible header title text for the page.
    /// </summary>
    public virtual string PageTitleText { get; protected init; } = string.Empty;

    /// <summary>
    /// Gets the locator for the page title element.
    /// </summary>
    public virtual ILocator PageTitle { get; protected init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BasePage"/> class.
    /// </summary>
    /// <param name="page">The Playwright page instance.</param>
    /// <param name="pageUrl">Optional relative URL path for the page.</param>
    /// <param name="pageTitleText">Optional expected title text for the page.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="page"/> is <see langword="null"/>.</exception>
    protected BasePage(IPage page, string pageUrl = "", string pageTitleText = "")
    {
        ArgumentNullException.ThrowIfNull(page);

        Page = page;
        PageUrl = pageUrl;
        PageTitleText = pageTitleText;
        PageTitle = Page.Locator(DefaultTitleSelector);
    }

    /// <summary>
    /// Checks whether the page is loaded by waiting for the expected URL and <see cref="PageTitle"/> visibility,
    /// then verifying that its text matches <see cref="PageTitleText"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the page title becomes visible and matches <see cref="PageTitleText"/>; otherwise, <see langword="false"/>.</returns>
    public virtual async Task<bool> IsLoadedAsync()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(PageUrl))
            {
                await Page.WaitForURLAsync($"**{PageUrl}");
            }

            await PageTitle.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible
            });

            var actualTitle = (await PageTitle.InnerTextAsync()).Trim();
            return string.Equals(actualTitle, PageTitleText, StringComparison.OrdinalIgnoreCase);
        }
        catch (TimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// Navigates to the specified relative <paramref name="url"/> (or <see cref="PageUrl"/> when omitted).
    /// </summary>
    /// <param name="url">Optional relative URL path override.</param>
    public virtual Task<IResponse?> OpenAsync(string url = "")
    {
        var path = string.IsNullOrWhiteSpace(url) ? PageUrl : url;
        var normalizedPath = string.IsNullOrEmpty(path)
            ? "/"
            : (path.StartsWith('/') ? path : $"/{path}");
        return Page.GotoAsync(normalizedPath);
    }
}
