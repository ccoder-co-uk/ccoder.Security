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
        //given
        SSOUser expectedSSOUser = RandomSSOUser();

        ssoUserServiceMock.Setup(expression: identityBrokerMock =>
        identityBrokerMock.Me())
        .Returns(value: expectedSSOUser);

        //when
        SSOUser actualSSOUser = ssoUserProcessingService.Me();

        //then
        ssoUserServiceMock.Verify(expression: identityBrokerMock =>
        identityBrokerMock.Me(),
times: Times.Once());

        ssoUserServiceMock.VerifyNoOtherCalls();
        passwordEncryptionBrokerMock.VerifyNoOtherCalls();
    }
}