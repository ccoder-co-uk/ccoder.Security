using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Objects.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Security.AcceptanceTests;

public static class SecurityWebApplicationFactoryExtensions
{
    private static readonly object SetupLock = new();
    private static bool acceptanceEnvironmentConfigured = false;
    private static bool setupComplete = false;

    public static void EnsureDatabasesAreSetupForTesting(this WebApplicationFactory<AcceptanceHost> appFactory)
    {
        lock (SetupLock)
        {
            if (setupComplete)
                return;

            ConfigureAcceptanceEnvironment();

            using IServiceScope scope = appFactory.Services.CreateScope();
            IServiceProvider scopedServices = scope.ServiceProvider;
            EnsureSSOSetupForTesting(scopedServices).AsTask().Wait();
            setupComplete = true;
        }
    }

    private static void ConfigureAcceptanceEnvironment()
    {
        if (acceptanceEnvironmentConfigured)
            return;

        if (typeof(AcceptanceHost).Namespace == "SecurityMSSQL")
        {
            string databaseName = $"SSOAcceptanceTests_{Environment.ProcessId}";
            Environment.SetEnvironmentVariable(
                "ENV_ConnectionStrings__SSO",
                $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={databaseName};MultipleActiveResultSets=True;Trusted_Connection=True;Trust Server Certificate=true");
        }

        acceptanceEnvironmentConfigured = true;
    }

    private static async ValueTask EnsureSSOSetupForTesting(IServiceProvider scopedServices)
    {
        IAccountManager accountManager = scopedServices.GetRequiredService<IAccountManager>();
        ITenantManager tenantManager = scopedServices.GetRequiredService<ITenantManager>();

        using cCoder.Security.Data.EF.SecurityDbContext db =
            scopedServices.GetRequiredService<ISecurityDbContextFactory>().CreateDbContext();

        if (db.Database.ProviderName?.Contains("SqlServer", StringComparison.OrdinalIgnoreCase) != true)
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

