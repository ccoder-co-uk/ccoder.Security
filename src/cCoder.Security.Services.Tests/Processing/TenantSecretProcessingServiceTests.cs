using cCoder.Security.Data;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;
using cCoder.Security.Services.Processing;
using cCoder.Security.Services.Processing.Interfaces;
using Moq;
using Tynamix.ObjectFiller;

namespace cCoder.Security.Services.Tests.Processing;

public partial class TenantSecretProcessingServiceTests
{
    private readonly Mock<ITenantSecretService> tenantSecretServiceMock;
    private readonly Mock<ISymmetricCrypto<string>> cryptoMock;
    private readonly ITenantSecretProcessingService tenantSecretProcessingService;

    public TenantSecretProcessingServiceTests()
    {
        tenantSecretServiceMock = new Mock<ITenantSecretService>();
        cryptoMock = new Mock<ISymmetricCrypto<string>>();
        tenantSecretProcessingService =
            new TenantSecretProcessingService(tenantSecretServiceMock.Object, cryptoMock.Object);
    }

    private TenantSecret[] RandomTenantSecrets() =>
        Enumerable.Range(1, new Random().Next(1, 20))
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
