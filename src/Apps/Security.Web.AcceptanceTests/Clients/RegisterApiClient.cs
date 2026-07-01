using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Security.AcceptanceTests.Exceptions;
using Security.AcceptanceTests.Tests.Models;
using System.Text;

namespace Security.AcceptanceTests.Clients;

public class RegisterApiClient
{
    private readonly WebApplicationFactory<AcceptanceHost> webApplicationFactory;
    private readonly HttpClient api;

    public SecurityDbContext Database { get; set; }

    private const string endpoint = "Api/Account/";

    public RegisterApiClient()
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

    public async ValueTask PostAsync(string query, object content)
    {
        HttpResponseMessage request = await api.PostAsync(endpoint + query, new StringContent(content.ToJson(), Encoding.UTF8, "application/json"));

        if ((int)request.StatusCode == 500)
            throw new InternalServerErrorException(await request.Content.ReadAsStringAsync());

        if ((int)request.StatusCode == 400)
            throw new BadRequestException(await request.Content.ReadAsStringAsync());

        request.EnsureSuccessStatusCode();
    }

    public async ValueTask<RegistrationResult> RegisterAsync(RegisterUser registerUser, string query = "")
    {
        StringContent content = new(registerUser.ToJson(), Encoding.UTF8, "application/json");
        HttpResponseMessage request = await api.PostAsync(endpoint + "Register" + query, content);

        if ((int)request.StatusCode == 500)
            throw new InternalServerErrorException(await request.Content.ReadAsStringAsync());

        if ((int)request.StatusCode == 400)
            throw new BadRequestException(await request.Content.ReadAsStringAsync());

        request.EnsureSuccessStatusCode();
        return await request.Content.ReadAsAsync<RegistrationResult>();
    }

    public async Task TearDown(string ssoUserId)
    {
        cCoder.Security.Objects.Entities.SSOUser user = Database.Users
            .IgnoreQueryFilters()
            .FirstOrDefault(u => u.Id == ssoUserId);

        if (user != null)
        {
            List<cCoder.Security.Objects.Entities.Token> tokens = [.. Database.Tokens
                .IgnoreQueryFilters()
                .Where(t => t.UserName == user.Id)];

            List<cCoder.Security.Objects.Entities.SSOUserRole> userRoles = [.. Database.UserRoles
                .IgnoreQueryFilters()
                .Where(r => r.UserId == user.Id)];

            Database.Tokens.RemoveRange(tokens);
            Database.UserRoles.RemoveRange(userRoles);
            Database.Users.Remove(user);
            await Database.SaveChangesAsync();
        }
    }
}


