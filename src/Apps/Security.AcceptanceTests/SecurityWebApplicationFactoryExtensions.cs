using cCoder.Security.Api.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SecurityMSSQL;

namespace Security.AcceptanceTests;

public static class SecurityWebApplicationFactoryExtensions
{
    private static bool setupComplete = false;

    public static void EnsureDatabasesAreSetupForTesting(this WebApplicationFactory<Program> appFactory)
    {
        lock (appFactory)
            if (!setupComplete)
            {
                setupComplete = true;

                using IServiceScope scope = appFactory.Services.CreateScope();
                IServiceProvider scopedServices = scope.ServiceProvider;

                EnsureSSOSetupForTesting(scopedServices)
                    .AsTask()
                    .Wait();
            }
    }

    private static async ValueTask EnsureSSOSetupForTesting(IServiceProvider scopedServices)
    {
        IAccountManager accountManager = scopedServices.GetRequiredService<IAccountManager>();

        using cCoder.Security.Data.EF.SecurityDbContext db = scopedServices.GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext();

        db.Database.EnsureDeleted();
        db.Migrate();

        await SetupTestUser(accountManager);
        await accountManager.LoginAsync("TestUser", "TestPass01!");
    }

    private static async Task SetupTestUser(IAccountManager accountManager)
    {
        RegisterUser user = new()
        {
            DisplayName = "Test User",
            Email = "TestUser@somehwere.com",
            Password = "TestPass01!",
            Culture = string.Empty
        };

        (_, string confirmationToken) = await accountManager.RegisterAsync(user);
        await accountManager.ConfirmRegistrationAsync(confirmationToken);
    }

    private static SSORole CreateTestAdminsRole(string[] allPrivs) => new()
    {
        Name = "Test Admins",
        Privs = string.Join(",", allPrivs),
        UsersArePortalAdmins = true
    };
}
