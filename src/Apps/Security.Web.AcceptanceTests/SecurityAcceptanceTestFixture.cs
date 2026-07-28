// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Security.AcceptanceTests.Clients;

namespace Security.AcceptanceTests;

public sealed class SecurityAcceptanceTestFixture : IDisposable
{
    public SecurityAcceptanceTestFixture()
    {
        WebApplicationFactory = new SecurityWebApplicationFactory();
        WebApplicationFactory.EnsureDatabasesAreSetupForTesting();

        AccountApiClient = new AccountApiClient(
            webApplicationFactory: WebApplicationFactory);
        RegisterApiClient = new RegisterApiClient(
            webApplicationFactory: WebApplicationFactory);
        SSOUserApiClient = new SSOUserApiClient(
            webApplicationFactory: WebApplicationFactory);
    }

    public AccountApiClient AccountApiClient { get; }

    public RegisterApiClient RegisterApiClient { get; }

    public SSOUserApiClient SSOUserApiClient { get; }

    public SecurityWebApplicationFactory WebApplicationFactory { get; }

    public void Dispose()
    {
        SSOUserApiClient.Dispose();
        RegisterApiClient.Dispose();
        AccountApiClient.Dispose();
        WebApplicationFactory.Dispose();
    }
}