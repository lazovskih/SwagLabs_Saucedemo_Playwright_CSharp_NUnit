namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
/// <summary>
/// Represents the locale settings.
/// </summary>
public sealed class Locale
{
    /// <summary>Currency Symbol.</summary>
    public string currencySymbol { get; init; } = string.Empty;

    /// <summary>Tax rate.</summary>
    public decimal TaxRate { get; init; } = 0m;
}
