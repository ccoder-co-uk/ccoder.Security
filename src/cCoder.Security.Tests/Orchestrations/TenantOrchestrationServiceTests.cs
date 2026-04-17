using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using Moq;

namespace cCoder.Security.Tests.Orchestrations;

public partial class TenantOrchestrationServiceTests
{
    private readonly Mock<ITenantProcessingService> tenantProcessingServiceMock;
    private readonly Mock<ISSOUserProcessingService> userProcessingServiceMock;
    private readonly Mock<ISSORoleOrchestrationService> roleOrchestrationServiceMock;
    private readonly Mock<ISSOUserRoleOrchestrationService> userRoleOrchestrationServiceMock;
    private readonly Mock<ISSOAuthorizationBroker> authorizationBrokerMock;
    private readonly ITenantOrchestrationService tenantOrchestrationService;

    public TenantOrchestrationServiceTests()
    {
        tenantProcessingServiceMock = new Mock<ITenantProcessingService>(MockBehavior.Strict);
        userProcessingServiceMock = new Mock<ISSOUserProcessingService>(MockBehavior.Strict);
        roleOrchestrationServiceMock = new Mock<ISSORoleOrchestrationService>(MockBehavior.Strict);
        userRoleOrchestrationServiceMock = new Mock<ISSOUserRoleOrchestrationService>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<ISSOAuthorizationBroker>(MockBehavior.Strict);
        tenantOrchestrationService = new TenantOrchestrationService(
            tenantProcessingServiceMock.Object,
            userProcessingServiceMock.Object,
            roleOrchestrationServiceMock.Object,
            userRoleOrchestrationServiceMock.Object,
            authorizationBrokerMock.Object);
    }
}


