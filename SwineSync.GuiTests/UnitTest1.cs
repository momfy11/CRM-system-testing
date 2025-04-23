using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace PlaywrightTests;

[TestClass]
public class DemoTest : PageTest
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _browserContext;
    private IPage _page;

    [TestInitialize]
    public async Task Setup()
    {
        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 100  // Lägger in en fördröjning så vi kan se vad som händer
        });
        _browserContext = await _browser.NewContextAsync();
        _page = await _browserContext.NewPageAsync();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        await _browserContext.CloseAsync();
        await _browser.CloseAsync();
        _playwright.Dispose();
    }
    [TestMethod]
    public async Task SuperAdminLogin()
    {
        await _page.GotoAsync("http://localhost:5173");
        await _page.Locator("input[name=email]").FillAsync("super_gris@mail.com");
        await _page.Locator("input[name=password]").FillAsync("kung");
        await _page.Locator("input[type=submit][value='login!']").ClickAsync();

        await Expect(_page).ToHaveURLAsync("http://localhost:5173/super-admin");
    }

    [TestMethod]
    public async Task SuperAdminEditCompanySaveAndGoBack()
    {
        string phone = "07" + Random.Shared.Next(100000000, 999999999).ToString();

        await _page.GotoAsync("http://localhost:5173/super-admin");
        await _page.GetByRole(AriaRole.Button, new() { Name = "Companies" }).ClickAsync();
        await Expect(_page).ToHaveURLAsync("http://localhost:5173/companies");

        await _page.Locator(".list-item button:has-text('Edit')").First.ClickAsync();
        await Expect(_page).ToHaveURLAsync(new Regex(".*/companies/\\d+/edit"));

        await _page.Locator("input[name=phone]").FillAsync(phone);
        _page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await _page.Locator("input[type=submit][value='Save']").ClickAsync();

        await _page.GetByRole(AriaRole.Link, new() { Name = "Back" }).ClickAsync();
        await Expect(_page).ToHaveURLAsync("http://localhost:5173/companies");
    }

    [TestMethod]
    public async Task SuperAdminBlockAndActivateCompany()
    {
        await _page.GotoAsync("http://localhost:5173/companies");
        await _page.Locator(".list-item button:has-text('block')").First.ClickAsync();

        await _page.GetByRole(AriaRole.Button, new() { Name = "Show Inactive Companies" }).ClickAsync();
        await _page.Locator(".inactive-list-item button:has-text('Activate')").First.ClickAsync();
    }

    [TestMethod]
    public async Task SuperAdminAddCompany()
    {
        string suffix = Guid.NewGuid().ToString().Substring(0, 6);
        string phone = "07" + Random.Shared.Next(100000000, 999999999).ToString();

        await _page.GotoAsync("http://localhost:5173/companies/add");
        await _page.Locator("input[name=name]").FillAsync($"TestCo_{suffix}");
        await _page.Locator("input[name=email]").FillAsync($"contact_{suffix}@test.com");
        await _page.Locator("input[name=phone]").FillAsync(phone);
        await _page.Locator("input[name=description]").FillAsync("A testing company");
        await _page.Locator("input[name=domain]").FillAsync($"https://{suffix}.test.com");

        _page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

    }


    [TestMethod]
    public async Task SuperAdminEditAdminName()
    {
        string suffix = Guid.NewGuid().ToString()[..6];

        await _page.GotoAsync("http://localhost:5173");
        await _page.Locator("input[name=email]").FillAsync("super_gris@mail.com");
        await _page.Locator("input[name=password]").FillAsync("kung");
        await _page.Locator("input[type=submit][value='login!']").ClickAsync();
        await Expect(_page).ToHaveURLAsync("http://localhost:5173/super-admin");

        await _page.Locator("text=Admins").ClickAsync();
        await Expect(_page).ToHaveURLAsync("http://localhost:5173/admins");

        await _page.Locator("a:has(button:text('Edit'))").First.ClickAsync();

        await _page.Locator("input[name=name]").FillAsync($"Updated Admin {suffix}");

        _page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await _page.Locator("input[type=submit][value='Save']").ClickAsync();
    }


    [TestMethod]
    public async Task SuperAdminBlockAndActivateAdmin()
    {
        await _page.GotoAsync("http://localhost:5173/admins");
        await _page.Locator(".list-item button:has-text('block')").First.ClickAsync();

        await _page.GetByRole(AriaRole.Button, new() { Name = "Show Inactive Admins" }).ClickAsync();
        await _page.Locator(".inactive-list-item button:has-text('Activate')").First.ClickAsync();

    }

    [TestMethod]
    public async Task SuperAdminAddAdmin()
    {
        string suffix = Guid.NewGuid().ToString()[..6];

        await _page.GotoAsync("http://localhost:5173");
        await _page.Locator("input[name=email]").FillAsync("super_gris@mail.com");
        await _page.Locator("input[name=password]").FillAsync("kung");
        await _page.Locator("input[type=submit][value='login!']").ClickAsync();
        await Expect(_page).ToHaveURLAsync("http://localhost:5173/super-admin");

        await _page.Locator("text=Admins").ClickAsync();
        await Expect(_page).ToHaveURLAsync("http://localhost:5173/admins");

        await _page.Locator("text=Add Admin").ClickAsync();

        await _page.Locator("input[name=name]").FillAsync($"TestAdmin_{suffix}");
        await _page.Locator("input[name=email]").FillAsync($"admin_{suffix}@test.com");

        var dropdown = _page.Locator("select[name=company]");
        await dropdown.SelectOptionAsync(new SelectOptionValue { Index = 1 });

        _page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await _page.Locator("input[type=submit][value='Save']").ClickAsync();
    }

    [TestMethod]
    public async Task AdminLogin()
    {
        await _page.GotoAsync("http://localhost:5173");
        await _page.Locator("input[name=email]").FillAsync("trough-manager@company3.com");
        await _page.Locator("input[name=password]").FillAsync("feedme456");
        await _page.Locator("input[type=submit][value='login!']").ClickAsync();

        await Expect(_page).ToHaveURLAsync("http://localhost:5173/admin");
    }

    [TestMethod]
    public async Task AdminEditProductSaveAndBack()
    {
        await AdminLogin();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Products" }).ClickAsync();
        await _page.Locator("button", new() { HasTextString = "Edit" }).First.ClickAsync();

        var newDesc = $"Updated description {Guid.NewGuid().ToString().Substring(0, 4)}";
        await _page.Locator("input[name=description]").FillAsync(newDesc);

        _page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await _page.GetByRole(AriaRole.Button, new() { Name = "Back" }).ClickAsync();
        await _page.GetByText("Sign Out").ClickAsync();
    }

    [TestMethod]
    public async Task AdminBlockUnblockProduct()
    {
        await AdminLogin();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Products" }).ClickAsync();

        await _page.Locator("button", new() { HasTextString = "block" }).First.ClickAsync();
        await _page.Locator("button", new() { HasTextString = "Show Inactive Products" }).ClickAsync();
        await _page.Locator("button", new() { HasTextString = "Activate" }).First.ClickAsync();

        await _page.GetByText("Sign Out").ClickAsync();
    }

    [TestMethod]
    public async Task AdminAddProduct()
    {
        await AdminLogin();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Products" }).ClickAsync();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Add product" }).ClickAsync();

        string suffix = Guid.NewGuid().ToString().Substring(0, 6);

        await _page.Locator("input[name=name]").FillAsync($"TestProd_{suffix}");
        await _page.Locator("input[name=description]").FillAsync($"Desc_{suffix}");
        await _page.Locator("input[name=price]").FillAsync("999");
        await _page.Locator("input[name=category]").FillAsync($"Cat_{suffix}");

        _page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await _page.GetByText("Sign Out").ClickAsync();
    }

    [TestMethod]
    public async Task AdminEditSupportAgentSaveAndBack()
    {
        await AdminLogin();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Support Agents" }).ClickAsync();
        await _page.Locator("button", new() { HasTextString = "Edit" }).First.ClickAsync();

        var newName = $"Updated_{Guid.NewGuid().ToString().Substring(0, 4)}";
        await _page.Locator("input[name=name]").FillAsync(newName);

        _page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await _page.GetByRole(AriaRole.Button, new() { Name = "Back" }).ClickAsync();
        await _page.GetByText("Sign Out").ClickAsync();
    }

    [TestMethod]
    public async Task AdminBlockUnblockSupportAgent()
    {
        await AdminLogin();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Support Agents" }).ClickAsync();

        await _page.Locator("button", new() { HasTextString = "block" }).First.ClickAsync();
        await _page.Locator("button", new() { HasTextString = "Show Inactive Agents" }).ClickAsync();
        await _page.Locator("button", new() { HasTextString = "Activate" }).First.ClickAsync();

        await _page.GetByText("Sign Out").ClickAsync();
    }

    [TestMethod]
    public async Task AdminAddSupportAgent()
    {
        await AdminLogin();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Support Agents" }).ClickAsync();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Add Support Agent" }).ClickAsync();

        string suffix = Guid.NewGuid().ToString().Substring(0, 6);

        await _page.Locator("input[name=name]").FillAsync($"Agent_{suffix}");
        await _page.Locator("input[name=email]").FillAsync($"agent_{suffix}@company3.com");

        // Välj kategori
        var checkboxes = _page.Locator("input[type=checkbox]");
        int count = await checkboxes.CountAsync();
        for (int i = 0; i < count; i++)
        {
            await checkboxes.Nth(i).CheckAsync();
        }

        _page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

        await _page.GetByText("Sign Out").ClickAsync();
    }

    [TestMethod]
    public async Task AdminAddCategory()
    {
        await AdminLogin();
        await _page.GetByRole(AriaRole.Button, new() { Name = "Categories" }).ClickAsync();

        string suffix = Guid.NewGuid().ToString().Substring(0, 6);
        string categoryName = $"TestCategory_{suffix}";

        await _page.Locator("input[name=categoryName]").FillAsync(categoryName);
        await _page.Locator("input[type=submit][value='Add Category']").ClickAsync();

        await _page.GetByText("Sign Out").ClickAsync();
    }

    [TestMethod]
    public async Task CustomerAgentSendMessageAndLogout()
    {
        await _page.GotoAsync("http://localhost:5173");
        await _page.Locator("input[name=email]").FillAsync("tryne@hotmail.com");
        await _page.Locator("input[name=password]").FillAsync("asd123");
        await _page.Locator("input[type=submit][value='login!']").ClickAsync();

        await _page.Locator(".tickets-left").First.ClickAsync();

        await _page.Locator("textarea").FillAsync("Detta är ett autotestat svar.");
        await _page.Locator("input[type=submit][value='Send']").ClickAsync();

        await _page.GetByText("Sign Out").ClickAsync();
        await Expect(_page).ToHaveURLAsync("http://localhost:5173");
    }

    [TestMethod]
    public async Task CustomerAgentCloseTicket()
    {
        await _page.GotoAsync("http://localhost:5173");
        await _page.Locator("input[name=email]").FillAsync("tryne@hotmail.com");
        await _page.Locator("input[name=password]").FillAsync("asd123");
        await _page.Locator("input[type=submit][value='login!']").ClickAsync();


        await _page.Locator(".tickets-left").First.ClickAsync();

        await _page.GetByRole(AriaRole.Button, new() { Name = "Close Ticket" }).ClickAsync();

        await _page.GetByRole(AriaRole.Button, new() { Name = "Open Ticket" }).ClickAsync();

        await _page.GetByText("Back").ClickAsync();
        await _page.GetByText("Sign Out").ClickAsync();
       
    }

/*
    [TestMethod]
    public async Task CustomerCreatesTicket()
    {
        var suffix = Guid.NewGuid().ToString().Substring(0, 6);

        var mobileContext = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 600, Height = 695 },
            UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0 Mobile/15A372 Safari/604.1"
        });

        var page = await mobileContext.NewPageAsync();

        await page.GotoAsync("http://localhost:5173/tech-solutions");

        // Klicka på produkt och välj via text
        await page.Locator("select[name='product']").ClickAsync();
        await page.GetByText("1 ton Pig Feed").ClickAsync();

        await page.Locator("select[name='category']").ClickAsync();
        await page.GetByText("Product Feedback").ClickAsync();

        await page.Locator("input[placeholder='Enter your email..']").FillAsync($"customer_{suffix}@test.com");
        await page.Locator("input[placeholder='Enter title..']").FillAsync($"Testärende {suffix}");
        await page.Locator("textarea[placeholder='Enter message..']").FillAsync("Det här är ett automatiskt testmeddelande 🎫");

        page.Dialog += async (_, dialog) => await dialog.AcceptAsync();
        await page.Locator("input[type=submit][value='Create Ticket!']").ClickAsync();

        await mobileContext.CloseAsync();
    }

*/


}
