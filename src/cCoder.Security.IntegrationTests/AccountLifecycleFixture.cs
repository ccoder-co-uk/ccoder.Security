// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.IntegrationTests;

public sealed class AccountLifecycleFixture : IDisposable
{
    private readonly string previousConnectionString;
    private readonly string previousDecryptionKey;

    public AccountLifecycleFixture()
    {
        IntegrationTestConfiguration configuration =
            IntegrationTestConfiguration.Load();

        previousConnectionString = configuration.ProcessConnectionString;
        previousDecryptionKey = configuration.ProcessDecryptionKey;

        Environment.SetEnvironmentVariable(
            variable: IntegrationTestConfiguration.ConnectionStringVariableName,
            value: configuration.AcceptanceConnectionString);

        Environment.SetEnvironmentVariable(
            variable: IntegrationTestConfiguration.DecryptionKeyVariableName,
            value: configuration.DecryptionKey);

        WebApplicationFactory = new WebApplicationFactory<AcceptanceHost>();

        Api = WebApplicationFactory.CreateClient(
            options: new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri(uriString: "https://localhost")
            });
    }

    public HttpClient Api { get; }
    public WebApplicationFactory<AcceptanceHost> WebApplicationFactory { get; }

    public SecurityDbContext CreateSecurityDbContext()
    {
        using IServiceScope scope = WebApplicationFactory.Services.CreateScope();

        return scope.ServiceProvider
            .GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext(ignoreAuthInfo: true);
    }

    public void Dispose()
    {
        using SecurityDbContext database = CreateSecurityDbContext();

        database.Database.EnsureDeleted();
        Api.Dispose();
        WebApplicationFactory.Dispose();

        Environment.SetEnvironmentVariable(
            variable: IntegrationTestConfiguration.ConnectionStringVariableName,
            value: string.IsNullOrEmpty(value: previousConnectionString)
                ? null
                : previousConnectionString);

        Environment.SetEnvironmentVariable(
            variable: IntegrationTestConfiguration.DecryptionKeyVariableName,
            value: string.IsNullOrEmpty(value: previousDecryptionKey)
                ? null
                : previousDecryptionKey);
    }
}