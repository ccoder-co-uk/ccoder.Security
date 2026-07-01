using cCoder.Security.Services.Orchestrations;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Foundations.Events;
using cCoder.Security.Services.Processings.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using Tynamix.ObjectFiller;

namespace cCoder.Security.Tests.Orchestrations;

public partial class AuthenticationOrchestrationServiceTests
{
    private readonly Mock<ISessionProcessingService> sessionProcessingServiceMock;
    private readonly Mock<ITokenProcessingService> tokenProcessingServiceMock;
    private readonly Mock<ISSOUserProcessingService> ssoUserProcessingServiceMock;
    private readonly Mock<IAccountEventService> accountEventServiceMock;
    private readonly Mock<ILogger<AuthenticationOrchestrationService>> loggerMock;
    private readonly IAuthenticationOrchestrationService authenticationOrchestrationService;

    public AuthenticationOrchestrationServiceTests()
    {
        sessionProcessingServiceMock = new Mock<ISessionProcessingService>();
        tokenProcessingServiceMock = new Mock<ITokenProcessingService>();
        ssoUserProcessingServiceMock = new Mock<ISSOUserProcessingService>();
        accountEventServiceMock = new Mock<IAccountEventService>();
        loggerMock = new Mock<ILogger<AuthenticationOrchestrationService>>();
        authenticationOrchestrationService = new AuthenticationOrchestrationService(
            ssoUserProcessingServiceMock.Object,
            tokenProcessingServiceMock.Object,
            sessionProcessingServiceMock.Object,
            accountEventServiceMock.Object,
            loggerMock.Object);
    }

    private static string RandomString() => 
        new MnemonicString().GetValue();
}

