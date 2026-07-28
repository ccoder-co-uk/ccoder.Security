// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Testing;
namespace Security.AcceptanceTests;

public sealed class SecurityWebApplicationFactory
    : WebApplicationFactory<AcceptanceHost>
{
    public SecurityWebApplicationFactory()
    {
        ConnectionString =
            AcceptanceTestConfiguration.Current
                .CreateSecurityConnectionString();
    }

    public string ConnectionString { get; }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing: disposing);

        if (disposing)
        {
            SecurityWebApplicationFactoryExtensions.DropDatabaseForTesting(
                connectionString: ConnectionString);
        }
    }
}