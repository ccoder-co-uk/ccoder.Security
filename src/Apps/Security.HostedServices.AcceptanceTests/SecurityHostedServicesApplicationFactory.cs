// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Security.AcceptanceTests;

namespace Security.HostedServices.AcceptanceTests;

internal sealed class SecurityHostedServicesApplicationFactory
    : WebApplicationFactory<AcceptanceHost>
{
    private readonly string previousConnectionString;
    private readonly string previousDecryptionKey;

    internal SecurityHostedServicesApplicationFactory()
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

    private string ConnectionString { get; }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing: disposing);

        if (disposing)
        {
            DropDatabase();

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

    private void DropDatabase()
    {
        SqlConnectionStringBuilder builder =
            new(connectionString: ConnectionString);

        string databaseName = builder.InitialCatalog;
        builder.InitialCatalog = "master";

        using SqlConnection connection =
            new(connectionString: builder.ConnectionString);

        connection.Open();

        using SqlCommand command = connection.CreateCommand();

        command.CommandText =
            $"""
            IF DB_ID(@databaseName) IS NOT NULL
            BEGIN
                ALTER DATABASE [{databaseName}]
                    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                DROP DATABASE [{databaseName}];
            END
            """;

        _ = command.Parameters.AddWithValue(
            parameterName: "@databaseName",
            value: databaseName);

        _ = command.ExecuteNonQuery();
    }
}