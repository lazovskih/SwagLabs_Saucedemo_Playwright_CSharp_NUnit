using System.Text.Json;
using System.Xml.Linq;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework.Interfaces;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Pages;
using SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Utilities;
using ConfiguredBrowserType = SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Models.BrowserType;

[assembly: Parallelizable(ParallelScope.Fixtures)]
[assembly: LevelOfParallelism(4)]

namespace SwagLabs_Saucedemo_Playwright_CSharp_NUnit.Tests;

/// <summary>
/// Base test fixture that integrates Playwright's <see cref="PlaywrightTest"/> lifecycle and assertions
/// with multi-browser project configurations and shared Page Object initialization.
/// </summary>
/// <param name="browserProject">The browser project configuration injected by <see cref="Utilities.BrowserProjectSource"/>.</param>
public abstract class BaseTest(BrowserProjectConfig browserProject) : PlaywrightTest
{
    private const string RunSettingsFileName = "local.runsettings";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly BrowserProjectConfig _browserProject =
        browserProject ?? throw new ArgumentNullException(nameof(browserProject));

    /// <summary>
    /// Gets the active browser instance configured for the current <see cref="BrowserProjectConfig"/>.
    /// </summary>
    public IBrowser? Browser { get; private set; }

    /// <summary>
    /// Gets the isolated browser context created for the current test.
    /// </summary>
    public IBrowserContext Context { get; private set; } = null!;

    /// <summary>
    /// Gets the active page created for the current test.
    /// </summary>
    public IPage Page { get; private set; } = null!;

    protected BrowserConfig BrowserConfig { get; private set; } = new();
    protected Product[] Products { get; private set; } = [];
    protected ShippingData[] ShippingInfo { get; private set; } = [];
    protected Locale Locale { get; private set; } = new();
    protected LoginPage LoginPage { get; private set; } = null!;
    protected InventoryPage InventoryPage { get; private set; } = null!;
    protected InventoryItemPage InventoryItemPage { get; private set; } = null!;
    protected CartPage CartPage { get; private set; } = null!;
    protected CheckoutStepOnePage CheckoutStepOnePage { get; private set; } = null!;
    protected CheckoutStepTwoPage CheckoutStepTwoPage { get; private set; } = null!;
    protected CheckoutCompletePage CheckoutCompletePage { get; private set; } = null!;

    protected string Username { get; private set; } = string.Empty;
    protected string Password { get; private set; } = string.Empty;
    protected string UsernameLockedout { get; private set; } = string.Empty;

    /// <summary>
    /// Controls whether <see cref="Setup"/> automatically logs in as <c>standard_user</c> before each test.
    /// Override and set to <see langword="false"/> in login-specific test fixtures.
    /// </summary>
    protected virtual bool AutoLogin => true;

    /// <summary>
    /// Provides context creation options for each test's browser context.
    /// Enables video recording so videos can be saved when a test fails.
    /// </summary>
    public virtual BrowserNewContextOptions ContextOptions()
    {
        var options = new BrowserNewContextOptions();
        var envUrl = Environment.GetEnvironmentVariable("URL");
        if (!string.IsNullOrWhiteSpace(envUrl))
        {
            options.BaseURL = envUrl.TrimEnd('/');
        }

        options.RecordVideoDir = Path.Combine(GetArtifactsRootDirectory(), _browserProject.Name, "Videos");
        return options;
    }

    [OneTimeSetUp]
    public virtual void OneTimeSetup()
    {
        EnsureRunSettingsEnvironmentVariablesLoaded();

        BrowserConfig = BrowserProjectSource.Config;
        Products = LoadTestData<Product[]>("products.json");
        ShippingInfo = LoadTestData<ShippingData[]>("shipping.json");
        Locale = LoadTestData<Locale>("locale.json");

        Username = GetRequiredEnvironmentVariable("STANDARD_USER");
        Password = GetRequiredEnvironmentVariable("DEMO_PASSWORD");
        UsernameLockedout = GetRequiredEnvironmentVariable("LOCKED_OUT_USER");
    }

