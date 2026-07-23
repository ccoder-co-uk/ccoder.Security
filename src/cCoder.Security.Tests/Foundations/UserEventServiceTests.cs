// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.DateTime;
using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations;
using cCoder.Security.Services.Foundations.Interfaces;
using Moq;
using Tynamix.ObjectFiller;

namespace cCoder.Security.Tests.Foundations;

public partial class UserEventServiceTests
{
    private readonly Mock<ISecurityDateTimeOffsetBroker> dateTimeOffsetBrokerMock;
    private readonly Mock<IUserEventBroker> userEventBrokerMock;
    private readonly IUserEventService userEventService;

    public UserEventServiceTests()
    {
        dateTimeOffsetBrokerMock = new Mock<ISecurityDateTimeOffsetBroker>();
        userEventBrokerMock = new Mock<IUserEventBroker>();
        userEventService = new UserEventService(userEventBrokerMock.Object, dateTimeOffsetBrokerMock.Object);
    }

    private UserEvent[] RandomUserEvents() =>
        Enumerable.Range(start: 1, count: new Random().Next(minValue:10, maxValue:20))
            .Select(selector: _ => RandomUserEvent())
            .ToArray();

    private UserEvent RandomUserEvent() =>
        GetUserEventFiller()
            .Create();

    private Filler<UserEvent> GetUserEventFiller()
    {
        Filler<UserEvent> filler = new();

        filler.Setup()
            .OnType<DateTimeOffset>()
            .Use(valueToUse:DateTimeOffset.Now)
            .OnProperty(property:ue => ue.Session)
            .IgnoreIt()
            .OnProperty(property: ue => ue.CreatedByUser)
            .IgnoreIt()
            .OnProperty(property: ue => ue.Tenant)
            .IgnoreIt();

        return filler;
    }
}