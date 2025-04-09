using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation;
using cCoder.Security.Services.Foundation.Interfaces;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class SSOUserRoleServiceTests
{
    private readonly Mock<ISSOUserRoleBroker> userRoleBrokerMock;
    private readonly Mock<ITenantBroker> tenantBrokerMock;
    private readonly Mock<ISSOAuthorizationBroker> authBrokerMock;
    private readonly ISSOUserRoleService userRoleService;

    public SSOUserRoleServiceTests()
    {
        userRoleBrokerMock = new Mock<ISSOUserRoleBroker>();
        tenantBrokerMock = new Mock<ITenantBroker>();
        authBrokerMock = new Mock<ISSOAuthorizationBroker>();
        userRoleService = new SSOUserRoleService(userRoleBrokerMock.Object, tenantBrokerMock.Object, authBrokerMock.Object);
    }

    private static IQueryable<SSOUserRole> RandomUserRoles() => 
        Enumerable.Range(0, new Random().Next(100))
            .Select(i => RandomUserRole())
            .AsQueryable();

    private static SSOUserRole RandomUserRole() => 
        Builder<SSOUserRole>
            .CreateNew()
            .Build();
}