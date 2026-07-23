// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Services.Aggregations;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using Moq;
using Tynamix.ObjectFiller;

namespace cCoder.Security.Tests.Aggregations;

public partial class AuthenticationAggregationServiceTests
{
    private readonly Mock<ISessionProcessingService> sessionProcessingServiceMock;
    private readonly Mock<ITokenProcessingService> tokenProcessingServiceMock;
    private readonly Mock<ISSOUserProcessingService> ssoUserProcessingServiceMock;
    private readonly Mock<IAccountEventProcessingService> accountEventProcessingServiceMock;
    private readonly Mock<ILoggingProcessingService> loggingProcessingServiceMock;
    private readonly IAuthenticationAggregationService authenticationAggregationService;

    public AuthenticationAggregationServiceTests()
    {
        sessionProcessingServiceMock = new Mock<ISessionProcessingService>();
        tokenProcessingServiceMock = new Mock<ITokenProcessingService>();
        ssoUserProcessingServiceMock = new Mock<ISSOUserProcessingService>();

        accountEventProcessingServiceMock =
            new Mock<IAccountEventProcessingService>();

        loggingProcessingServiceMock =
            new Mock<ILoggingProcessingService>();

        authenticationAggregationService = new AuthenticationAggregationService(
            ssoUserProcessingService: ssoUserProcessingServiceMock.Object,
            tokenProcessingService: tokenProcessingServiceMock.Object,
            sessionProcessingService: sessionProcessingServiceMock.Object,
            accountEventProcessingService:
                accountEventProcessingServiceMock.Object,
            loggingProcessingService: loggingProcessingServiceMock.Object);
    }

    private static string RandomString() =>
        new MnemonicString().GetValue();
}