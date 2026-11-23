using System.Text.RegularExpressions;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Utilities;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Tests;

[Category("Smoke"), Category("Login")]
[TestFixtureSource(typeof(BrowserProjectSource), nameof(BrowserProjectSource.Projects))]
public class T1_LoginPage(BrowserProjectConfig browserProject) : BaseTest(browserProject)
{
    protected override bool AutoLogin => false;

    [Test(Description = "1.1 User with valid credentials can login and see products page")]
    public async Task T1_1_User_with_valid_credentials_Can_Login_And_See_Products_Page()
    {
        var inventoryPage = await LoginPage.LoginAsync(Username, Password);

        await Expect(Page, "Verify user is on the products page").ToHaveURLAsync(new Regex($".*{Regex.Escape(inventoryPage.PageUrl)}$"));
        await Expect(inventoryPage.PageTitle, "Verify products page title is displayed").ToHaveTextAsync(inventoryPage.PageTitleText);
    }

    [Test(Description = "1.2 Login with invalid password - unsuccessful")]
    public async Task T1_2_Login_With_Invalid_Password_Unsuccessful()
    {
        await LoginPage.LoginAsync(Username, Password + "wrong");
        await Expect(LoginPage.ErrorMessage, "Verify error message is displayed.").ToContainTextAsync(LoginPage.ErrorMessageText);
    }

    [Test(Description = "1.3 Login with invalid username - unsuccessful")]
    public async Task T1_3_Login_With_Invalid_Username_Unsuccessful()
    {
        await LoginPage.LoginAsync(Username + "x", Password);
        await Expect(LoginPage.ErrorMessage, "Verify error message is displayed.").ToContainTextAsync(LoginPage.ErrorMessageText);
    }

    [Test(Description = "1.4 Locked Out User Should See Error Message")]
    public async Task T1_4_LockedOutUser_ShouldSeeErrorMessage()
    {
        await LoginPage.LoginAsync(UsernameLockedout, Password);
        await Expect(LoginPage.ErrorMessage, "Verify locked out error message is displayed.").ToContainTextAsync(LoginPage.LockedOutMessageText);
    }

    [Test(Description = "1.5 Login, logout, verify internal pages not accessible")]
    public async Task T1_5_Login_Logout_Verify_Internal_Pages_Not_Accessible()
    {
        // Login with valid credentials
        var inventoryPage = await LoginPage.LoginAsync(Username, Password);

        // Verify Products page is displayed
        await Expect(Page, "Verify user is on the products page").ToHaveURLAsync(new Regex($".*{Regex.Escape(inventoryPage.PageUrl)}$"));
        await Expect(inventoryPage.PageTitle, "Verify products page title is displayed").ToHaveTextAsync(inventoryPage.PageTitleText);

        // Click logout
        await inventoryPage.ClickLogoutMenuAsync();

        // Verify redirected to login page
        await Expect(LoginPage.PageTitle, "User should be redirected to the login page after logout.").ToHaveTextAsync(LoginPage.PageTitleText);
        await Expect(Page, "Verify user is redirected to the login page after logout.").ToHaveURLAsync(new Regex(".*/$"));

        InternalPage[] internalPages =
        [
            new InventoryPage(Page),
            new CartPage(Page),
            new CheckoutCompletePage(Page),
            new CheckoutStepOnePage(Page),
            new CheckoutStepTwoPage(Page)
        ];
        foreach (var page in internalPages)
        {
            // Try to navigate directly to internal pages 
            await page.OpenAsync();

            // Verify redirected back to login page and access-denied error is shown
            await Expect(LoginPage.PageTitle, $"'{page.PageTitleText}' page should not be accessible after logout.").ToHaveTextAsync(LoginPage.PageTitleText);
            await Expect(Page, "Verify user is redirected to the login page after logout.").ToHaveURLAsync(new Regex(".*/$"));

            var noAccessMessageText = LoginPage.NoAccessMessageText.Replace("{pageUrl}", page.PageUrl);
            await Expect(LoginPage.ErrorMessage, "Verify error message is displayed.").ToContainTextAsync(noAccessMessageText);
        }
    }
}