    [SetUp]
    public virtual async Task Setup()
    {
        Browser ??= await LaunchBrowserAsync();
        Context = await Browser.NewContextAsync(ContextOptions());
        await Context.Tracing.StartAsync(new TracingStartOptions
        {
            Title = $"{_browserProject.Name}.{TestContext.CurrentContext.Test.Name}",
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
        Page = await Context.NewPageAsync();
        InitializePageObjects();
        if (AutoLogin)
        {
            InventoryPage = await LoginPage.LoginAsync(Username, Password);
            await Expect(InventoryPage.PageTitle).ToHaveTextAsync(InventoryPage.PageTitleText);
        }
    }

   [TearDown]
    public virtual async Task Teardown()
    {
        var video = Page?.Video;
        var isFailed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;
        var collectAll = string.Equals(
            Environment.GetEnvironmentVariable("COLLECT_ALL_ARTIFACTS"),
            "true",
            StringComparison.OrdinalIgnoreCase);
        var testArtifactDir = Path.Combine(GetArtifactsRootDirectory(), _browserProject.Name);
        var safeTestName = GetSafeTestArtifactName();
        try
        {
            await CaptureScreenshotAndTraceAsync(isFailed || collectAll, testArtifactDir, safeTestName);
        }
        finally
        {
            if (Context is not null)
            {
                await Context.CloseAsync();
            }
            await HandleVideoArtifactAsync(video, isFailed, testArtifactDir, safeTestName);
            Context = null!;
            Page = null!;
        }
    }

 [OneTimeTearDown]
    public virtual async Task OneTimeTeardown()
    {
        if (Browser is not null)
        {
            await Browser.CloseAsync();
            Browser = null;
        }
    }

    /// <summary>
    /// Initializes all shared Page Object instances with the current test's <see cref="Page"/>.
    /// </summary>
    private void InitializePageObjects()
    {
        LoginPage = new LoginPage(Page);
        InventoryPage = new InventoryPage(Page);
        InventoryItemPage = new InventoryItemPage(Page);
        CartPage = new CartPage(Page);
        CheckoutStepOnePage = new CheckoutStepOnePage(Page);
        CheckoutStepTwoPage = new CheckoutStepTwoPage(Page);
        CheckoutCompletePage = new CheckoutCompletePage(Page);
    }

    /// <summary>
    /// Captures a full-page screenshot and Playwright trace ZIP before the browser context closes.
    /// </summary>
    private async Task CaptureScreenshotAndTraceAsync(bool shouldSave, string testArtifactDir, string safeTestName)
    {
        if (Context is null)
        {
            return;
        }

        if (!shouldSave)
        {
            await Context.Tracing.StopAsync();
            return;
        }

        Directory.CreateDirectory(testArtifactDir);

        if (Page is not null && !Page.IsClosed)
        {
            var screenshotPath = Path.Combine(testArtifactDir, $"{safeTestName}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = screenshotPath,
                FullPage = true
            });
            TestContext.AddTestAttachment(screenshotPath, "Page screenshot");
        }

        var tracePath = Path.Combine(testArtifactDir, $"{safeTestName}.zip");
        await Context.Tracing.StopAsync(new TracingStopOptions
        {
            Path = tracePath
        });
        TestContext.AddTestAttachment(tracePath, "Playwright trace");
    }

    /// <summary>
    /// Saves and attaches the recorded video when a test fails, or deletes the temporary video when it passes.
    /// Must be called after <see cref="IBrowserContext.CloseAsync"/> so the video file is finalized.
    /// </summary>
    private static async Task HandleVideoArtifactAsync(
        IVideo? video,
        bool isFailed,
        string testArtifactDir,
        string safeTestName)
    {
        if (video is null)
        {
            return;
        }

        if (isFailed)
        {
            Directory.CreateDirectory(testArtifactDir);
            var videoPath = Path.Combine(testArtifactDir, $"{safeTestName}.webm");
            await video.SaveAsAsync(videoPath);
            await video.DeleteAsync();
            TestContext.AddTestAttachment(videoPath, "Failure video recording");
        }
        else
        {
            await video.DeleteAsync();
        }
    }

    private static string GetArtifactsRootDirectory() =>
        Environment.GetEnvironmentVariable("PLAYWRIGHT_ARTIFACTS_DIR")
        ?? Path.Combine(TestContext.CurrentContext.WorkDirectory, "TestResults", "PlaywrightArtifacts");

    private string GetSafeTestArtifactName() =>
        string.Concat(
            $"{_browserProject.Name}_{TestContext.CurrentContext.Test.Name}"
                .Split(Path.GetInvalidFileNameChars()));

    /// <summary>
    /// Launches the browser specified by <see cref="_browserProject"/> using the <see cref="PlaywrightTest.Playwright"/>
    /// instance managed by <see cref="PageTest"/>.
    /// </summary>
    private Task<IBrowser> LaunchBrowserAsync()
    {
        var browserType = GetBrowserType();
        var launchOptions = CreateLaunchOptions();
        return browserType.LaunchAsync(launchOptions);
    }

    /// <summary>
    /// Resolves the <see cref="IBrowserType"/> from <see cref="PlaywrightTest.Playwright"/> for the current project.
    /// </summary>
    private IBrowserType GetBrowserType() => _browserProject.Browser switch
    {
        ConfiguredBrowserType.Chrome or ConfiguredBrowserType.Edge => Playwright.Chromium,
        ConfiguredBrowserType.Firefox => Playwright.Firefox,
        ConfiguredBrowserType.WebKit => Playwright.Webkit,
        _ => throw new ArgumentOutOfRangeException(
            nameof(_browserProject.Browser),
            _browserProject.Browser,
            "Unsupported browser type.")
    };

    /// <summary>
    /// Builds <see cref="BrowserTypeLaunchOptions"/> from <see cref="BrowserConfig"/> for the current browser project.
    /// Allows the <c>HEADLESS</c> environment variable to override <see cref="BrowserConfig.Headless"/> in CI pipelines.
    /// </summary>
    private BrowserTypeLaunchOptions CreateLaunchOptions()
    {
        var browserOptions = _browserProject.Browser switch
        {
            ConfiguredBrowserType.Chrome => BrowserConfig.Chrome,
            ConfiguredBrowserType.Firefox => BrowserConfig.Firefox,
            ConfiguredBrowserType.Edge => BrowserConfig.Edge,
            ConfiguredBrowserType.WebKit => BrowserConfig.WebKit,
            _ => throw new ArgumentOutOfRangeException(
                nameof(_browserProject.Browser),
                _browserProject.Browser,
                "Unsupported browser type.")
        };

        var headlessEnv = Environment.GetEnvironmentVariable("HEADLESS");
        var isHeadless = bool.TryParse(headlessEnv, out var parsedHeadless)
            ? parsedHeadless
            : BrowserConfig.Headless;

        var launchOptions = new BrowserTypeLaunchOptions
        {
            Headless = isHeadless,
            Args = browserOptions.Arguments,
            Channel = _browserProject.Browser switch
            {
                ConfiguredBrowserType.Chrome => "chrome",
                ConfiguredBrowserType.Edge => "msedge",
                _ => null
            }
        };

        if (_browserProject.Browser == ConfiguredBrowserType.Firefox && browserOptions.Preferences.Count > 0)
        {
            launchOptions.FirefoxUserPrefs = browserOptions.Preferences
                .ToDictionary(preference => preference.Key, preference => (object)preference.Value);
        }

        return launchOptions;
    }

    /// <summary>
    /// Ensures environment variables defined in <c>local.runsettings</c> are loaded into the process
    /// even when the test runner was not explicitly launched with <c>--settings local.runsettings</c>.
    /// </summary>
    private static void EnsureRunSettingsEnvironmentVariablesLoaded()
    {
        var runSettingsPath = Path.Combine(AppContext.BaseDirectory, RunSettingsFileName);
        if (!File.Exists(runSettingsPath))
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory is not null)
            {
                var candidate = Path.Combine(directory.FullName, RunSettingsFileName);
                if (File.Exists(candidate))
                {
                    runSettingsPath = candidate;
                    break;
                }
                directory = directory.Parent;
            }
        }

