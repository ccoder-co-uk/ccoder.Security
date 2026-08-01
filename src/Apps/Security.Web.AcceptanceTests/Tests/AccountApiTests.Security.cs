// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

public partial class AccountApiTests
{
    [Fact]
    public async Task ShouldNotDiscloseWhetherLoginAccountExistsAsync()
    {
        // Given
        using HttpClient api = webApplicationFactory.CreateClient();

        Auth knownAccount = new()
        {
            User = "TestUser",
            Pass = "Definitely-Wrong-Password-01!"
        };

        Auth unknownAccount = new()
        {
            User = $"unknown-{Guid.NewGuid():N}@example.com",
            Pass = "Definitely-Wrong-Password-01!"
        };

        // When
        using HttpResponseMessage knownResponse = await api.PostAsJsonAsync(
            requestUri: "Api/Account/Login",
            value: knownAccount);

        using HttpResponseMessage unknownResponse = await api.PostAsJsonAsync(
            requestUri: "Api/Account/Login",
            value: unknownAccount);

        string knownBody = await knownResponse.Content.ReadAsStringAsync();
        string unknownBody = await unknownResponse.Content.ReadAsStringAsync();

        // Then
        knownResponse.StatusCode.Should()
            .Be(expected: HttpStatusCode.Unauthorized);

        unknownResponse.StatusCode.Should()
            .Be(expected: knownResponse.StatusCode);

        unknownBody.Should()
            .Be(expected: knownBody);

        knownBody.Should()
            .NotContain(unexpected: knownAccount.User);

        unknownBody.Should()
            .NotContain(unexpected: unknownAccount.User);
    }

    [Fact]
    public async Task ShouldNotDiscloseWhetherPasswordRecoveryAccountExistsAsync()
    {
        // Given
        using HttpClient api = webApplicationFactory.CreateClient();

        SSOUser knownUser = userApiClient.Database.Users
            .IgnoreQueryFilters()
            .First(predicate: user => user.Id == "TestUser");

        ForgotPasswordRequest knownAccount = new()
        {
            Email = knownUser.Email
        };

        ForgotPasswordRequest unknownAccount = new()
        {
            Email = $"unknown-{Guid.NewGuid():N}@example.com"
        };

        // When
        using HttpResponseMessage knownResponse = await api.PostAsJsonAsync(
            requestUri: "Api/Account/ForgotPassword",
            value: knownAccount);

        using HttpResponseMessage unknownResponse = await api.PostAsJsonAsync(
            requestUri: "Api/Account/ForgotPassword",
            value: unknownAccount);

        string knownBody = await knownResponse.Content.ReadAsStringAsync();
        string unknownBody = await unknownResponse.Content.ReadAsStringAsync();

        // Then
        knownResponse.StatusCode.Should()
            .Be(expected: HttpStatusCode.OK);

        unknownResponse.StatusCode.Should()
            .Be(expected: knownResponse.StatusCode);

        unknownBody.Should()
            .Be(expected: knownBody);

        knownBody.Should()
            .NotContain(unexpected: knownAccount.Email);

        unknownBody.Should()
            .NotContain(unexpected: unknownAccount.Email);
    }
}