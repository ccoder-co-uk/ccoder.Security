using cCoder.Security.Data.EF;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Security.AcceptanceTests.Clients;
using SSO.AcceptanceTests;
using System.Text;

namespace cCoder.Security.AcceptanceTests.Clients;

public class CookieSSOUserApiClient
{
    readonly WebApplicationFactory<SecurityMSSQL.Program> webApplicationFactory;
    HttpClient api;

    public SecurityDbContext Database { get; set; }

    const string Endpoint = "Api/Security/SSOUser/";

    public CookieSSOUserApiClient()
    {
        webApplicationFactory = new();
        webApplicationFactory.EnsureDatabasesAreSetupForTesting();

        api = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = true });
        api.Authenticate("TestUser", "TestPass01!").Wait();

        using var scope = webApplicationFactory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;

        Database = scopedServices.GetRequiredService<SecurityDbContext>();
    }

    public async ValueTask<Token> LoginAsync(Auth auth, string query = "", bool keepSessionCookie = false)
    {
        var content = new StringContent(auth.ToJson(), Encoding.UTF8, "application/json");
        var request = await api.PostAsync("/Api/Account/Login" + query, content);
        request.EnsureSuccessStatusCode();
        return await request.Content.ReadAsAsync<Token>();
    }

    public void AddBearerAuthentication(string bearer)
    {
        if (bearer == null)
            api.DefaultRequestHeaders.Authorization = null;
        else
            api.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", bearer);
    }

    public void AddBasicAuthentication(Auth auth)
    {
        if (auth == null)
            api.DefaultRequestHeaders.Authorization = null;
        else
        {
            string encoded = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(auth.User + ":" + auth.Pass));

            api.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("basic", encoded);
        }
    }

    public async ValueTask<IEnumerable<SSOUser>> GetAllSSOUsersAsync(string query = "")
        => await api.GetODataCollection<SSOUser>(Endpoint + query);

    public async ValueTask<SSOUser> Me(string query = "")
    {
        var response = await api.GetAsync(Endpoint + "Me()" + query);
        return await response.Content.ReadAsAsync<SSOUser>();
    }
}