// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings;
using cCoder.Security.Services.Processings.Interfaces;
using Moq;
using Tynamix.ObjectFiller;

namespace cCoder.Security.Tests.Processings;

public partial class UserEventProcessingServiceTests
{
    private readonly Mock<IUserEventService> userEventServiceMock;
    private readonly IUserEventProcessingService userEventProcessingService;

    public UserEventProcessingServiceTests()
    {
        userEventServiceMock = new Mock<IUserEventService>();
        userEventProcessingService = new UserEventProcessingService(userEventServiceMock.Object);
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