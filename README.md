# 🛒 SauceDemo – Playwright & NUnit Automation Project (C#)

An end-to-end (E2E) test automation project for the [SauceDemo (Swag Labs)](https://www.saucedemo.com) e-commerce web application, built with **C# (.NET 8)**, **[Microsoft Playwright for .NET](https://playwright.dev/dotnet/)**, and **[NUnit](https://nunit.org/)**.

## 📹 Video Recording

> 🎬 **Watch the test execution demo on YouTube:** [https://youtu.be/Y5U61Md1cOI](https://youtu.be/Y5U61Md1cOI)

---

## 📋 About the Project

This project demonstrates automated E2E testing of key user flows on the [www.saucedemo.com](https://www.saucedemo.com) website across **Chrome**, **Firefox**, **Microsoft Edge**, and **WebKit**:

- 🔐 **User authentication (`T1_LoginPage`)** — valid/invalid login, locked-out user handling, logout, and route access control for unauthenticated users
- 🛍️ **Inventory page cart & catalog interactions (`T2_InventoryPage`)** — verifying all products are displayed, adding/removing items on the products page, verifying `Add to cart` / `Remove` button state transitions, validating shopping cart badge counts, and verifying product details (name, price, image, and description)
- 🔍 **Item details page interactions (`T3_InventoryItemPage`)** — adding and removing items directly on the product detail page, verifying shopping cart contents and cart badge count updates, confirming button state synchronization between the inventory, item details, and cart pages, and validating item details
- 💳 **Checkout flow (`T4_CheckoutFlow`)** — customer shipping details -> order summary -> order completion and confirmation
- 🔃 **Product catalog sorting (`T5_SortingProducts`)** — sorting items by name (A–Z / Z–A) and price (low-to-high / high-to-low)
- 🛒 **Shopping cart page verification (`T6_CartPage`)** — verifying button states for selected cart items, navigating from cart to product details, and validating item name, price, description, and quantity in the cart
- 🧾 **Checkout overview & totals verification (`T7_CheckoutPage`)** — verifying subtotal, tax (8%), and total calculations for multiple items and zero items, and validating product details and quantities on the checkout overview page

The tests are architected using the **Page Object Model (POM)** with **encapsulated component composition** ([`ProductActionsComponent`](Pages/Components/ProductActionsComponent.cs)), **Playwright NUnit `PlaywrightTest` integration** ([`BaseTest`](Tests/BaseTest.cs)), **parameterized multi-browser test fixtures** ([`BrowserProjectSource`](Utilities/BrowserProjectSource.cs)), and **web-first assertions** (`Expect(...)`).

### Tech Stack

| Tool / Library                                                                                      | Version | Purpose                                                            |
| --------------------------------------------------------------------------------------------------- | ------- | ------------------------------------------------------------------ |
| [.NET SDK](https://dotnet.microsoft.com/)                                                           | 8.0     | Target framework (`net8.0`, C# 12)                                 |
| [Microsoft.Playwright.NUnit](https://www.nuget.org/packages/Microsoft.Playwright.NUnit)             | 1.64.1  | Browser automation, `PlaywrightTest` runner & web-first assertions |
| [Microsoft.Playwright.TestAdapter](https://www.nuget.org/packages/Microsoft.Playwright.TestAdapter) | 1.64.1  | Playwright test adapter integration                                |
| [NUnit](https://www.nuget.org/packages/NUnit)                                                       | 5.0.0   | Test framework & parallel fixture execution                        |
| [NUnit.Analyzers](https://www.nuget.org/packages/NUnit.Analyzers)                                   | 4.15.0  | Static code analysis for NUnit tests                               |
| [NUnit3TestAdapter](https://www.nuget.org/packages/NUnit3TestAdapter)                               | 6.3.1   | Visual Studio / VS Code / `dotnet test` runner adapter             |
| [Microsoft.NET.Test.Sdk](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk)                     | 18.11.1 | MSBuild test host infrastructure                                   |
| [coverlet.collector](https://www.nuget.org/packages/coverlet.collector)                             | 6.0.0   | Code coverage collector                                            |

---

## 🛡️ Code Quality & Static Analysis

- **Roslyn Static Analysis & Code Style**: C# compiler warnings and `NUnit.Analyzers` are enforced in [`SwagLabs_Saucedemo_Playwright_CSharp_NUnit.csproj`](SwagLabs_Saucedemo_Playwright_CSharp_NUnit.csproj) and verified in CI using `dotnet build -warnaserror`, with code style rules configured in [`.editorconfig`](.editorconfig).

---

## 🔄 CI/CD Pipelines

Automated builds and E2E test runs are configured for both **GitHub Actions** and **Azure DevOps**:

### GitHub Actions ([`.github/workflows/playwright.yml`](.github/workflows/playwright.yml))

Triggers on `push`, `pull_request`, and manual `workflow_dispatch` runs (with automatic cancellation of superseded PR runs):

1. **Job 1 — `quality_gate` (`Static Analysis & Build Verification`)**:
    - Restores NuGet dependencies and runs `dotnet build --no-restore --configuration Release -warnaserror` on `ubuntu-latest` with .NET `8.0.x` to enforce zero compiler errors and zero analyzer warnings.
2. **Job 2 — `playwright_test` (`Playwright NUnit E2E Tests`)**:
    - Runs after `quality_gate` succeeds across a multi-browser matrix (`Chrome`, `Firefox`, `Edge`, `WebKit`) with `fail-fast: false`.
    - Installs `ffmpeg`, Playwright browser binaries, and OS dependencies (`playwright.ps1 install`).
    - Executes the NUnit test suite in headless mode (`HEADLESS: "true"`, `BROWSER_PROJECT: ${{ matrix.browserName }}`) using the **`TEST`** GitHub Environment secrets/variables: `STANDARD_USER`, `LOCKED_OUT_USER`, `DEMO_PASSWORD`, and `URL`.
    - Publishes `.trx` test results as GitHub Check Runs (`dorny/test-reporter@v3`), writes a Cobertura code coverage summary to the job summary (`GITHUB_STEP_SUMMARY`), and uploads all test reports, coverage XML, full-page screenshots, Playwright trace `.zip` archives, and failure video recordings (`.webm`) as `TestArtifacts-<Browser>` workflow artifacts (`actions/upload-artifact@v4`).

### Azure DevOps ([`azure-pipelines.yml`](azure-pipelines.yml))

Triggers on pushes and Pull Requests:

1. **Stage 1 — `Quality_Gate` (`Static Analysis & Build Verification`)**:
    - Restores NuGet dependencies and runs `dotnet build --no-restore --configuration Release -warnaserror` to enforce zero compiler errors and zero analyzer warnings.
2. **Stage 2 — `Playwright_Test` (`Playwright NUnit E2E Test Execution`)**:
    - Runs after `Quality_Gate` succeeds across a multi-browser matrix (`Chrome`, `Firefox`, `Edge`, `WebKit`).
    - Installs `ffmpeg`, Playwright browser binaries, and OS dependencies (`playwright.ps1 install`).
    - Executes the NUnit test suite in headless mode (`HEADLESS: "true"`, `BROWSER_PROJECT: $(browserName)`) using credentials from the **`Playwright-Test-Config`** Azure DevOps Library Variable Group.
    - Collects and publishes `.trx` test results (`PublishTestResults@2`), HTML test reports, Cobertura code coverage (`PublishCodeCoverageResults@2`), full-page screenshots, Playwright trace `.zip` archives, and failure video recordings (`.webm`) as pipeline artifacts (`PublishPipelineArtifact@1`).

---

## ⚙️ Prerequisites

- **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** (or later)
- **[PowerShell (`pwsh` / Windows PowerShell)](https://learn.microsoft.com/en-us/powershell/)** (for running Playwright's browser installer script `playwright.ps1`)
- **[Git](https://git-scm.com/)**
- Optional IDE: **Visual Studio 2022+**, **VS Code** / **Antigravity** (with **C# Dev Kit**), or **JetBrains Rider**

---

## 🚀 Project Setup

### 1. Clone the repository

```bash
git clone https://github.com/lazovskih/SwagLabs_Saucedemo_Playwright_CSharp_NUnit.git
cd SwagLabs_Saucedemo_Playwright_CSharp_NUnit
```

### 2. Configure local environment variables (`local.runsettings`)

Create a `local.runsettings` file in the project root (git-ignored) to supply test credentials for local runs:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <RunConfiguration>
    <EnvironmentVariables>
      <URL>https://www.saucedemo.com</URL>
      <STANDARD_USER>standard_user</STANDARD_USER>
      <LOCKED_OUT_USER>locked_out_user</LOCKED_OUT_USER>
      <DEMO_PASSWORD>secret_sauce</DEMO_PASSWORD>
    </EnvironmentVariables>
  </RunConfiguration>
</RunSettings>
```

> ⚠️ **Never commit `local.runsettings`.** It is listed in [`.gitignore`](.gitignore) and automatically loaded by [`SwagLabs_Saucedemo_Playwright_CSharp_NUnit.csproj`](SwagLabs_Saucedemo_Playwright_CSharp_NUnit.csproj) and [`BaseTest.cs`](Tests/BaseTest.cs) when present locally.

### 3. Configure browser projects (`TestData/browserConfig.json`)

Configure the target browser projects (`Chrome`, `Firefox`, `Edge`, `WebKit`), default headless mode, CLI launch arguments, and Firefox user preferences in [`TestData/browserConfig.json`](TestData/browserConfig.json) (or override at runtime via the `BROWSER_PROJECT` and `HEADLESS` environment variables).

### 4. Restore packages, build, and install Playwright browsers

```bash
dotnet restore
dotnet build
pwsh bin/Debug/net8.0/playwright.ps1 install --with-deps
```

---

## ▶️ Running Tests

Tests are configured to run **in parallel across test fixtures** (`[assembly: Parallelizable(ParallelScope.Fixtures)]`, `[assembly: LevelOfParallelism(4)]`).

### Run all tests (across all configured browsers)

```bash
dotnet test
```

### Run tests by Category (`Smoke`, `Login`, `ProductsPage`, `ItemPage`, `Checkout`, `Sorting`, `CartPage`, `CheckoutPage`)

```bash
dotnet test --filter "TestCategory=Smoke"
dotnet test --filter "TestCategory=Login"
dotnet test --filter "TestCategory=ProductsPage"
dotnet test --filter "TestCategory=ItemPage"
dotnet test --filter "TestCategory=Checkout"
dotnet test --filter "TestCategory=Sorting"
dotnet test --filter "TestCategory=CartPage"
dotnet test --filter "TestCategory=CheckoutPage"
```

### Run a specific test fixture class

```bash
dotnet test --filter "FullyQualifiedName~T1_LoginPage"
dotnet test --filter "FullyQualifiedName~T2_InventoryPage"
dotnet test --filter "FullyQualifiedName~T3_InventoryItemPage"
dotnet test --filter "FullyQualifiedName~T4_CheckoutFlow"
dotnet test --filter "FullyQualifiedName~T5_SortingProducts"
dotnet test --filter "FullyQualifiedName~T6_CartPage"
dotnet test --filter "FullyQualifiedName~T7_CartPage"
```

### Run a specific test for a specific browser and in headless mode

**Cross-Platform (Bash, PowerShell, CMD):**

```bash
dotnet test -e HEADLESS=true -e BROWSER_PROJECT=Chrome --filter "FullyQualifiedName~T3_InventoryItemPage"
```

**Bash (Git Bash / Linux / macOS):**

```bash
HEADLESS="true" BROWSER_PROJECT="Chrome" dotnet test --filter "FullyQualifiedName~T3_InventoryItemPage"
```

**PowerShell:**

```powershell
$env:HEADLESS="true"; $env:BROWSER_PROJECT="Chrome"; dotnet test --filter "FullyQualifiedName~T3_InventoryItemPage"
```

### Run a single test method

```bash
dotnet test --filter "Name~T4_1_Completes_checkout_for_a_selected_product"
```

### Run tests with detailed console output or TRX report

```bash
dotnet test --logger "console;verbosity=detailed"
dotnet test --logger "trx;LogFileName=TestResults.trx"
```

---

## 📁 Project Structure

```text
SwagLabs_Saucedemo_Playwright_CSharp_NUnit/
├── .github/
│   └── workflows/
│       └── playwright.yml                          # GitHub Actions CI/CD workflow (Quality Gate & Multi-Browser Matrix)
├── Models/                                         # Strongly-typed data & configuration models
│   ├── BrowserConfig.cs                            # BrowserType enum, BrowserProjectConfig & BrowserConfig models
│   ├── CartButtonAction.cs                         # Enum for product button actions (Add, Remove, AddToCart)
│   ├── CurrencyParseOptions.cs                     # Options model for currency string parsing
│   ├── Locale.cs                                   # Tax rate & locale constants
│   ├── Product.cs                                  # Product catalog item model
│   └── ShippingData.cs                             # Customer checkout shipping data model
├── Pages/                                          # Page Object Model (POM) classes
│   ├── Components/
│   │   └── ProductActionsComponent.cs              # Encapsulated component for product locators & cart actions
│   ├── BasePage.cs                                 # Abstract base page with navigation & load verification
│   ├── InternalPage.cs                             # Shared authenticated header, sidebar menu & cart badge actions
│   ├── LoginPage.cs                                # Login page actions & error message locators
│   ├── InventoryPage.cs                            # Products catalog page (add/remove, sorting, item queries)
│   ├── InventoryItemPage.cs                        # Product detail page (add/remove, item details, back navigation)
│   ├── CartPage.cs                                 # Shopping cart page (item verification, checkout, continue shopping)
│   ├── CheckoutStepOnePage.cs                      # Checkout Step 1: Customer shipping information form
│   ├── CheckoutStepTwoPage.cs                      # Checkout Step 2: Order overview, item rows & subtotal/tax/total parsing
│   └── CheckoutCompletePage.cs                     # Checkout Complete: Order confirmation header & message
├── TestData/                                       # External JSON test data & config (copied to output on build)
│   ├── browserConfig.json                          # Browser projects, headless mode, CLI args & profile preferences
│   ├── products.json                               # Product catalog items (id, name, description, price, imageUrl)
│   └── shipping.json                               # Customer shipping details (firstName, lastName, postalCode)
├── Tests/                                          # NUnit test fixtures
│   ├── BaseTest.cs                                 # Base test fixture (PlaywrightTest, browser lifecycle, tracing & video)
│   ├── T1_LoginPage.cs                             # 1. Login, logout & route access control tests
│   ├── T2_InventoryPage.cs                         # 2. Inventory page cart, button state & item detail tests
│   ├── T3_InventoryItemPage.cs                     # 3. Product detail page cart, button state & item detail tests
│   ├── T4_CheckoutFlow.cs                          # 4. End-to-end checkout completion test
│   ├── T5_SortingProducts.cs                       # 5. Product sorting tests (Name A-Z/Z-A, Price Low-High/High-Low)
│   ├── T6_CartPage.cs                              # 6. Shopping cart item status & detail verification tests
│   └── T7_CheckoutPage.cs                          # 7. Checkout overview totals, tax & item detail verification tests
├── Utilities/                                      # Helper utilities & NUnit data sources
│   ├── BrowserProjectSource.cs                     # Cached browserConfig.json loader & TestFixtureSource provider
│   └── Helpers.cs                                  # Product slug generation & invariant currency parsing
├── .editorconfig                                   # C# code style & Roslyn analyzer configuration
├── azure-pipelines.yml                             # Azure DevOps CI/CD pipeline (Quality Gate & Multi-Browser Matrix)
├── local.runsettings                               # Local environment variables (git-ignored)
├── SwagLabs_Saucedemo_Playwright_CSharp_NUnit.csproj # .NET 8 project configuration, analyzers & NuGet dependencies
├── SwagLabs_Saucedemo_Playwright_CSharp_NUnit.slnx   # Solution file
└── README.md                                       # Project documentation
```

---

## 🌐 Target Application

- **URL:** [https://www.saucedemo.com](https://www.saucedemo.com)
- **Type:** Demo e-commerce web application (Swag Labs)
- **Purpose:** QA E2E test automation practice

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

