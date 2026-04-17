using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Objects.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Security.AcceptanceTests;

public static class SecurityWebApplicationFactoryExtensions
{
    private static bool setupComplete = false;

    public static void EnsureDatabasesAreSetupForTesting(this WebApplicationFactory<AcceptanceHost> appFactory)
    {
        lock (appFactory)
        {
            if (setupComplete)
                return;

            setupComplete = true;

            using IServiceScope scope = appFactory.Services.CreateScope();
            IServiceProvider scopedServices = scope.ServiceProvider;
            EnsureSSOSetupForTesting(scopedServices).AsTask().Wait();
        }
    }

    private static async ValueTask EnsureSSOSetupForTesting(IServiceProvider scopedServices)
    {
        IAccountManager accountManager = scopedServices.GetRequiredService<IAccountManager>();
        ITenantManager tenantManager = scopedServices.GetRequiredService<ITenantManager>();

        using cCoder.Security.Data.EF.SecurityDbContext db =
            scopedServices.GetRequiredService<ISecurityDbContextFactory>().CreateDbContext();

        db.Database.EnsureDeleted();
        db.Migrate();

        await SetupTestUser(tenantManager);
        await accountManager.LoginAsync("TestUser", "TestPass01!");
    }

    private static async Task SetupTestUser(ITenantManager tenantManager)
    {
        await tenantManager.SetupAsync(new SetupDetails
        {
            Tenant = new Tenant
            {
                Id = "default",
                Name = "default",
                Description = "Acceptance test tenant"
            },
            User = new SSOUser
            {
                Id = "TestUser",
                DisplayName = "Test User",
                Email = "TestUser@somehwere.com",
                PasswordHash = "TestPass01!"
            }
        });
    }
}

