using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace cCoder.Security.Services.Tests.Orchestration;

public partial class SSOUserRegistrationOrchestrationServiceTests
{
    private readonly Mock<ISSOUserProcessingService> ssoUserProcessingServiceMock;
    private readonly Mock<ITokenProcessingService> tokenProcessingServiceMock;
    private readonly Mock<ISSORoleProcessingService> roleProcessingServiceMock;
    private readonly Mock<ISSOUserRoleProcessingService> userRoleProcessingServiceMock;
    private readonly Mock<ISSOUserRoleOrchestrationService> userRoleOrchestrationServiceMock;
    private readonly Mock<ILogger<SSOUserRegistrationOrchestrationService>> loggerMock;
    private readonly ISSOUserOrchestrationService ssoUserRegistrationOrchestrationService;

    public SSOUserRegistrationOrchestrationServiceTests()
    {
        ssoUserProcessingServiceMock = new Mock<ISSOUserProcessingService>(MockBehavior.Strict);
        tokenProcessingServiceMock = new Mock<ITokenProcessingService>(MockBehavior.Strict);
        roleProcessingServiceMock = new Mock<ISSORoleProcessingService>(MockBehavior.Strict);
        userRoleProcessingServiceMock = new Mock<ISSOUserRoleProcessingService>(MockBehavior.Strict);
        userRoleOrchestrationServiceMock = new Mock<ISSOUserRoleOrchestrationService>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger<SSOUserRegistrationOrchestrationService>>();
        ssoUserRegistrationOrchestrationService = new SSOUserRegistrationOrchestrationService(
            ssoUserProcessingServiceMock.Object,
            tokenProcessingServiceMock.Object,
            roleProcessingServiceMock.Object,
            userRoleProcessingServiceMock.Object,
            userRoleOrchestrationServiceMock.Object,
            loggerMock.Object);
    }
}
