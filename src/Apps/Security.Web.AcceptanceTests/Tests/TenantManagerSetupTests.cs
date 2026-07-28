// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

public partial class TenantManagerSetupTests
{
    [Fact]
    public async Task ShouldBootstrapFirstTenantRoleUserAndMembership()
    {
        // Given
        string originalConnectionString =
            Environment.GetEnvironmentVariable(variable: "Security__ConnectionString");

        string acceptanceConnectionString = CreateIsolatedAcceptanceConnectionString();

        // When
        Environment.SetEnvironmentVariable(
variable: "Security__ConnectionString",
value: acceptanceConnectionString);

        // Then
        try
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

            using cCoder.Security.Data.EF.SecurityDbContext assertDb = dbContextFactory.CreateDbContext();

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
        finally
        {
            global::Security.AcceptanceTests.SecurityWebApplicationFactoryExtensions
                .DropDatabaseForTesting(connectionString: acceptanceConnectionString);

            Environment.SetEnvironmentVariable(variable: "Security__ConnectionString", value: originalConnectionString);
        }
    }

    private static string CreateIsolatedAcceptanceConnectionString()
    {
        string connectionString = Environment.GetEnvironmentVariable(
            variable: "Security__ConnectionString")
            ?? throw new InvalidOperationException(
                message: "Security__ConnectionString is required.");

        Microsoft.Data.SqlClient.SqlConnectionStringBuilder builder =
            new(connectionString);

        builder.InitialCatalog =
            $"{builder.InitialCatalog}-acceptance-{Guid.NewGuid():N}";

        return builder.ConnectionString;
    }
}