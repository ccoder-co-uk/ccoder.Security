// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Services.Foundations.Events;
using cCoder.Security.Services.Orchestrations;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace cCoder.Security.Tests.Orchestrations;

public partial class SSOUserOrchestrationServiceTests
{
    private readonly Mock<ISSOUserProcessingService> ssoUserProcessingServiceMock;
    private readonly Mock<ITokenProcessingService> tokenProcessingServiceMock;
    private readonly Mock<ISSORoleProcessingService> roleProcessingServiceMock;
    private readonly Mock<ISSOUserRoleProcessingService> userRoleProcessingServiceMock;
    private readonly Mock<ISSOUserRoleOrchestrationService> userRoleOrchestrationServiceMock;
    private readonly Mock<IAccountEventService> accountEventServiceMock;
    private readonly Mock<ILogger<SSOUserOrchestrationService>> loggerMock;
    private readonly ISSOUserOrchestrationService ssoUserOrchestrationService;

    public SSOUserOrchestrationServiceTests()
    {
        ssoUserProcessingServiceMock = new Mock<ISSOUserProcessingService>(MockBehavior.Strict);
        tokenProcessingServiceMock = new Mock<ITokenProcessingService>(MockBehavior.Strict);
        roleProcessingServiceMock = new Mock<ISSORoleProcessingService>(MockBehavior.Strict);
        userRoleProcessingServiceMock = new Mock<ISSOUserRoleProcessingService>(MockBehavior.Strict);
        userRoleOrchestrationServiceMock = new Mock<ISSOUserRoleOrchestrationService>(MockBehavior.Strict);
        accountEventServiceMock = new Mock<IAccountEventService>(MockBehavior.Strict);
        loggerMock = new Mock<ILogger<SSOUserOrchestrationService>>();
        ssoUserOrchestrationService = new SSOUserOrchestrationService(
            ssoUserProcessingServiceMock.Object,
            tokenProcessingServiceMock.Object,
            roleProcessingServiceMock.Object,
            userRoleProcessingServiceMock.Object,
            userRoleOrchestrationServiceMock.Object,
            accountEventServiceMock.Object,
            loggerMock.Object);
    }

    private void SetupRegistrationCreatedEvent(SSOUser user, RegisterUser registerForm, string token) =>
        accountEventServiceMock
            .Setup(expression: service => service.RaiseRegistrationCreatedEventAsync(user, registerForm, token))
            .Returns(value: ValueTask.CompletedTask);
}