// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Serialization;
using cCoder.Security.Brokers.Sessions;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Foundations;
using Moq;
using Xunit;

namespace cCoder.Security.Tests.Foundations;

public sealed partial class SessionServiceTests
{
    [Fact]
    public void SetSSOUser_WhenUserIsProvided_ShouldUseSerializationBroker()
    {
        // Given
        const string serializedUser = "serialized-user";
        SSOUser user = new() { Id = "user-id" };

        Mock<IWebSessionBroker> sessionBrokerMock =
            new(behavior: MockBehavior.Strict);

        Mock<ISerializationBroker> serializationBrokerMock =
            new(behavior: MockBehavior.Strict);

        serializationBrokerMock
            .Setup(expression: broker => broker.Serialize(obj: user))
            .Returns(value: serializedUser);

        sessionBrokerMock
            .Setup(expression: broker => broker.SetString(
                key: "ssoUser",
                value: serializedUser));

        SessionService service = new(
            sessionBroker: sessionBrokerMock.Object,
            serializationBroker: serializationBrokerMock.Object);

        // When
        service.SetSSOUser(sSOUser: user);

        // Then
        serializationBrokerMock.VerifyAll();
        sessionBrokerMock.VerifyAll();
    }

    [Fact]
    public void SetSSOUser_WhenUserHasNullProperties_ShouldPreserveJsonShape()
    {
        // Given
        SSOUser user = new() { Id = "user-id" };

        Mock<IWebSessionBroker> sessionBrokerMock =
            new(behavior: MockBehavior.Strict);

        sessionBrokerMock
            .Setup(expression: broker => broker.SetString(
                key: "ssoUser",
                value: It.Is<string>(match: serialized =>
                    serialized.Contains(
                        value: "\"DisplayName\":null"))));

        SessionService service = new(
            sessionBroker: sessionBrokerMock.Object,
            serializationBroker: new SerializationBroker());

        // When
        service.SetSSOUser(sSOUser: user);

        // Then
        sessionBrokerMock.VerifyAll();
    }
}