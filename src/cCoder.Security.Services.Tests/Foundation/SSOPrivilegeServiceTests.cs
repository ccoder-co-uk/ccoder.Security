using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation;
using cCoder.Security.Services.Foundation.Interfaces;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Security.Services.Tests.Foundation;

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
            .Select(i => RandomSSOPrivilege())
            .AsQueryable();

    private static SSOPrivilege RandomSSOPrivilege() => 
        Builder<SSOPrivilege>
            .CreateNew()
            .Build();
}