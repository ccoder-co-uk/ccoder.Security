using cCoder.Security.Objects.Entities;
using FluentAssertions;
using Moq;
using System.Security;
using Xunit;

namespace cCoder.Security.Services.Tests.Processing;

public partial class SSOUserProcessingServiceTests
	{
		[Fact]
		public async Task FindByUserAndPasswordWorksAsExpected()
		{
			//given
			string inputPassword = RandomString();

			IQueryable<SSOUser> ssoUsersInService = RandomSSOUsers()
				.AsQueryable();

			foreach (SSOUser user in ssoUsersInService)
				user.LockoutEnabled = false;

			ssoUserServiceMock.Setup(ssoUserServiceMock =>
				ssoUserServiceMock.GetAllSSOUsers(true))
				.Returns(ssoUsersInService);

        SSOUser expectedSSOUser = ssoUsersInService.First();

        passwordEncryptionBrokerMock.Setup(passwordEncryptionBrokerMock =>
				passwordEncryptionBrokerMock.EncryptedAndPlainTextAreEqual(expectedSSOUser.PasswordHash, inputPassword))
				.Returns(true);

			//when
			SSOUser actualSSOUser = await ssoUserProcessingService
				.FindByUserAndPasswordAsync(expectedSSOUser.Id, inputPassword);

			//then
			actualSSOUser.Should().BeEquivalentTo(expectedSSOUser);

			ssoUserServiceMock.Verify(ssoUserServiceMock =>
				ssoUserServiceMock.GetAllSSOUsers(true),
				Times.Exactly(2));

			passwordEncryptionBrokerMock.Verify(passwordEncryptionBrokerMock =>
				passwordEncryptionBrokerMock.EncryptedAndPlainTextAreEqual(expectedSSOUser.PasswordHash, inputPassword),
				Times.Once);
		}

    [Fact]
    public async Task FindByUserAndPasswordNotWorksForLockoutAsExpected()
    {
        //given
        string inputPassword = RandomString();

        IQueryable<SSOUser> ssoUsersInService = RandomSSOUsers()
            .AsQueryable();

        ssoUserServiceMock.Setup(ssoUserServiceMock =>
            ssoUserServiceMock.GetAllSSOUsers(true))
            .Returns(ssoUsersInService);

        SSOUser expectedSSOUser = ssoUsersInService.First();

        expectedSSOUser.LockoutEnabled = true;

        passwordEncryptionBrokerMock.Setup(passwordEncryptionBrokerMock =>
            passwordEncryptionBrokerMock.EncryptedAndPlainTextAreEqual(expectedSSOUser.PasswordHash, inputPassword))
            .Returns(true);

        //when & then
        await Assert.ThrowsAsync<SecurityException>(async () => await ssoUserProcessingService
			.FindByUserAndPasswordAsync(expectedSSOUser.Id, inputPassword));
    }
}