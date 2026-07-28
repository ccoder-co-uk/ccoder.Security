// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Security.AcceptanceTests;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public partial class TenantManagerSetupTests
{
    [Fact]
    public async Task ShouldBootstrapFirstTenantRoleUserAndMembership()
    {
        // Given
        using SecurityWebApplicationFactory appFactory = new();

        // When
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

        await tenantManager.SetupAsync(setupDetails: new SetupDetails
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

        // Then
        using cCoder.Security.Data.EF.SecurityDbContext assertDb =
            dbContextFactory.CreateDbContext();

        Tenant tenant = assertDb.Tenants.IgnoreQueryFilters()
                            .Single();

        SSORole role = assertDb.Roles.IgnoreQueryFilters()
                           .Single();

        SSOUser user = assertDb.Users.IgnoreQueryFilters()
                           .Single();

        SSOUserRole userRole = assertDb.UserRoles.IgnoreQueryFilters()
                                   .Single();

        tenant.Id.Should()
            .Be(expected: "default");

        role.Name.Should()
            .Be(expected: "Administrators");

        role.TenantId.Should()
            .Be(expected: "default");

        role.UsersArePortalAdmins.Should()
            .BeTrue();

        role.Privs.Split(
            separator: ',',
            options: StringSplitOptions.RemoveEmptyEntries)
            .Should()
            .Contain(expected: ["security_admin", "tenant_read", "tenant_admin"]);

        user.Id.Should()
            .Be(expected: "admin");

        user.EmailConfirmed.Should()
            .BeTrue();

        userRole.UserId.Should()
            .Be(expected: "admin");

        userRole.RoleId.Should()
            .Be(expected: role.Id);

        assertDb.Roles.IgnoreQueryFilters()
            .Should()
            .OnlyContain(predicate: foundRole => foundRole.TenantId == "default");
    }
}