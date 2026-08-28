// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Data.SqlClient;

namespace cCoder.Security.IntegrationTests;

internal sealed class IntegrationTestConfiguration
{
    internal const string ConnectionStringVariableName =
        "SecurityData__ConnectionString";

    internal const string DecryptionKeyVariableName =
        "Security__DecryptionKey";

    private IntegrationTestConfiguration(
        string processConnectionString,
        string acceptanceConnectionString,
        string processDecryptionKey,
        string decryptionKey)
    {
        ProcessConnectionString = processConnectionString;
        AcceptanceConnectionString = acceptanceConnectionString;
        ProcessDecryptionKey = processDecryptionKey;
        DecryptionKey = decryptionKey;
    }

    internal string ProcessConnectionString { get; }

    internal string AcceptanceConnectionString { get; }

    internal string ProcessDecryptionKey { get; }

    internal string DecryptionKey { get; }

    internal static IntegrationTestConfiguration Load()
    {
        string sourceConnectionString =
            ReadRequiredValue(
                variableName: ConnectionStringVariableName);

        SqlConnectionStringBuilder builder =
            new(connectionString: sourceConnectionString);

        if (string.IsNullOrWhiteSpace(value: builder.InitialCatalog))
        {
            throw new InvalidOperationException(
                "Integration test connection strings must name a database.");
        }

        builder.InitialCatalog =
            $"{builder.InitialCatalog}-acceptance-{Guid.NewGuid():N}";

        return new IntegrationTestConfiguration(
            processConnectionString:
                Environment.GetEnvironmentVariable(
                    variable:
                        ConnectionStringVariableName)
                ?? string.Empty,
            acceptanceConnectionString: builder.ConnectionString,
            processDecryptionKey:
                Environment.GetEnvironmentVariable(
                    variable: DecryptionKeyVariableName)
                ?? string.Empty,
            decryptionKey: ReadRequiredValue(
                variableName: DecryptionKeyVariableName));
    }

    private static string ReadRequiredValue(string variableName)
    {
        string value =
            Environment.GetEnvironmentVariable(variable: variableName)
            ?? Environment.GetEnvironmentVariable(
                variable: variableName,
                target: EnvironmentVariableTarget.User)
            ?? Environment.GetEnvironmentVariable(
                variable: variableName,
                target: EnvironmentVariableTarget.Machine);

        if (!string.IsNullOrWhiteSpace(value: value))
        {
            return value;
        }

        throw new InvalidOperationException(
            $"Required configuration environment variable '{variableName}' was not found.");
    }
}