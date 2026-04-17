using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations;
using cCoder.Security.Services.Foundations.Interfaces;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Security.Tests.Foundations;

public partial class SSOUserServiceTests
{
    private readonly Mock<ISSOUserBroker> userBrokerMock;
    private readonly ISSOUserService userService;

    public SSOUserServiceTests()
    {
        userBrokerMock = new Mock<ISSOUserBroker>();
        userService = new SSOUserService(userBrokerMock.Object);
    }

    private static string RandomString() => 
        new RandomGenerator().NextString(5, 12);

    private static IQueryable<SSOUser> RandomUsers() => 
        Enumerable.Range(0, new Random().Next(100))
            .Select(i => RandomUser(RandomString()))
            .AsQueryable();

    private static SSOUser RandomUser(string id) => 
        Builder<SSOUser>
            .CreateNew()
            .With(i => i.Id = id)
            .Build();
}

