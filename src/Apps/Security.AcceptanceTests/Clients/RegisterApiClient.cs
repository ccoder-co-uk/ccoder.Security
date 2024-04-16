using cCoder.Security.AcceptanceTests.Exceptions;
using cCoder.Security.AcceptanceTests.Tests.Models;
using cCoder.Security.Data.EF;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Security.AcceptanceTests.Clients;
using SSO.AcceptanceTests;
using System.Text;

namespace cCoder.Security.AcceptanceTests.Clients;

public class RegisterApiClient
{
    readonly WebApplicationFactory<SecurityMSSQL.Program> webApplicationFactory;
    readonly HttpClient api;

    public SecurityDbContext Database { get; set; }

    const string endpoint = "Api/Account/";

    public RegisterApiClient()
    {
        webApplicationFactory = new();
        webApplicationFactory.EnsureSSOSetupForTesting();

        api = webApplicationFactory.CreateClient();
        api.Authenticate("TestUser", "TestPass01!").Wait();

        using var scope = webApplicationFactory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;

        Database = scopedServices.GetRequiredService<ISecurityDbContextFactory>()
                    .CreateDbContext();
    }

    public async ValueTask PostAsync(string query, object content)
    {
        var request = await api.PostAsync(endpoint + query, new StringContent(content.ToJson(), Encoding.UTF8, "application/json"));

        if ((int)request.StatusCode == 500)
            throw new InternalServerErrorException(await request.Content.ReadAsStringAsync());

        if ((int)request.StatusCode == 400)
            throw new BadRequestException(await request.Content.ReadAsStringAsync());

        request.EnsureSuccessStatusCode();
    }

    public async ValueTask<RegistrationResult> RegisterAsync(RegisterUser registerUser, string query = "")
    {
        var content = new StringContent(registerUser.ToJson(), Encoding.UTF8, "application/json");
        var request = await api.PostAsync(endpoint + "Register" + query, content);

        if ((int)request.StatusCode == 500)
            throw new InternalServerErrorException(await request.Content.ReadAsStringAsync());

        if ((int)request.StatusCode == 400)
            throw new BadRequestException(await request.Content.ReadAsStringAsync());

        request.EnsureSuccessStatusCode();
        return await request.Content.ReadAsAsync<RegistrationResult>();
    }

    public async Task TearDown(string ssoUserId)
    {
        var user = Database.Users
            .IgnoreQueryFilters()
            .FirstOrDefault(u => u.Id == ssoUserId);

        if (user != null)
        {
            var tokens = Database.Tokens
                .IgnoreQueryFilters()
                .Where(t => t.UserName == user.Id)
                .ToList();

            var userRoles = Database.UserRoles
                .IgnoreQueryFilters()
                .Where(r => r.UserId == user.Id)
                .ToList();

            Database.Tokens.RemoveRange(tokens);
            Database.UserRoles.RemoveRange(userRoles);
            Database.Users.Remove(user);
            await Database.SaveChangesAsync();
        }
    }
}

