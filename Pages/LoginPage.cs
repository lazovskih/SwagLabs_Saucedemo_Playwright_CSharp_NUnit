using Microsoft.Playwright;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages;

/// <summary>
/// Page Object representing the Swag Labs login page.
/// </summary>
public class LoginPage : BasePage
{
    private readonly ILocator _usernameField;
    private readonly ILocator _passwordField;
    private readonly ILocator _loginButton;

    public const string ErrorMessageText = "Epic sadface: Username and password do not match any user in this service";
    public const string NoAccessMessageText = "Epic sadface: You can only access '{pageUrl}' when you are logged in.";
    public const string LockedOutMessageText = "Epic sadface: Sorry, this user has been locked out.";
    public ILocator ErrorMessage ; 

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginPage"/> class.
    /// </summary>
    /// <param name="page">The Playwright page instance.</param>
    public LoginPage(IPage page) : base(page, string.Empty, "Swag Labs")
    {
        PageTitle = Page.Locator("div.login_logo");
        _usernameField = Page.Locator("[data-test='username']");
        _passwordField = Page.Locator("[data-test='password']");
        _loginButton = Page.Locator("[data-test='login-button']");
        ErrorMessage = Page.Locator("[data-test='error']");
    }

    /// <summary>
    /// Navigates to the login page and submits the provided credentials.
    /// </summary>
    /// <param name="username">The username to log in with.</param>
    /// <param name="password">The password to log in with.</param>
    /// <returns>An initialized <see cref="InventoryPage"/> instance.</returns>
    public async Task<InventoryPage> LoginAsync(string username, string password)
    {
        await OpenAsync();
        await _usernameField.FillAsync(username);
        await _passwordField.FillAsync(password);

        await _loginButton.ClickAsync();
     return   new InventoryPage(Page);
      }

    /// <summary>
    /// Retrieves the text of the login error message banner.
    /// </summary>
    /// <returns>The error message string.</returns>
    public Task<string> GetErrorMessageTextAsync() => ErrorMessage.InnerTextAsync();
}
