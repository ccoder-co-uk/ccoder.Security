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
public partial class AccountApiTests(
    AccountApiClient userApiClient,
    RegisterApiClient registerApiClient)
{
    private static Auth RandomAuth(RegisterUser user) =>
        new()
        {
            User = user.Email,
            Pass = user.Password
        };

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

    private Task TearDownUserAsync(string userId) =>
        userApiClient.TearDown(ssoUserId: userId);
}