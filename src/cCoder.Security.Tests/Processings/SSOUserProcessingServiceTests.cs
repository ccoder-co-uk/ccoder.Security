// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Encryption;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Interfaces;
using Moq;
using Tynamix.ObjectFiller;

namespace cCoder.Security.Tests.Processings;

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
        Enumerable.Range(start: 1, count: new Random().Next(10, 20))
            .Select(selector: _ => RandomSSOUser())
            .ToArray();

    private static SSOUser RandomSSOUser() =>
        GetSSOUserFiller().Create();

    private static Filler<SSOUser> GetSSOUserFiller()
    {
        Filler<SSOUser> filler = new();

        filler.Setup()
            .OnProperty(p => p.Roles).IgnoreIt()
            .OnProperty(property: p => p.Tokens).IgnoreIt()
            .OnProperty(property: p => p.UserEvents).IgnoreIt();

        return filler;
    }
}