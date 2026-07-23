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

public partial class SSOPrivilegeServiceTests
{
    private readonly Mock<ISSOPrivilegeBroker> privBrokerMock;
    private readonly ISSOPrivilegeService privService;

    public SSOPrivilegeServiceTests()
    {
        privBrokerMock = new Mock<ISSOPrivilegeBroker>();
        privService = new SSOPrivilegeService(privBrokerMock.Object);
    }

    private static IQueryable<SSOPrivilege> RandomSSOPrivileges() =>
        Enumerable.Range(0, new Random().Next(100))
            .Select(selector: i => RandomSSOPrivilege())
            .AsQueryable();

    private static SSOPrivilege RandomSSOPrivilege() =>
        Builder<SSOPrivilege>
            .CreateNew()
            .Build();
}