        if (!File.Exists(runSettingsPath))
        {
            return;
        }

        var document = XDocument.Load(runSettingsPath);
        var envElements = document
            .Root?
            .Element("RunConfiguration")?
            .Element("EnvironmentVariables")?
            .Elements();

        if (envElements is null)
        {
            return;
        }

        foreach (var element in envElements)
        {
            var key = element.Name.LocalName;
            var value = element.Value;
            if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)) && !string.IsNullOrWhiteSpace(value))
            {
                Environment.SetEnvironmentVariable(key, value);
            }
        }
    }

    /// <summary>
    /// Retrieves a required environment variable (checking both process environment and NUnit <see cref="TestContext.Parameters"/>)
    /// or throws an <see cref="InvalidOperationException"/> if missing.
    /// </summary>
    /// <param name="variableName">The environment variable name.</param>
    private static string GetRequiredEnvironmentVariable(string variableName) =>
        Environment.GetEnvironmentVariable(variableName)
        ?? TestContext.Parameters[variableName]
        ?? throw new InvalidOperationException($"{variableName} environment variable is not set.");

    /// <summary>
    /// Deserializes a JSON file from the <c>TestData</c> directory into <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Target model or array type (e.g., <see cref="BrowserConfig"/> or <c>Product[]</c>).</typeparam>
    /// <param name="fileName">File name inside <c>TestData</c>.</param>
    /// <returns>The deserialized instance of <typeparamref name="T"/>.</returns>
    protected static T LoadTestData<T>(string fileName)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "TestData", fileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"Test data file not found at '{filePath}'. Ensure CopyToOutputDirectory is set in the .csproj file.",
                filePath);
        }

        return JsonSerializer.Deserialize<T>(File.ReadAllText(filePath), JsonOptions)
            ?? throw new InvalidOperationException($"Failed to deserialize '{fileName}' into {typeof(T).Name}.");
    }
}
