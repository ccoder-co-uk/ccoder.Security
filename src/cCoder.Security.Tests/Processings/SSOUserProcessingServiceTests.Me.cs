// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Processings;

public partial class SSOUserProcessingServiceTests
{
    [Fact]
    public void MeShouldWorkAsExpected()
    {
        // Given
        SSOUser expectedSSOUser = RandomSSOUser();

        ssoUserServiceMock.Setup(expression: identityBrokerMock =>
        identityBrokerMock.Me())
        .Returns(value: expectedSSOUser);

        // When
        SSOUser actualSSOUser = ssoUserProcessingService.Me();

        // Then
        ssoUserServiceMock.Verify(expression: identityBrokerMock =>
        identityBrokerMock.Me(),
times: Times.Once());

        ssoUserServiceMock.VerifyNoOtherCalls();
        passwordEncryptionBrokerMock.VerifyNoOtherCalls();
    }
}