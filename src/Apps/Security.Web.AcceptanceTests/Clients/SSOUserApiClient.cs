// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Security.AcceptanceTests.Clients;

public class SSOUserApiClient : IDisposable
{
    private readonly WebApplicationFactory<AcceptanceHost> webApplicationFactory;
    private readonly HttpClient api;

    public SecurityDbContext Database { get; set; }

    private const string Endpoint = "Api/Security/SSOUser/";

    public SSOUserApiClient()
    {
        webApplicationFactory = new();
        webApplicationFactory.EnsureDatabasesAreSetupForTesting();

        api = webApplicationFactory.CreateClient();
        api.Authenticate(user: "TestUser", pass: "TestPass01!").Wait();

        using IServiceScope scope = webApplicationFactory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;

        Database = scopedServices.GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext();
    }

    public async ValueTask<IEnumerable<SSOUser>> GetAllSSOUsersAsync(string query = "") =>
        await api.GetODataCollection<SSOUser>(query: Endpoint + query);

    public void Dispose()
    {
        Database?.Dispose();
        api?.Dispose();
        webApplicationFactory?.Dispose();
        SecurityWebApplicationFactoryExtensions.DropAcceptanceDatabaseForTesting();
    }
}