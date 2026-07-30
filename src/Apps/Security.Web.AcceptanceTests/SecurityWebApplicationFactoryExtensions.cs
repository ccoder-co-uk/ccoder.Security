// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Security.AcceptanceTests;

public static class SecurityWebApplicationFactoryExtensions
{
    public static void EnsureDatabasesAreSetupForTesting(
        this SecurityWebApplicationFactory appFactory)
    {
        using IServiceScope scope = appFactory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;

        EnsureSSOSetupForTesting(scopedServices: scopedServices)
            .AsTask()
            .Wait();
    }

    private static async ValueTask EnsureSSOSetupForTesting(IServiceProvider scopedServices)
    {
        IAuthenticationManager authenticationAggregationService =
            scopedServices.GetRequiredService<IAuthenticationManager>();

        ITenantManager tenantManager = scopedServices.GetRequiredService<ITenantManager>();

        using cCoder.Security.Data.EF.SecurityDbContext db =
            scopedServices.GetRequiredService<ISecurityDbContextFactory>()
                .CreateDbContext(ignoreAuthInfo: true);

        if (db.Database.ProviderName?.Contains(value: "SqlServer", comparisonType: StringComparison.OrdinalIgnoreCase) == true)
        { DropDatabaseForTesting(connectionString: db.Database.GetConnectionString()); }
        else
        { db.Database.EnsureDeleted(); }

        db.Migrate();

        await SetupTestUser(tenantManager: tenantManager);

        await authenticationAggregationService.LoginAsync(
            username: "TestUser",
            password: "TestPass01!");
    }

    private static async Task SetupTestUser(ITenantManager tenantManager)
    {
        SetupDetails setupDetails = new()
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
        };

        await tenantManager.SetupAsync(
            setupDetails: setupDetails);
    }

    internal static void DropDatabaseForTesting(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(value: connectionString))
        { return; }

        SqlConnectionStringBuilder builder = new(connectionString)
        {
            Encrypt = true,
            TrustServerCertificate = true,
        };

        string databaseName = builder.InitialCatalog ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value: databaseName))
        { return; }

        if (!databaseName.Contains(value: "accept", comparisonType: StringComparison.OrdinalIgnoreCase)
            && !databaseName.Contains(value: "integrationtest", comparisonType: StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
            $"Refusing to drop non-acceptance test database '{databaseName}'.");
        }

        builder.InitialCatalog = "master";

        using SqlConnection connection = new(builder.ConnectionString);
        connection.Open();

        using SqlCommand command = connection.CreateCommand();

        command.CommandText = @"
IF DB_ID(@databaseName) IS NOT NULL
BEGIN
    DECLARE @sql nvarchar(max) =
        N'ALTER DATABASE [' + REPLACE(@databaseName, ']', ']]') + N'] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;'
        + N'DROP DATABASE [' + REPLACE(@databaseName, ']', ']]') + N']';
    EXEC(@sql);
END";

        _ = command.Parameters.AddWithValue(parameterName: "@databaseName", value: databaseName);
        command.ExecuteNonQuery();
    }
}