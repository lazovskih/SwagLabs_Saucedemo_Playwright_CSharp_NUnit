using System.Text.Json.Serialization;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BrowserType
{
    Chrome,
    Firefox,
    Edge,
    WebKit
}

public sealed class BrowserProjectConfig
{
    public string Name { get; init; } = string.Empty;
    public BrowserType Browser { get; init; }

    public override string ToString() => Name;
}

public sealed class BrowserOptionsConfig
{
    public string[] Arguments { get; init; } = [];
    public Dictionary<string, bool> Preferences { get; init; } = [];
}

public sealed class BrowserConfig
{
    public bool Headless { get; init; }
    public BrowserProjectConfig[] Projects { get; init; } = [];
    public BrowserOptionsConfig Chrome { get; init; } = new();
    public BrowserOptionsConfig Firefox { get; init; } = new();
    public BrowserOptionsConfig Edge { get; init; } = new();
    public BrowserOptionsConfig WebKit { get; init; } = new();
}
