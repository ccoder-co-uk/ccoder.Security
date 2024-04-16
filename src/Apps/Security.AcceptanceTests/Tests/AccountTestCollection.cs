using cCoder.Security.AcceptanceTests.Clients;
using Xunit;

namespace cCoder.Security.AcceptanceTests.Tests
{
    [CollectionDefinition(nameof(AccountTestCollection))]
    public class AccountTestCollection :
        ICollectionFixture<AccountApiClient>,
        ICollectionFixture<RegisterApiClient>,
        ICollectionFixture<SSOUserApiClient>,
        ICollectionFixture<CookieSSOUserApiClient>
    {

    }
}
