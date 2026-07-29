// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Testing;
namespace Security.AcceptanceTests;

public sealed class SecurityWebApplicationFactory
    : WebApplicationFactory<AcceptanceHost>
{
    private readonly string previousConnectionString;
    private readonly string previousDecryptionKey;

    public SecurityWebApplicationFactory()
    {
        AcceptanceTestConfiguration configuration =
            AcceptanceTestConfiguration.Load();

        ConnectionString =
            configuration.AcceptanceConnectionString;

        previousConnectionString =
            configuration.ProcessConnectionString;

        previousDecryptionKey =
            configuration.ProcessDecryptionKey;

        Environment.SetEnvironmentVariable(
            variable:
                AcceptanceTestConfiguration
                    .ConnectionStringVariableName,
            value: ConnectionString);

        Environment.SetEnvironmentVariable(
            variable:
                AcceptanceTestConfiguration
                    .DecryptionKeyVariableName,
            value: configuration.DecryptionKey);
    }

    public string ConnectionString { get; }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing: disposing);

        if (disposing)
        {
            SecurityWebApplicationFactoryExtensions.DropDatabaseForTesting(
                connectionString: ConnectionString);

            Environment.SetEnvironmentVariable(
                variable:
                    AcceptanceTestConfiguration
                        .ConnectionStringVariableName,
                value: string.IsNullOrEmpty(
                    value: previousConnectionString)
                    ? null
                    : previousConnectionString);

            Environment.SetEnvironmentVariable(
                variable:
                    AcceptanceTestConfiguration
                        .DecryptionKeyVariableName,
                value: string.IsNullOrEmpty(
                    value: previousDecryptionKey)
                    ? null
                    : previousDecryptionKey);
        }
    }
}