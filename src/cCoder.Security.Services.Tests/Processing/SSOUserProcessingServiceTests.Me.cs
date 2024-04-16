using cCoder.Security.Objects.Entities;
using Moq;
using Xunit;

namespace cCoder.Security.Services.Tests.Processing;

public partial class SSOUserProcessingServiceTests
{
	[Fact]
	public void MeShouldWorkAsExpected()
	{
		//given
		SSOUser expectedSSOUser = RandomSSOUser();

            ssoUserServiceMock.Setup(identityBrokerMock =>
			identityBrokerMock.Me())
			.Returns(expectedSSOUser);

		//when
		SSOUser actualSSOUser = ssoUserProcessingService.Me();

            //then
            ssoUserServiceMock.Verify(identityBrokerMock =>
			identityBrokerMock.Me(),
			Times.Once());

		ssoUserServiceMock.VerifyNoOtherCalls();
		passwordEncryptionBrokerMock.VerifyNoOtherCalls();
	}
}

