// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Services.Orchestrations;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using Moq;

namespace cCoder.Security.Tests.Orchestrations;

public partial class SSOUserRoleOrchestrationServiceTests
{
    private readonly Mock<ISSOUserRoleProcessingService> userRoleProcessingServiceMock;
    private readonly Mock<IAuthorizationProcessingService> authorizationProcessingServiceMock;
    private readonly ISSOUserRoleOrchestrationService userRoleOrchestrationService;

    public SSOUserRoleOrchestrationServiceTests()
    {
        userRoleProcessingServiceMock = new Mock<ISSOUserRoleProcessingService>(MockBehavior.Strict);

        authorizationProcessingServiceMock =
            new Mock<IAuthorizationProcessingService>(MockBehavior.Strict);

        userRoleOrchestrationService = new SSOUserRoleOrchestrationService(
            userRoleProcessingService: userRoleProcessingServiceMock.Object,
            authorizationProcessingService: authorizationProcessingServiceMock.Object);
    }
}