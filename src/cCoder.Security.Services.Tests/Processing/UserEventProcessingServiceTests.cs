using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;
using cCoder.Security.Services.Processing;
using cCoder.Security.Services.Processing.Interfaces;
using Moq;
using Tynamix.ObjectFiller;

namespace cCoder.Security.Services.Tests.Processing;

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
