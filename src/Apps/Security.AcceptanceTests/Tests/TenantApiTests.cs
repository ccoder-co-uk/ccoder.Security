using System.Reflection.PortableExecutable;
using Bogus;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using Security.AcceptanceTests;
using Security.AcceptanceTests.Clients;
using Tynamix.ObjectFiller;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests;

[Collection(nameof(AllTestsCollection))]
public partial class TenantApiTests
	{
    private readonly RegisterApiClient registerApiClient;
    private readonly AccountApiClient accountApiClient;
    private readonly SSOUserApiClient ssoUserApiClient;

    public TenantApiTests(RegisterApiClient userApiClient, AccountApiClient accountApiClient, SSOUserApiClient ssoUserApiClient)
    {
        this.registerApiClient = userApiClient;
        this.accountApiClient = accountApiClient;
        this.ssoUserApiClient = ssoUserApiClient;
    }

    private static Auth RandomAuth(RegisterUser user)
        => new()
        {
            User = user.Email,
            Pass = user.Password
        };

    private static RegisterUser RandomRegisterUser()
        => GetRegisterUserFiller().Generate();

    private static Faker<RegisterUser> GetRegisterUserFiller()
    {
        Faker<RegisterUser> filler = new Faker<RegisterUser>()
            .RuleFor(r => r.DisplayName, f => f.Name.FullName())
            .RuleFor(r => r.Email, f => f.Internet.Email())
            .RuleFor(r => r.Password, f => f.Internet.Password() + f.Random.Number(5) + "!")
            .RuleFor(r => r.Culture, f => f.Locale)
            .RuleFor(r => r.PhoneNumber, f => f.Phone.PhoneNumber());

        return filler;
    }

    private async Task TearDownUserAsync(string userId)
        => await registerApiClient.TearDown(userId);

    private static Tenant RandomTenant() =>
        GetTenantFillter().Create();

    private static Filler<Tenant> GetTenantFillter()
    {
        Filler<Tenant> filler = new();
        filler.Setup()
            .OnType<DateTimeOffset>().Use(DateTimeOffset.UtcNow)
            .OnType<DateTimeOffset?>().IgnoreIt()
            .OnProperty(t => t.Id).Use(RandomString())
            .OnProperty(t => t.CreatedBy).Use(RandomString())
            .OnProperty(t => t.LastUpdatedBy).IgnoreIt()
            .OnProperty(t => t.Description).Use(RandomString())
            .OnProperty(t => t.Name).Use(RandomString())
            .OnProperty(t => t.Roles).IgnoreIt()
            .OnProperty(t => t.UserEvents).IgnoreIt()
            .OnProperty(t => t.Analysis).IgnoreIt();

        return filler;
    }
    private static string RandomString() =>
        new MnemonicString().GetValue();
}

