using cCoder.Security.Data.Brokers.Encryption;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;
using cCoder.Security.Services.Processing;
using cCoder.Security.Services.Processing.Interfaces;
using Moq;
using Tynamix.ObjectFiller;

namespace cCoder.Security.Services.Tests.Processing;

public partial class SSOUserProcessingServiceTests
{
	private readonly Mock<IPasswordEncryptionBroker> passwordEncryptionBrokerMock;
	private readonly Mock<ISSOUserService> ssoUserServiceMock;
	private readonly ISSOUserProcessingService ssoUserProcessingService;

	public SSOUserProcessingServiceTests()
	{
		passwordEncryptionBrokerMock = new Mock<IPasswordEncryptionBroker>();
		ssoUserServiceMock = new Mock<ISSOUserService>();
		ssoUserProcessingService = new SSOUserProcessingService(ssoUserServiceMock.Object,
			passwordEncryptionBrokerMock.Object);
	}

    private static string RandomString() => 
		new MnemonicString().GetValue();

    private static SSOUser[] RandomSSOUsers() => 
		Enumerable.Range(1, new Random().Next(10, 20))
			.Select(_ => RandomSSOUser())
			.ToArray();

    private static SSOUser RandomSSOUser() => 
		GetSSOUserFiller().Create();

    private static Filler<SSOUser> GetSSOUserFiller()
	{
        Filler<SSOUser> filler = new();

		filler.Setup()
			.OnProperty(p => p.Roles).IgnoreIt()
			.OnProperty(p => p.Tokens).IgnoreIt()
			.OnProperty(p => p.UserEvents).IgnoreIt();

		return filler;
	}
}

