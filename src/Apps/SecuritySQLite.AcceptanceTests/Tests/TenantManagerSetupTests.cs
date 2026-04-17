using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public class TenantManagerSetupTests
{
    [Fact]
    public async Task ShouldBootstrapFirstTenantRoleUserAndMembership()
    {
        using WebApplicationFactory<AcceptanceHost> appFactory = new();
        using IServiceScope scope = appFactory.Services.CreateScope();
        IServiceProvider services = scope.ServiceProvider;

        ISecurityDbContextFactory dbContextFactory =
            services.GetRequiredService<ISecurityDbContextFactory>();

        using (cCoder.Security.Data.EF.SecurityDbContext db = dbContextFactory.CreateDbContext())
        {
            db.Database.EnsureDeleted();
            db.Migrate();
        }

        ITenantManager tenantManager = services.GetRequiredService<ITenantManager>();

        await tenantManager.SetupAsync(new SetupDetails
        {
            Tenant = new Tenant
            {
                Id = "default",
                Name = "Default"
            },
            User = new SSOUser
            {
                Id = "admin",
                DisplayName = "Admin User",
                Email = "admin@example.com",
                PasswordHash = "TestPass01!"
            }
        });

        using cCoder.Security.Data.EF.SecurityDbContext assertDb = dbContextFactory.CreateDbContext();

        Tenant tenant = assertDb.Tenants.IgnoreQueryFilters().Single();
        SSORole role = assertDb.Roles.IgnoreQueryFilters().Single();
        SSOUser user = assertDb.Users.IgnoreQueryFilters().Single();
        SSOUserRole userRole = assertDb.UserRoles.IgnoreQueryFilters().Single();

        tenant.Id.Should().Be("default");
        role.Name.Should().Be("Administrators");
        role.TenantId.Should().Be("default");
        role.UsersArePortalAdmins.Should().BeTrue();
        user.Id.Should().Be("admin");
        user.EmailConfirmed.Should().BeTrue();
        userRole.UserId.Should().Be("admin");
        userRole.RoleId.Should().Be(role.Id);
        assertDb.Roles.IgnoreQueryFilters().Should().OnlyContain(foundRole => foundRole.TenantId == "default");
    }
}

