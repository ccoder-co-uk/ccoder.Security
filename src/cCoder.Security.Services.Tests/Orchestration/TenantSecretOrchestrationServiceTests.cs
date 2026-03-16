using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;
using Moq;
using Tynamix.ObjectFiller;

namespace cCoder.Security.Services.Tests.Orchestration;

public partial class TenantSecretOrchestrationServiceTests
{
    private readonly Mock<ITenantSecretProcessingService> tenantSecretProcessingServiceMock;
    private readonly Mock<ISSOAuthorizationBroker> authBrokerMock;
    private readonly ITenantSecretOrchestrationService tenantSecretOrchestrationService;

    public TenantSecretOrchestrationServiceTests()
    {
        tenantSecretProcessingServiceMock = new Mock<ITenantSecretProcessingService>();
        authBrokerMock = new Mock<ISSOAuthorizationBroker>();
        tenantSecretOrchestrationService =
            new TenantSecretOrchestrationService(
                tenantSecretProcessingServiceMock.Object,
                authBrokerMock.Object);
    }

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

    private static SSOUser CreateUserWithTenantSecretReadRole(string tenantId) =>
        new()
        {
            Id = "user",
            Roles =
            [
                new SSOUserRole
                {
                    Role = new SSORole
                    {
                        TenantId = tenantId,
                        Privs = "tenantsecret_read"
                    }
                }
            ]
        };
}
