using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;
using Moq;

namespace cCoder.Security.Services.Tests.Orchestration;

public partial class TenantOrchestrationServiceTests
{
    private readonly Mock<ITenantProcessingService> tenantProcessingServiceMock;
    private readonly Mock<ISSORoleOrchestrationService> roleOrchestrationServiceMock;
    private readonly Mock<ISSOUserRoleOrchestrationService> userRoleOrchestrationServiceMock;
    private readonly Mock<ISSOAuthorizationBroker> authorizationBrokerMock;
    private readonly ITenantOrchestrationService tenantOrchestrationService;

    public TenantOrchestrationServiceTests()
    {
        tenantProcessingServiceMock = new Mock<ITenantProcessingService>(MockBehavior.Strict);
        roleOrchestrationServiceMock = new Mock<ISSORoleOrchestrationService>(MockBehavior.Strict);
        userRoleOrchestrationServiceMock = new Mock<ISSOUserRoleOrchestrationService>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<ISSOAuthorizationBroker>(MockBehavior.Strict);
        tenantOrchestrationService = new TenantOrchestrationService(
            tenantProcessingServiceMock.Object,
            roleOrchestrationServiceMock.Object,
            userRoleOrchestrationServiceMock.Object,
            authorizationBrokerMock.Object);
    }
}
