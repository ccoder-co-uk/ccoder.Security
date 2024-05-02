using Security.AcceptanceTests.Clients;
using Xunit;

namespace Security.AcceptanceTests;

[CollectionDefinition(nameof(AllTestsCollection))]
public class AllTestsCollection :
    ICollectionFixture<AccountApiClient>,
    ICollectionFixture<RegisterApiClient>,
    ICollectionFixture<SSOUserApiClient>,
    ICollectionFixture<CookieSSOUserApiClient>
{

}
