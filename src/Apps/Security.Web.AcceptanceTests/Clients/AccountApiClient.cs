// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Security.AcceptanceTests.Exceptions;
using System.Text;

namespace Security.AcceptanceTests.Clients;

public class AccountApiClient : IDisposable
{
    private readonly WebApplicationFactory<AcceptanceHost> webApplicationFactory;
    private readonly bool dropAcceptanceDatabaseOnDispose;
    private HttpClient api;
    public SecurityDbContext Database { get; set; }

    private const string endpoint = "Api/Account/";

    public AccountApiClient()
        : this(authenticate: true, dropAcceptanceDatabaseOnDispose: true)
    {
    }

    private AccountApiClient(
        bool authenticate,
        bool dropAcceptanceDatabaseOnDispose)
    {
        this.dropAcceptanceDatabaseOnDispose = dropAcceptanceDatabaseOnDispose;
        webApplicationFactory = new();
        webApplicationFactory.EnsureDatabasesAreSetupForTesting();

        api = webApplicationFactory.CreateClient();

        if (authenticate)
            api.Authenticate(user: "TestUser", pass: "TestPass01!").Wait();

        using IServiceScope scope = webApplicationFactory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;

        Database = scopedServices.GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext();
    }

    public static AccountApiClient CreateUnauthenticated() =>
        new(authenticate: false, dropAcceptanceDatabaseOnDispose: false);

    public HttpClient UseNoCookiesApiClient() =>
        api = webApplicationFactory.CreateClient(options: new WebApplicationFactoryClientOptions { HandleCookies = false });

    public void AddBearerAuthentication(string bearer)
    {
        if (bearer == null)
            api.DefaultRequestHeaders.Authorization = null;
        else
            api.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", bearer);
    }

    public void AddBasicAuthentication(Auth auth)
    {
        if (auth == null)
            api.DefaultRequestHeaders.Authorization = null;
        else
        {
            string encoded =
                Convert.ToBase64String(inArray: Encoding.UTF8.GetBytes(auth.User + ":" + auth.Pass));

            api.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("basic", encoded);
        }
    }

    public async ValueTask PostAsync(string query, object content)
    {
        HttpResponseMessage request = await api.PostAsync(requestUri: endpoint + query, content: new StringContent(content.ToJson(), Encoding.UTF8, "application/json"));

        if ((int)request.StatusCode == 500)
            throw new InternalServerErrorException(await request.Content.ReadAsStringAsync());

        if ((int)request.StatusCode == 400)
            throw new BadRequestException(await request.Content.ReadAsStringAsync());

        request.EnsureSuccessStatusCode();
    }

    public async ValueTask<Token> LoginAsync(Auth auth, string query = "")
    {
        StringContent content = new(auth.ToJson(), Encoding.UTF8, "application/json");
        HttpResponseMessage request = await api.PostAsync(requestUri: endpoint + "Login" + query, content: content);
        request.EnsureSuccessStatusCode();
        return await request.Content.ReadAsAsync<Token>();
    }

    public async ValueTask<SSOUser> Me(string query = "")
    {
        HttpResponseMessage response = await api.GetAsync(requestUri: endpoint + "Me" + query);
        return await response.Content.ReadAsAsync<SSOUser>();
    }

    public async Task TearDown(string ssoUserId)
    {
        SSOUser user = Database.Users
            .IgnoreQueryFilters()
            .FirstOrDefault(predicate: u => u.Id == ssoUserId);

        if (user != null)
        {
            List<Token> tokens = [.. Database.Tokens
                .IgnoreQueryFilters()
                .Where(predicate:t => t.UserName == user.Id)];

            List<SSOUserRole> userRoles = [.. Database.UserRoles
                .IgnoreQueryFilters()
                .Where(predicate:r => r.UserId == user.Id)];

            Database.Tokens.RemoveRange(entities: tokens);
            Database.UserRoles.RemoveRange(entities: userRoles);
            Database.Users.Remove(entity: user);
            await Database.SaveChangesAsync();
        }
    }

    public void Dispose()
    {
        Database?.Dispose();
        api?.Dispose();
        webApplicationFactory?.Dispose();

        if (dropAcceptanceDatabaseOnDispose)
            SecurityWebApplicationFactoryExtensions.DropAcceptanceDatabaseForTesting();
    }
}