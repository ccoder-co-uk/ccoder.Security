using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Security.AcceptanceTests.Exceptions;
using SecurityMSSQL;
using System.Text;

namespace Security.AcceptanceTests.Clients;

public class AccountApiClient
{
    private readonly WebApplicationFactory<Program> webApplicationFactory;
    private HttpClient api;
    public SecurityDbContext Database { get; set; }

    private const string endpoint = "Api/Account/";

    public AccountApiClient()
    {
        webApplicationFactory = new();
        webApplicationFactory.EnsureDatabasesAreSetupForTesting();

        api = webApplicationFactory.CreateClient();
        api.Authenticate("TestUser", "TestPass01!").Wait();

        using IServiceScope scope = webApplicationFactory.Services.CreateScope();
        IServiceProvider scopedServices = scope.ServiceProvider;

        Database = scopedServices.GetRequiredService<ISecurityDbContextFactory>()
            .CreateDbContext();
    }

    public HttpClient UseNoCookiesApiClient() =>
        api = webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions { HandleCookies = false });

    public async ValueTask PostAsync(string query, object content)
    {
        HttpResponseMessage request = await api.PostAsync(endpoint + query, new StringContent(content.ToJson(), Encoding.UTF8, "application/json"));

        if ((int)request.StatusCode == 500)
            throw new InternalServerErrorException(await request.Content.ReadAsStringAsync());

        if ((int)request.StatusCode == 400)
            throw new BadRequestException(await request.Content.ReadAsStringAsync());

        request.EnsureSuccessStatusCode();
    }

    public async ValueTask<Token> LoginAsync(Auth auth, string query = "")
    {
        StringContent content = new(auth.ToJson(), Encoding.UTF8, "application/json");
        HttpResponseMessage request = await api.PostAsync(endpoint + "Login" + query, content);
        request.EnsureSuccessStatusCode();
        return await request.Content.ReadAsAsync<Token>();
    }

    public async Task TearDown(string ssoUserId)
    {
        SSOUser user = Database.Users
            .IgnoreQueryFilters()
            .FirstOrDefault(u => u.Id == ssoUserId);

        if (user != null)
        {
            List<Token> tokens = [.. Database.Tokens
                .IgnoreQueryFilters()
                .Where(t => t.UserName == user.Id)];

            List<SSOUserRole> userRoles = [.. Database.UserRoles
                .IgnoreQueryFilters()
                .Where(r => r.UserId == user.Id)];

            Database.Tokens.RemoveRange(tokens);
            Database.UserRoles.RemoveRange(userRoles);
            Database.Users.Remove(user);
            await Database.SaveChangesAsync();
        }
    }
}
