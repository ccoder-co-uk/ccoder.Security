using cCoder.Security.Api.Interfaces;
using cCoder.Security.Data.Brokers.Encryption;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.SecurityMSSQL;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace SSO.AcceptanceTests;

public static class SecurityWebApplicationFactoryExtensions
{
    static bool setupComplete = false;

    public static void EnsureDatabasesAreSetupForTesting(this WebApplicationFactory<Program> appFactory)
    {
        lock (appFactory)
        {
            if (!setupComplete)
            {
                setupComplete = true;

                using var scope = appFactory.Services.CreateScope();
                var scopedServices = scope.ServiceProvider;

                EnsureSSOSetupForTesting(scopedServices)
                    .AsTask()
                    .Wait();
            }
        }
    }

    static async ValueTask EnsureSSOSetupForTesting(IServiceProvider scopedServices)
    {
        var accountManager = scopedServices.GetRequiredService<IAccountManager>();

        using var db = scopedServices.GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext();

        db.Database.EnsureDeleted();
        db.Migrate();

        await SetupTestUser(accountManager);
        await accountManager.LoginAsync("TestUser", "TestPass01!");
    }

    static async Task SetupTestUser(IAccountManager accountManager)
    {
        var user = new RegisterUser
        {
            DisplayName = "Test User",
            Email = "TestUser@somehwere.com",
            Password = "TestPass01!",
            Culture = string.Empty
        };

        (_, string confirmationToken) = await accountManager.RegisterAsync(user);
        await accountManager.ConfirmRegistrationAsync(confirmationToken);
    }

    static SSORole CreateTestAdminsRole(string[] allPrivs) => new()
    {
        Name = "Test Admins",
        Privs = string.Join(",",allPrivs),
        UsersArePortalAdmins = true
    };
}
