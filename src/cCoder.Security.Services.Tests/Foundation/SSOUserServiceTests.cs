using FizzWare.NBuilder;
using Moq;
using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation;
using cCoder.Security.Services.Foundation.Interfaces;
using System;
using System.Linq;

namespace cCoder.Security.Services.Tests.Foundation
{
    public partial class SSOUserServiceTests
    {
        readonly Mock<ISSOUserBroker> userBrokerMock;
        readonly ISSOUserService userService;

        public SSOUserServiceTests()
        {
            userBrokerMock = new Mock<ISSOUserBroker>();
            userService = new SSOUserService(userBrokerMock.Object);
        }

        static string RandomString()
            => new RandomGenerator().NextString(5, 12);

        static IQueryable<SSOUser> RandomUsers()
            => Enumerable.Range(0, new Random().Next(100))
                .Select(i => RandomUser(RandomString()))
                .AsQueryable();

        static SSOUser RandomUser(string id)
            => Builder<SSOUser>
                .CreateNew()
                .With(i => i.Id = id)
                .Build();
    }
}