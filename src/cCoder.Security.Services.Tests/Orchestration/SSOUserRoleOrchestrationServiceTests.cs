using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Services.Orchestration;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;
using Moq;

namespace cCoder.Security.Services.Tests.Orchestration;

public partial class SSOUserRoleOrchestrationServiceTests
{
    private readonly Mock<ISSOUserRoleProcessingService> userRoleProcessingServiceMock;
    private readonly Mock<ISSOAuthorizationBroker> authorizationBrokerMock;
    private readonly ISSOUserRoleOrchestrationService userRoleOrchestrationService;

    public SSOUserRoleOrchestrationServiceTests()
    {
        userRoleProcessingServiceMock = new Mock<ISSOUserRoleProcessingService>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<ISSOAuthorizationBroker>(MockBehavior.Strict);
        userRoleOrchestrationService = new SSOUserRoleOrchestrationService(
            userRoleProcessingServiceMock.Object,
            authorizationBrokerMock.Object);
    }
}
