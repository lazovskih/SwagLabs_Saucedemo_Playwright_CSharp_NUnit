namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;

/// <summary>
/// Options to configure currency parsing behavior.
/// </summary>
public sealed class CurrencyParseOptions
{
    /// <summary>
    /// Currency symbol to match. Defaults to "$".
    /// </summary>
    public string CurrencySymbol { get; init; } = "$";

    /// <summary>
    /// Allow negative currency values, e.g., -$5.00. Defaults to <see langword="false"/>.
    /// </summary>
    public bool AllowNegative { get; init; }
}
