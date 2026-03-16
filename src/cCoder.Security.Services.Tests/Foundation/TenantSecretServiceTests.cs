using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation;
using cCoder.Security.Services.Foundation.Interfaces;
using Moq;
using Tynamix.ObjectFiller;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class TenantSecretServiceTests
{
    private readonly Mock<ITenantSecretBroker> tenantSecretBrokerMock;
    private readonly ITenantSecretService tenantSecretService;

    public TenantSecretServiceTests()
    {
        tenantSecretBrokerMock = new Mock<ITenantSecretBroker>();
        tenantSecretService = new TenantSecretService(tenantSecretBrokerMock.Object);
    }

    private TenantSecret[] RandomTenantSecrets() =>
        Enumerable.Range(1, new Random().Next(10, 20))
            .Select(_ => RandomTenantSecret())
            .ToArray();

    private TenantSecret RandomTenantSecret() =>
        GetTenantSecretFiller().Create();

    private Filler<TenantSecret> GetTenantSecretFiller()
    {
        Filler<TenantSecret> filler = new();

        filler.Setup()
            .OnType<DateTimeOffset>().Use(DateTimeOffset.UtcNow)
            .OnProperty(secret => secret.Tenant).IgnoreIt();

        return filler;
    }
}
