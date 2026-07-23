// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        new RandomGenerator().NextString(minLength: 5, maxLength: 12);

    private static IQueryable<SSOUser> RandomUsers() =>
        Enumerable.Range(start: 0, count: new Random().Next(maxValue:100))
            .Select(selector: i => RandomUser(id: RandomString()))
            .AsQueryable();

    private static SSOUser RandomUser(string id) =>
        Builder<SSOUser>
            .CreateNew()
            .With(func: i => i.Id = id)
            .Build();
}