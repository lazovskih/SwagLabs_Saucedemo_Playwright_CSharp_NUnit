using System.Globalization;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Utilities;

public static class Helpers
{
    /// <summary>
    /// Generates a normalized lowercase hyphen-delimited product identifier slug from a <see cref="Product"/>.
    /// </summary>
    /// <param name="product">The product object.</param>
    /// <returns>The normalized product ID string.</returns>
    public static string GetProductId(Product product)
    {
        return product.Id ?? product.Name.ToLower().Replace(" ", "-");
    }
    /// <summary>
    /// Extracts a numeric value by finding the index of the first digit,
    /// slicing the number substring to the end of the text, and evaluating
    /// any preceding minus sign.
    /// <para>
    /// Designed for predictable DOM outputs like:
    /// <list type="bullet">
    /// <item><description><c>"Tax: $1.92"</c> -> <c>1.92m</c></description></item>
    /// <item><description><c>"Estimated State Sales Tax: $1,249.92"</c> -> <c>1249.92m</c></description></item>
    /// <item><description><c>"Itemized Discount: -$15.50"</c> -> <c>-15.50m</c></description></item>
    /// </list>
    /// </para>
    /// </summary>
    /// <param name="rawText">The unparsed string extracted from the DOM.</param>
    /// <param name="options">Configurable parsing rules.</param>
    /// <returns>The extracted decimal number.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="rawText"/> is null, empty, or whitespace.</exception>
    /// <exception cref="FormatException">Thrown when no digits are found or the extracted value cannot be parsed.</exception>
    public static decimal ParseCurrencyToNumber(string rawText, CurrencyParseOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(rawText))
        {
            throw new ArgumentException(
                $"[CurrencyParsingError]: Invalid input received: '{rawText}'. Expected non-empty string.",
                nameof(rawText));
        }

        var allowNegative = options?.AllowNegative ?? false;
        var trimmedText = rawText.Trim();

        var expectedSymbol = options?.CurrencySymbol ?? "$";
        if (!string.IsNullOrEmpty(expectedSymbol) && !trimmedText.Contains(expectedSymbol, StringComparison.Ordinal))
        {
            throw new FormatException(
                $"[CurrencyParsingError]: Expected currency symbol '{expectedSymbol}' was not found in '{rawText}'.");
        }

        // 1. Locate the index of the very first digit (the delimiter pivot)
        var firstDigitIndex = trimmedText.AsSpan().IndexOfAnyInRange('0', '9');

        if (firstDigitIndex == -1)
        {
            throw new FormatException(
                $"[CurrencyParsingError]: No numeric digits found in input string: '{rawText}'.");
        }

        // 2. Slice the number substring from the first digit to the end
        // Strip commas (thousands separators) and any trailing whitespace
        var rawNumberPart = trimmedText[firstDigitIndex..].Replace(",", string.Empty).Trim();

        // 3. Inspect the prefix substring (everything prior to the first digit) for a minus sign
        var prefix = trimmedText.AsSpan(0, firstDigitIndex);
        var isNegative = allowNegative && prefix.Contains('-');

        // 4. Parse the final sanitized numeric string using invariant culture
        var candidate = isNegative ? $"-{rawNumberPart}" : rawNumberPart;

        if (!decimal.TryParse(candidate, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var parsedValue))
        {
            throw new FormatException(
                $"[CurrencyParsingError]: Failed to parse extracted substring '{rawNumberPart}' from original: '{rawText}'.");
        }

        return parsedValue;
    }
}
