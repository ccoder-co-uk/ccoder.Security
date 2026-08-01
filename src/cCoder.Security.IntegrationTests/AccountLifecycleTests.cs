// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.EF;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Entities;
using cCoder.Security.Exposures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace cCoder.Security.IntegrationTests;

[Collection(nameof(AllTestsCollection))]
public partial class AccountLifecycleTests
{
    private const string DefaultPassword = "TestPass01!";
    private const string UpdatedPassword = "ChangedPass01!";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly AccountLifecycleFixture fixture;
    private readonly HttpClient api;

    public AccountLifecycleTests(AccountLifecycleFixture fixture)
    {
        this.fixture = fixture;
        api = fixture.Api;
    }

    private SecurityDbContext CreateSecurityDbContext()
    {
        return fixture.CreateSecurityDbContext();
    }

    private static RegisterUser CreateRegisterUser(string name, string password = DefaultPassword) =>
        new()
        {
            DisplayName = $"{name} User",
            Email = $"{name}.{Guid.NewGuid():N}@example.com",
            Password = password,
            Culture = "en-GB",
            PhoneNumber = "01234567890"
        };

    private static Auth CreateAuth(RegisterUser user, string password = null) =>
        new()
        {
            User = user.Email,
            Pass = password ?? user.Password
        };

    private async ValueTask<(SSOUser User, string Token)> RegisterAsync(RegisterUser user)
    {
        HttpResponseMessage response = await api.PostAsJsonAsync(requestUri: "/Api/Account/Register", value: user);

        response.StatusCode
            .Should()
            .Be(expected: HttpStatusCode.Created);

        response.EnsureSuccessStatusCode();

        return await ReadUserTokenResultAsync(response: response);
    }

    private async ValueTask<(SSOUser User, string Token)> InviteAsync(RegisterUser user)
    {
        HttpResponseMessage response = await api.PostAsJsonAsync(requestUri: "/Api/Account/Invite", value: user);

        response.StatusCode
            .Should()
            .Be(expected: HttpStatusCode.Created);

        response.EnsureSuccessStatusCode();

        return await ReadUserTokenResultAsync(response: response);
    }

    private async ValueTask<string> ResendInviteAsync(string userId)
    {
        HttpResponseMessage response = await api.PostAsync(
requestUri: $"/Api/Account/ResendInvite?userId={WebUtility.UrlEncode(value: userId)}",
            content: null);

        response.EnsureSuccessStatusCode();

        using JsonDocument document = JsonDocument.Parse(json: await response.Content.ReadAsStringAsync());

        return document.RootElement.GetProperty(propertyName: "token")
                   .GetString();
    }

    private async ValueTask ConfirmRegistrationAsync(string token)
    {
        HttpResponseMessage response = await api.PostAsync(
requestUri: $"/Api/Account/ConfirmRegistration?confirmationToken={WebUtility.UrlEncode(value: token)}",
            content: null);

        response.EnsureSuccessStatusCode();
    }

    private async ValueTask AcceptInviteAsync(string userId, string token, RegisterUser user)
    {
        HttpResponseMessage response = await api.PostAsJsonAsync(
requestUri: $"/Api/Account/AcceptInvite?userId={WebUtility.UrlEncode(value: userId)}&inviteToken={WebUtility.UrlEncode(value: token)}",
value: user);

        response.EnsureSuccessStatusCode();
    }

    private async ValueTask<Token> LoginAsync(Auth auth)
    {
        HttpResponseMessage response = await api.PostAsJsonAsync(requestUri: "/Api/Account/Login", value: auth);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Token>(options: JsonOptions);
    }

    private async ValueTask LogoutAsync()
    {
        HttpResponseMessage response = await api.PostAsync(requestUri: "/Api/Account/Logout", content: null);
        response.EnsureSuccessStatusCode();
    }

    private async ValueTask RequestPasswordResetAsync(string email)
    {
        HttpResponseMessage response = await api.PostAsJsonAsync(
requestUri: "/Api/Account/ForgotPassword",
value: new ForgotPasswordRequest { Email = email });

        response.EnsureSuccessStatusCode();
    }

    private async ValueTask<Token> RequestPasswordResetTokenAsync(string email)
    {
        using IServiceScope scope = fixture.WebApplicationFactory.Services.CreateScope();

        IAuthenticationManager authenticationManager = scope.ServiceProvider
            .GetRequiredService<IAuthenticationManager>();

        return await authenticationManager.ForgotPasswordAsync(email: email);
    }

    private async ValueTask ConfirmForgotPasswordAsync(string token, string userId, string password)
    {
        ConfirmForgotPasswordRequest request = new()
        {
            Token = token,
            UserId = userId,
            NewPassword = password,
            ConfirmPassword = password
        };

        HttpResponseMessage response = await api.PostAsJsonAsync(
requestUri: "/Api/Account/ConfirmForgotPassword",
value: request);

        response.EnsureSuccessStatusCode();
    }

    private ValueTask<HttpResponseMessage> TryLoginAsync(Auth auth) =>
        new(api.PostAsJsonAsync(requestUri: "/Api/Account/Login", value: auth));

    private async ValueTask AssertLoginRejectedAsync(Auth auth)
    {
        HttpResponseMessage response = await TryLoginAsync(auth: auth);

        response.IsSuccessStatusCode.Should()
            .BeFalse();

        response.StatusCode.Should()
            .Be(expected: HttpStatusCode.Unauthorized);
    }

    private SSOUser FindUser(string userId)
    {
        using SecurityDbContext database = CreateSecurityDbContext();

        return database.Users
            .IgnoreQueryFilters()
            .First(predicate: user => user.Id == userId);
    }

    private Token FindStoredToken(string selector)
    {
        using SecurityDbContext database = CreateSecurityDbContext();

        return database.Tokens
            .IgnoreQueryFilters()
            .First(predicate: token => token.Id == selector);
    }

    private async ValueTask<(SSOUser User, string Token)> ReadUserTokenResultAsync(
        HttpResponseMessage response)
    {
        using JsonDocument document = JsonDocument.Parse(json: await response.Content.ReadAsStringAsync());

        SSOUser user = JsonSerializer.Deserialize<SSOUser>(
json: document.RootElement.GetProperty(propertyName: "user")
          .GetRawText(),
options: JsonOptions);

        string token = document.RootElement.GetProperty(propertyName: "token")
                           .GetString();

        return (user, token);
    }
}