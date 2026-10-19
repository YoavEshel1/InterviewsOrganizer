using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;

string csvFilePath = "StartupsData.csv";

// Write header to CSV using UTF-8 encoding
using (var writer = new StreamWriter(csvFilePath, false, Encoding.UTF8))
{
    writer.WriteLine("Company Name,Profile Link,Description,Founded,Business Model,Employees,Stage,Total Raised,Classifications");
}

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
var page = await browser.NewPageAsync();

string startUrl = "https://finder.startupnationcentral.org/startups/search?&sort=employees-desc&status=Active";
await page.GotoAsync(startUrl, new PageGotoOptions { Timeout = 60000 });

bool hasNextPage = true;
int pageNumber = 1;

while (hasNextPage)
{
    Console.WriteLine($"Scraping page {pageNumber}...");

    // 1. Wait for company elements to load on the page
    await page.WaitForSelectorAsync("div.company");

    // 2. Query all rows (each company card is wrapped in an a > div.company)
    var companyCards = await page.QuerySelectorAllAsync("a:has(div.company)");

    using (var writer = new StreamWriter(csvFilePath, true, Encoding.UTF8))
    {
        foreach (var card in companyCards)
        {
            // Company profile URL
            string profileUrl = await card.GetAttributeAsync("href") ?? "";

            // Company name
            var nameElement = await card.QuerySelectorAsync("h2.company-name");
            string name = nameElement != null ? await nameElement.InnerTextAsync() : "";
            name = CleanText(name);

            // Company description (div below company name)
            var descElement = await card.QuerySelectorAsync(".table-row-item div:not([class])");
            string description = descElement != null ? await descElement.InnerTextAsync() : "";

            // Extract all column span elements (returns IReadOnlyList<IElementHandle>)
            var spans = await card.QuerySelectorAllAsync("span.table-row-item");

            // Accessing items safely using .Count instead of .Length
            string founded = spans.Count > 0 ? await spans[0].InnerTextAsync() : "";
            string businessModel = spans.Count > 1 ? await spans[1].InnerTextAsync() : "";
            string employees = spans.Count > 2 ? await spans[2].InnerTextAsync() : "";
            string stage = spans.Count > 3 ? await spans[3].InnerTextAsync() : "";
            string totalRaised = spans.Count > 4 ? await spans[4].InnerTextAsync() : "";

            // Extract all classification tags
            var classElements = await card.QuerySelectorAllAsync(".company-classifications .classification");
            var classificationsList = await Task.WhenAll(classElements.Select(async e => await e.InnerTextAsync()));
            string classifications = string.Join("; ", classificationsList.Select(c => c.Trim()));

            // Append row to CSV with quote escaping
            string csvLine = $"\"{EscapeCsv(name)}\",\"https://finder.startupnationcentral.org{profileUrl}\",\"{EscapeCsv(description)}\",\"{EscapeCsv(founded)}\",\"{EscapeCsv(businessModel)}\",\"{EscapeCsv(employees)}\",\"{EscapeCsv(stage)}\",\"{EscapeCsv(totalRaised)}\",\"{EscapeCsv(classifications)}\"";
            writer.WriteLine(csvLine);
        }
    }

    // 3. Handle next page pagination
    var nextButton = await page.QuerySelectorAsync("a:has-text('Next'), button:has-text('Next'), .pagination-next");

    if (nextButton != null && await nextButton.IsVisibleAsync())
    {
        string className = await nextButton.GetAttributeAsync("class") ?? "";
        bool isDisabled = className.Contains("disabled", StringComparison.OrdinalIgnoreCase);

        if (!isDisabled)
        {
            await nextButton.ClickAsync();

            // Wait for network requests to settle
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            // Brief pause to ensure DOM updates complete
            await page.WaitForTimeoutAsync(2000);
            pageNumber++;
        }
        else
        {
            hasNextPage = false;
        }
    }
    else
    {
        hasNextPage = false;
    }
}

Console.WriteLine($"Scraping finished successfully. Output file: {csvFilePath}");

static string CleanText(string input)
{
    if (string.IsNullOrWhiteSpace(input)) return "";
    return input.Replace("\n", " ").Replace("\r", " ").Trim();
}

static string EscapeCsv(string field)
{
    if (string.IsNullOrEmpty(field)) return "";
    return field.Trim().Replace("\"", "\"\"");
}