// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Security.AcceptanceTests;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public sealed partial class TenantDeletionStorageTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task TenantAndDependentsCommitOrRollBackTogether(bool rejectTenantDelete)
    {
        // Given
        using SecurityWebApplicationFactory appFactory = new();
        appFactory.EnsureDatabasesAreSetupForTesting();
        using IServiceScope scope = appFactory.Services.CreateScope();

        ISecurityDbContextFactory factory =
            scope.ServiceProvider.GetRequiredService<ISecurityDbContextFactory>();

        Tenant tenant = new()
        {
            Id = "tenant-delete-fixture",
            Name = "Tenant deletion fixture",
            Description = "Temporary acceptance-test fixture"
        };

        SSORole role = new() { Id = Guid.NewGuid(), TenantId = tenant.Id, Name = "Fixture role" };

        TenantAnalysis analysis = new()
        {
            Id = Guid.NewGuid(),
            TenantId = tenant.Id,
            Key = "fixture",
            Name = "Fixture analysis",
            Value = "fixture",
            CreatedOn = DateTimeOffset.UtcNow
        };

        using (SecurityDbContext setup = factory.CreateDbContext(ignoreAuthInfo: true))
        {
            setup.AddRange(entities: [tenant, role, analysis, new SSOUserRole
            {
                RoleId = role.Id,
                UserId = "TestUser"
            }]);

            await setup.SaveChangesAsync();

            if (rejectTenantDelete)
            {
                await setup.Database.ExecuteSqlRawAsync(sql: """
                    CREATE TABLE dbo.FixtureTenantDependency
                    (
                        TenantId nvarchar(50) NOT NULL REFERENCES dbo.Tenants(Id)
                    );
                    INSERT INTO dbo.FixtureTenantDependency (TenantId)
                    VALUES ('tenant-delete-fixture');
                    """);
            }
        }

        ITenantBroker broker = scope.ServiceProvider.GetRequiredService<ITenantBroker>();

        // When
        Func<Task> delete = async () => await broker.DeleteTenantAsync(tenant: tenant);

        if (rejectTenantDelete)
        {
            DbUpdateException error = await Assert.ThrowsAsync<DbUpdateException>(testCode: delete);

            Microsoft.Data.SqlClient.SqlException sqlError =
                Assert.IsType<Microsoft.Data.SqlClient.SqlException>(@object: error.InnerException);

            Assert.Equal(expected: 547, actual: sqlError.Number);
        }
        else
        {
            await delete();
        }

        // Then
        using SecurityDbContext result = factory.CreateDbContext(ignoreAuthInfo: true);

        bool hasTenant = result.Tenants
            .IgnoreQueryFilters()
            .Any(predicate: item => item.Id == tenant.Id);

        bool hasRole = result.Roles
            .IgnoreQueryFilters()
            .Any(predicate: item => item.Id == role.Id);

        bool hasMembership = result.UserRoles
            .IgnoreQueryFilters()
            .Any(predicate: item => item.RoleId == role.Id);

        bool hasAnalysis = result.TenantAnalysis
            .IgnoreQueryFilters()
            .Any(predicate: item => item.Id == analysis.Id);

        bool hasOtherTenant = result.Tenants
            .IgnoreQueryFilters()
            .Any(predicate: item => item.Id == "default");

        Assert.Equal(expected: rejectTenantDelete, actual: hasTenant);
        Assert.Equal(expected: rejectTenantDelete, actual: hasRole);
        Assert.Equal(expected: rejectTenantDelete, actual: hasMembership);
        Assert.Equal(expected: rejectTenantDelete, actual: hasAnalysis);
        Assert.True(condition: hasOtherTenant);
    }
}