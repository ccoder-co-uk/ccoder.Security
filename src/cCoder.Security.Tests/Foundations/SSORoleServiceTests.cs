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

public partial class SSORoleServiceTests
{
    private readonly Mock<ISSORoleBroker> roleBrokerMock;
    private readonly ISSORoleService roleService;

    public SSORoleServiceTests()
    {
        roleBrokerMock = new Mock<ISSORoleBroker>();
        roleService = new SSORoleService(roleBrokerMock.Object);
    }

    private static IQueryable<SSORole> RandomRoles() =>
        Enumerable.Range(start: 0, count: new Random().Next(maxValue:100))
            .Select(selector: i => RandomRole(id: Guid.NewGuid()))
            .AsQueryable();

    private static SSORole RandomRole(Guid id) =>
        Builder<SSORole>
            .CreateNew()
            .With(func: i => i.Id = id)
            .Build();
}