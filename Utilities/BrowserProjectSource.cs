using System.Text.Json;
using NUnit.Framework;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Utilities;

public static class BrowserProjectSource
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly Lazy<BrowserConfig> CachedConfig = new(LoadAndValidateConfig);

    public static BrowserConfig Config => CachedConfig.Value;

    public static IEnumerable<TestFixtureData> Projects
    {
        get
        {
            var config = Config;
            var browserFilter = Environment.GetEnvironmentVariable("BROWSER_PROJECT");
            var selectedProjects = !string.IsNullOrWhiteSpace(browserFilter)
                ? config.Projects
                    .Where(project =>
                        string.Equals(project.Name, browserFilter, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(project.Browser.ToString(), browserFilter, StringComparison.OrdinalIgnoreCase))
                    .ToArray()
                : config.Projects;

            if (selectedProjects.Length == 0)
            {
                throw new InvalidOperationException($"No browser project matched BROWSER_PROJECT='{browserFilter}'.");
            }

            return selectedProjects.Select(project => new TestFixtureData(project));
        }
    }

    private static BrowserConfig LoadAndValidateConfig()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "TestData", "browserConfig.json");
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"Browser configuration not found at '{filePath}'. Ensure TestData is copied to the output directory.",
                filePath);
        }

        var config = JsonSerializer.Deserialize<BrowserConfig>(File.ReadAllText(filePath), JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize browserConfig.json.");

        if (config.Projects.Length == 0)
        {
            throw new InvalidOperationException("At least one browser project must be configured.");
        }

        if (config.Projects.Any(project => string.IsNullOrWhiteSpace(project.Name)))
        {
            throw new InvalidOperationException("Each browser project must have a name.");
        }

        if (config.Projects.Select(project => project.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count() != config.Projects.Length)
        {
            throw new InvalidOperationException("Browser project names must be unique.");
        }

        return config;
    }
}
