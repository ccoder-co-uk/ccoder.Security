using Bogus;
using cCoder.Security.Objects.DTOs;
using Security.AcceptanceTests;
using Security.AcceptanceTests.Clients;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

[Collection(nameof(AllTestsCollection))]
public partial class RegisterApiTests(
    RegisterApiClient userApiClient, 
    AccountApiClient accountApiClient,
    SSOUserApiClient ssoUserApiClient)
{
    private static RegisterUser RandomRegisterUser()
        => GetRegisterUserFiller().Generate();

    private static Faker<RegisterUser> GetRegisterUserFiller()
    {
        Faker<RegisterUser> filler = new Faker<RegisterUser>()
            .RuleFor(r => r.DisplayName, f => f.Name.FullName())
            .RuleFor(r => r.Email, f => f.Internet.Email())
            .RuleFor(r => r.Password, f => f.Internet.Password(prefix: "Cc123!"))
            .RuleFor(r => r.Culture, f => f.Locale)
            .RuleFor(r => r.PhoneNumber, f => f.Phone.PhoneNumber());

        return filler;
    }

    private async Task TearDownUserAsync(string userId) => 
        await userApiClient.TearDown(userId);
}


