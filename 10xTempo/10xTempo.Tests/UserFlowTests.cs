using System.Globalization;
using System.Text.RegularExpressions;
using _10xTempo.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace _10xTempo.Tests;

public class UserFlowTests : IClassFixture<TempoFactory>
{
    private readonly TempoFactory _factory;

    public UserFlowTests(TempoFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task User_can_register_create_company_and_add_report()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = true
        });

        var email = $"user{Guid.NewGuid():N}@tempo.test";
        const string password = "Pass123!";

        await Register(client, email, password);
        await CreateCompany(client, "Test Company");

        await using (var preScope = _factory.Services.CreateAsyncScope())
        {
            var db = preScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = preScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(email) ?? throw new InvalidOperationException("Użytkownik nie został znaleziony.");

            Assert.True(await db.Companies.AnyAsync(), "Firma nie została zapisana.");
            Assert.True(await db.UserCompanies.AnyAsync(), "Powiązanie użytkownika z firmą nie zostało zapisane.");
            Assert.True(await db.UserCompanies.AnyAsync(uc => uc.UserId == user.Id), "Powiązanie nie wskazuje na zalogowanego użytkownika.");
            var companyId = await db.Companies.Select(c => c.Id).FirstAsync();
            await AddReport(client, companyId, 2.5m);
        }

        await using (var scope = _factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            Assert.True(await db.Reports.AnyAsync(), "Raport nie został zapisany w bazie.");
        }

        var reportsPage = await client.GetStringAsync("/Reports/Index");
        if (!reportsPage.Contains("2.5", StringComparison.OrdinalIgnoreCase) &&
            !reportsPage.Contains("2,5", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Raport powinien zawierać liczbe godzin 2.5. Zawartość: {reportsPage}");
        }

        Assert.Contains("Oczekuje", reportsPage);
    }

    private static async Task Register(HttpClient client, string email, string password)
    {
        var token = await GetAntiforgery(client, "/Identity/Account/Register");
        var response = await client.PostAsync("/Identity/Account/Register",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Input.Email"] = email,
                ["Input.Password"] = password,
                ["Input.ConfirmPassword"] = password,
                ["__RequestVerificationToken"] = token
            }));

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Register failed: {body}");
        }
        response.EnsureSuccessStatusCode();
    }

    private static async Task CreateCompany(HttpClient client, string name)
    {
        var token = await GetAntiforgery(client, "/Companies/Create");
        var response = await client.PostAsync("/Companies/Create",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Input.Name"] = name,
                ["__RequestVerificationToken"] = token
            }));

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Create company failed: {body}");
        }
        response.EnsureSuccessStatusCode();
    }

    private static async Task AddReport(HttpClient client, Guid companyId, decimal hours)
    {
        var token = await GetAntiforgery(client, "/Reports/Create");
        var response = await client.PostAsync("/Reports/Create",
            new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["Input.CompanyId"] = companyId.ToString(),
                ["Input.Hours"] = hours.ToString(CultureInfo.InvariantCulture),
                ["__RequestVerificationToken"] = token
            }));

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Add report failed: {body}");
        }
        response.EnsureSuccessStatusCode();
    }

    private static async Task<string> GetAntiforgery(HttpClient client, string path)
    {
        var response = await client.GetAsync(path);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"GET {path} failed: {body}");
        }

        var html = await response.Content.ReadAsStringAsync();
        var match = Regex.Match(html, "name=[\"']__RequestVerificationToken[\"'][^>]*value=[\"'](?<value>[^\"']+)[\"']",
            RegexOptions.IgnoreCase);
        if (!match.Success)
        {
            throw new InvalidOperationException($"Antiforgery token not found for {path}");
        }

        return match.Groups["value"].Value;
    }
}

public class TempoFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"tempo-test-{Guid.NewGuid():N}.db";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
    builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite($"Data Source={_dbName}"));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        });
    }
}
