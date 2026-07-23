// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Events;
using cCoder.Security.Services.Aggregations;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using Moq;

namespace cCoder.Security.Tests.Aggregations;

public partial class RegistrationAggregationServiceTests
{
    private readonly Mock<ISSOUserProcessingService> ssoUserProcessingServiceMock;
    private readonly Mock<ITenantProcessingService> tenantProcessingServiceMock;
    private readonly Mock<ITokenProcessingService> tokenProcessingServiceMock;
    private readonly Mock<ISSORoleProcessingService> roleProcessingServiceMock;
    private readonly Mock<ISSOUserRoleProcessingService> userRoleProcessingServiceMock;
    private readonly Mock<IAccountEventProcessingService> accountEventProcessingServiceMock;
    private readonly Mock<ILoggingProcessingService> loggingProcessingServiceMock;
    private readonly IRegistrationAggregationService registrationAggregationService;

    public RegistrationAggregationServiceTests()
    {
        ssoUserProcessingServiceMock = new Mock<ISSOUserProcessingService>(MockBehavior.Strict);
        tenantProcessingServiceMock = new Mock<ITenantProcessingService>(MockBehavior.Strict);
        tokenProcessingServiceMock = new Mock<ITokenProcessingService>(MockBehavior.Strict);
        roleProcessingServiceMock = new Mock<ISSORoleProcessingService>(MockBehavior.Strict);
        userRoleProcessingServiceMock = new Mock<ISSOUserRoleProcessingService>(MockBehavior.Strict);

        accountEventProcessingServiceMock =
            new Mock<IAccountEventProcessingService>(MockBehavior.Strict);

        loggingProcessingServiceMock =
            new Mock<ILoggingProcessingService>(MockBehavior.Strict);

        registrationAggregationService = new RegistrationAggregationService(
            ssoUserProcessingService: ssoUserProcessingServiceMock.Object,
            tenantProcessingService: tenantProcessingServiceMock.Object,
            tokenProcessingService: tokenProcessingServiceMock.Object,
            roleProcessingService: roleProcessingServiceMock.Object,
            userRoleProcessingService: userRoleProcessingServiceMock.Object,
            accountEventProcessingService:
                accountEventProcessingServiceMock.Object,
            loggingProcessingService: loggingProcessingServiceMock.Object);
    }

    private void SetupRegistrationCreatedEvent(SSOUser user, RegisterUser registerForm, string token) =>
        accountEventProcessingServiceMock
            .Setup(expression: service => service.RaiseSecurityAccountEventRequestAsync(
accountEventRequest:                It.Is<SecurityAccountEventRequest>(match:request =>
                    request.Kind == SecurityAccountEventKind.RegistrationCreated
                    && request.User == user
                    && request.RegisterForm == registerForm
                    && request.Token == token)))
            .Returns(value: ValueTask.CompletedTask);
}