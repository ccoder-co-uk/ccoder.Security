using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Services.Orchestration;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;
using Moq;

namespace cCoder.Security.Services.Tests.Orchestration;

public partial class SSORoleOrchestrationServiceTests
{
    private readonly Mock<ISSORoleProcessingService> roleProcessingServiceMock;
    private readonly Mock<ISSOAuthorizationBroker> authorizationBrokerMock;
    private readonly ISSORoleOrchestrationService roleOrchestrationService;

    public SSORoleOrchestrationServiceTests()
    {
        roleProcessingServiceMock = new Mock<ISSORoleProcessingService>(MockBehavior.Strict);
        authorizationBrokerMock = new Mock<ISSOAuthorizationBroker>(MockBehavior.Strict);
        roleOrchestrationService = new SSORoleOrchestrationService(
            roleProcessingServiceMock.Object,
            authorizationBrokerMock.Object);
    }
}
