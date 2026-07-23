// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Services.Orchestrations;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using Moq;

namespace cCoder.Security.Tests.Orchestrations;

public partial class SSORoleOrchestrationServiceTests
{
    private readonly Mock<ISSORoleProcessingService> roleProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly ISSORoleOrchestrationService roleOrchestrationService;

    public SSORoleOrchestrationServiceTests()
    {
        roleProcessingServiceMock = new Mock<ISSORoleProcessingService>(MockBehavior.Strict);

        authorizationProcessingServiceMock =
            new Mock<IAuthorizationProcessingService>(MockBehavior.Strict);

        roleOrchestrationService = new SSORoleOrchestrationService(
            roleProcessingService: roleProcessingServiceMock.Object,
            authorizationProcessingService: authorizationProcessingServiceMock.Object);
    }
}