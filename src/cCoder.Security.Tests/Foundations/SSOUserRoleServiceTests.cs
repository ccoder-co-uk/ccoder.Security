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

public partial class SSOUserRoleServiceTests
{
    private readonly Mock<ISSOUserRoleBroker> userRoleBrokerMock;
    private readonly ISSOUserRoleService userRoleService;

    public SSOUserRoleServiceTests()
    {
        userRoleBrokerMock = new Mock<ISSOUserRoleBroker>();
        userRoleService = new SSOUserRoleService(userRoleBrokerMock.Object);
    }

    private static IQueryable<SSOUserRole> RandomUserRoles() =>
        Enumerable.Range(start: 0, count: new Random().Next(100))
            .Select(selector: i => RandomUserRole())
            .AsQueryable();

    private static SSOUserRole RandomUserRole() =>
        Builder<SSOUserRole>
            .CreateNew()
            .Build();
}