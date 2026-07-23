// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Bogus;
using cCoder.Security.Objects.DTOs;
using Security.AcceptanceTests;
using Security.AcceptanceTests.Clients;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

[Collection(nameof(AllTestsCollection))]
public partial class SSOUserApiTests(
    RegisterApiClient registerApiClient,
    SSOUserApiClient ssoUserApiClient,
    AccountApiClient accountApiClient)
{
    private static Auth RandomAuth(RegisterUser user)
                =>
        new()
        {
            User = user.Email,
            Pass = user.Password
        };

    private static RegisterUser[] RandomRegisterUsers() =>
        [.. Enumerable.Range(start: 1, count: new Random().Next(minValue:10, maxValue:20))
                .Select(selector: _ => RandomRegisterUser())];

    private static RegisterUser RandomRegisterUser() =>
        GetRegisterUserFiller()
            .Generate();

    private static Faker<RegisterUser> GetRegisterUserFiller()
    {
        Faker<RegisterUser> filler = new Faker<RegisterUser>()
            .RuleFor(property:r => r.DisplayName, setter:f => f.Name.FullName())
            .RuleFor(property:r => r.Email, setter:f => f.Internet.Email())
            .RuleFor(property:r => r.Password, setter:f => f.Internet.Password(prefix: "Cc123!"))
            .RuleFor(property: r => r.Culture, setter: f => f.Locale)
            .RuleFor(property: r => r.PhoneNumber, setter: f => f.Phone.PhoneNumber());

        return filler;
    }

    private Task TearDownUserAsync(string userId)
        =>
        accountApiClient.TearDown(ssoUserId: userId);
}