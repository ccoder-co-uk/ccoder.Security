// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.Data.SqlClient;

namespace Security.AcceptanceTests;

internal sealed class AcceptanceTestConfiguration
{
    private AcceptanceTestConfiguration()
    {
        string sourceConnectionString = Environment.GetEnvironmentVariable(
            variable: "Security__ConnectionString")
            ?? throw new InvalidOperationException(
                message: "Security__ConnectionString is required.");

        SecurityConnectionString = sourceConnectionString;
    }

    internal static AcceptanceTestConfiguration Current { get; } = new();

    internal string SecurityConnectionString { get; }

    internal string CreateSecurityConnectionString()
    {
        SqlConnectionStringBuilder builder = new(
            connectionString: SecurityConnectionString);

        builder.InitialCatalog =
            $"{builder.InitialCatalog}-acceptance-{Guid.NewGuid():N}";

        string connectionString = builder.ConnectionString;

        Environment.SetEnvironmentVariable(
            variable: "Security__ConnectionString",
            value: connectionString);

        return connectionString;
    }
}