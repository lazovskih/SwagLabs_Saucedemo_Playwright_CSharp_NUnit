using System.Text.Json.Serialization;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
/// <summary>
/// Product model class
/// </summary>
public sealed class Product
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }

    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; init; } = string.Empty;
}
