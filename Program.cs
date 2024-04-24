using Microsoft.Playwright;
using OtpNet;
using static System.Net.Mime.MediaTypeNames;

public static class Program
{
    public static async Task Main(string[] args)
    {
        /*
        NOTE: In production scenarios you should keep the credentials & secrets in Key Vault NOT Environment Variables.
        */

        using IPlaywright playwright = await Playwright.CreateAsync();
        await using IBrowser browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            SlowMo = 2000 // Add pause to each step so that you can see automation in action!
        });
        await AadSignIn(browser);
    }

    private static async Task AadSignIn(IBrowser browser)
    {
        //var twoFactorSecret = "qdcgk7sz37ugjzj5";
        var twoFactorSecret = "kd5szoe4wu364zwt";
        var r = GenerateTwoFactorAuthCode(twoFactorSecret);
        IBrowserContext context = await browser.NewContextAsync();
        IPage page = await context.NewPageAsync();

        await page.GotoAsync("https://www.gkpulseportaluat.co.uk/Account/SignUpSignIn");

        await page.Locator("#email").FillAsync("user.auto@networklimited.co.uk");
        await page.Locator("#password").FillAsync("UseAutdgs76");
        await page.GetByRole(AriaRole.Button, new() { Name = "Sign in" }).ClickAsync();
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Enter your code" }).FillAsync(GenerateTwoFactorAuthCode(twoFactorSecret));
        await page.GetByRole(AriaRole.Button, new() { Name = "Verify" }).ClickAsync();
        await Task.Delay(TimeSpan.FromMinutes(1));
    }

    private static string GenerateTwoFactorAuthCode(string secret)
    {
        Totp totp = new(secretKey: Base32Encoding.ToBytes(secret));

        return totp.ComputeTotp();
    }
}