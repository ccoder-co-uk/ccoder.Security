using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Services.Orchestrations;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using Moq;

namespace cCoder.Security.Tests.Orchestrations;

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


