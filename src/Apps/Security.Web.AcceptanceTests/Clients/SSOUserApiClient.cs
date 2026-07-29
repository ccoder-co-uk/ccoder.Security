// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Security.AcceptanceTests.Clients;

public class SSOUserApiClient : IDisposable
{
    private readonly SecurityWebApplicationFactory webApplicationFactory;
    private readonly HttpClient api;

    public SecurityDbContext Database { get; set; }

    private const string Endpoint = "Api/Security/SSOUser/";

    public SSOUserApiClient(
        SecurityWebApplicationFactory webApplicationFactory)
    {
        this.webApplicationFactory = webApplicationFactory;

        api = webApplicationFactory.CreateClient();

        api.Authenticate(user: "TestUser", pass: "TestPass01!")
            .Wait();

        using IServiceScope scope = webApplicationFactory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;

        Database = scopedServices.GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext();
    }

    public ValueTask<IEnumerable<SSOUser>> GetAllSSOUsersAsync(string query = "") =>
        new(api.GetSSOUsersAsync(query: Endpoint + query));

    public void Dispose()
    {
        Database?.Dispose();
        api?.Dispose();
    }
}