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
        Enumerable.Range(1, new Random().Next(10, 20))
            .Select(_ => RandomUserEvent())
            .ToArray();

    private UserEvent RandomUserEvent() => 
        GetUserEventFiller().Create();

    private Filler<UserEvent> GetUserEventFiller()
    {
        Filler<UserEvent> filler = new();

        filler.Setup()
            .OnType<DateTimeOffset>().Use(DateTimeOffset.Now)
            .OnProperty(ue => ue.Session).IgnoreIt()
            .OnProperty(ue => ue.CreatedByUser).IgnoreIt()
            .OnProperty(ue => ue.Tenant).IgnoreIt();

        return filler;
    }
}


