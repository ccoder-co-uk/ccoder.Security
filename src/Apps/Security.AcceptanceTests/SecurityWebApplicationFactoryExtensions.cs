using cCoder.Security.Data.Brokers.Encryption;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.SecurityMSSQL;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SSO.AcceptanceTests;

public static class SecurityWebApplicationFactoryExtensions
{
    static bool setupComplete = false;

    public static void EnsureSSOSetupForTesting(this WebApplicationFactory<Program> appFactory)
    {
        lock (appFactory)
        {
            if (!setupComplete)
            {
                setupComplete = true;

                using var scope = appFactory.Services.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var encryptionBroker = scopedServices.GetService<IPasswordEncryptionBroker>();

                using var db = scopedServices.GetRequiredService<ISecurityDbContextFactory>()
                    .CreateDbContext();

                db.Migrate();
                SeedTestData(db, encryptionBroker);
            }
        }
    }

    static void SeedTestData(SecurityDbContext db, IPasswordEncryptionBroker encryptionBroker)
    {
        SetupTestUser(db, encryptionBroker);
    }

    static void SetupTestUser(SecurityDbContext db, IPasswordEncryptionBroker encryptionBroker)
    {
        if (!db.Users.IgnoreQueryFilters().Any(u => u.Id == "TestUser"))
        {
            var allPrivs = db.GetPrivileges().Select(p => p.Id).ToArray();
            var user = db.Add(CreateTestUser(encryptionBroker)).Entity;
            var role = db.Add(CreateTestAdminsRole(allPrivs)).Entity;
            db.SaveChanges();
            db.Add(new SSOUserRole { UserId = user.Id, RoleId = role.Id });
            db.SaveChanges();
        }
    }

    static SSOUser CreateTestUser(IPasswordEncryptionBroker encryptionBroker) => new()
    {
        Id = "TestUser",
        DisplayName = "Test User",
        PasswordHash = encryptionBroker.Encrypt("TestPass01!"),
        AccessFailedCount = 0,
        Email = "TestUser@corporatelinx.com",
        EmailConfirmed = true,
        PhoneNumber = string.Empty,
        PhoneNumberConfirmed = false,
        LockoutEnabled = false,
        LockoutEndDateUtc = null,
    };

    static SSORole CreateTestAdminsRole(string[] allPrivs) => new()
    {
        Name = "Test Admins",
        Privs = string.Join(",",allPrivs),
        UsersArePortalAdmins = true
    };
}
