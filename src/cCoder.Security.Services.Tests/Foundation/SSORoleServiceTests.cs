using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation;
using cCoder.Security.Services.Foundation.Interfaces;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class SSORoleServiceTests
{
    private readonly Mock<ISSORoleBroker> roleBrokerMock;
    private readonly Mock<ISSOAuthorizationBroker> authBrokerMock;
    private readonly ISSORoleService roleService;

    public SSORoleServiceTests()
    {
        roleBrokerMock = new Mock<ISSORoleBroker>();
        authBrokerMock = new Mock<ISSOAuthorizationBroker>();
        roleService = new SSORoleService(roleBrokerMock.Object, authBrokerMock.Object);
    }

    private static IQueryable<SSORole> RandomRoles() => 
        Enumerable.Range(0, new Random().Next(100))
            .Select(i => RandomRole(Guid.NewGuid()))
            .AsQueryable();

    private static SSORole RandomRole(Guid id) => 
        Builder<SSORole>
            .CreateNew()
            .With(i => i.Id = id)
            .Build();
}