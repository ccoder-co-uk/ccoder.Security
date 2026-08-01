// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Encryption.Interfaces;
using cCoder.Security.Brokers.DateTime;
using cCoder.Security.Models;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Interfaces;
using Moq;
using Tynamix.ObjectFiller;
using PasswordHashingBrokerMock =
    Moq.Mock<cCoder.Security.Brokers.Encryption.Interfaces.IPasswordHashingBroker>;

namespace cCoder.Security.Tests.Processings;

#pragma warning disable STXFORMAT008 // Long utility-broker test doubles trigger a formatting false positive.
public partial class SSOUserProcessingServiceTests
{
    private readonly PasswordHashingBrokerMock passwordHashingBrokerMock;
    private readonly Mock<ISSOUserService> ssoUserServiceMock;
    private readonly Mock<ISecurityDateTimeOffsetBroker> dateTimeOffsetBrokerMock;
    private readonly SecurityConfiguration securityConfiguration;
    private readonly ISSOUserProcessingService ssoUserProcessingService;

    public SSOUserProcessingServiceTests()
    {
        passwordHashingBrokerMock = new PasswordHashingBrokerMock();
        ssoUserServiceMock = new Mock<ISSOUserService>();
        dateTimeOffsetBrokerMock = new Mock<ISecurityDateTimeOffsetBroker>();
        securityConfiguration = new SecurityConfiguration();

        ssoUserProcessingService = new SSOUserProcessingService(ssoUserServiceMock.Object,
            passwordHashingBrokerMock.Object,
            dateTimeOffsetBrokerMock.Object,
            securityConfiguration);
    }

    private static string RandomString() =>
        new MnemonicString().GetValue();

    private static SSOUser[] RandomSSOUsers() =>
        Enumerable.Range(start: 1, count: new Random().Next(minValue: 10, maxValue: 20))
            .Select(selector: _ => RandomSSOUser())
            .ToArray();

    private static SSOUser RandomSSOUser() =>
        GetSSOUserFiller()
            .Create();

    private static Filler<SSOUser> GetSSOUserFiller()
    {
        Filler<SSOUser> filler = new();

        filler.Setup()
            .OnProperty(property: p => p.Roles)
            .IgnoreIt()
            .OnProperty(property: p => p.Tokens)
            .IgnoreIt()
            .OnProperty(property: p => p.UserEvents)
            .IgnoreIt();

        return filler;
    }
}
#pragma warning restore STXFORMAT008