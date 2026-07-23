// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Services.Orchestrations;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using Moq;

namespace cCoder.Security.Tests.Orchestrations;

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