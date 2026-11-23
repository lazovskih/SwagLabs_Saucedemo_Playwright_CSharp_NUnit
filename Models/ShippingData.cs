namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
/// <summary>
/// Represents the shipping information for a customer.
/// </summary>
public sealed class ShippingData
{
    /// <summary>Gets the customer's first name.</summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>Gets the customer's last name.</summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>Gets the customer's postal or zip code.</summary>
    public string PostalCode { get; init; } = string.Empty;
}

