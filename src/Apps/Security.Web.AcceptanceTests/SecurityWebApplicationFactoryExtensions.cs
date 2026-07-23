// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Security.AcceptanceTests;

public static class SecurityWebApplicationFactoryExtensions
{
    private static readonly object SetupLock = new();
    private static bool acceptanceEnvironmentConfigured = false;
    private static bool setupComplete = false;
    private static bool ownsAcceptanceDatabase = false;
    private static string acceptanceConnectionString;

    public static void EnsureDatabasesAreSetupForTesting(this WebApplicationFactory<AcceptanceHost> appFactory)
    {
        lock (SetupLock)
        {
            if (setupComplete)
            { return; }

            ConfigureAcceptanceEnvironment();

            using IServiceScope scope = appFactory.Services.CreateScope();
            IServiceProvider scopedServices = scope.ServiceProvider;

            EnsureSSOSetupForTesting(scopedServices: scopedServices)
                .AsTask()
                .Wait();

            setupComplete = true;
        }
    }

    private static void ConfigureAcceptanceEnvironment()
    {
        if (acceptanceEnvironmentConfigured)
        { return; }

        if (typeof(AcceptanceHost).Namespace == "Security.Web")
        {
            acceptanceConnectionString = Environment.GetEnvironmentVariable(
                variable: "ConnectionStrings__SSO");

            if (!string.IsNullOrWhiteSpace(value: acceptanceConnectionString))
            {
                SqlConnectionStringBuilder connectionStringBuilder =
                    new(connectionString: acceptanceConnectionString);

                connectionStringBuilder.InitialCatalog =
                    $"{connectionStringBuilder.InitialCatalog}-AcceptanceTests-{Environment.ProcessId}";

                acceptanceConnectionString =
                    connectionStringBuilder.ConnectionString;

                Environment.SetEnvironmentVariable(
                    variable: "ENV_ConnectionStrings__SSO",
                    value: acceptanceConnectionString);

                acceptanceEnvironmentConfigured = true;
                ownsAcceptanceDatabase = true;
                return;
            }

            throw new InvalidOperationException(
                "ConnectionStrings__SSO must be configured for acceptance tests.");
        }

        acceptanceEnvironmentConfigured = true;
    }

    private static async ValueTask EnsureSSOSetupForTesting(IServiceProvider scopedServices)
    {
        IAuthenticationAggregationService authenticationAggregationService =
            scopedServices.GetRequiredService<IAuthenticationAggregationService>();

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

    public static void DropAcceptanceDatabaseForTesting()
    {
        if (!ownsAcceptanceDatabase)
        { return; }

        DropDatabaseForTesting(connectionString: acceptanceConnectionString);
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