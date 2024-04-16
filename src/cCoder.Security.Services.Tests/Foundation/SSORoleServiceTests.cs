using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation;
using cCoder.Security.Services.Foundation.Interfaces;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Security.Services.Tests.Foundation;

public partial class SSORoleServiceTests
{
    readonly Mock<ISSORoleBroker> roleBrokerMock;
    readonly ISSORoleService roleService;

    public SSORoleServiceTests()
    {
        roleBrokerMock = new Mock<ISSORoleBroker>();
        roleService = new SSORoleService(roleBrokerMock.Object);
    }

    static IQueryable<SSORole> RandomRoles() => 
        Enumerable.Range(0, new Random().Next(100))
            .Select(i => RandomRole(Guid.NewGuid()))
            .AsQueryable();

    static SSORole RandomRole(Guid id) => 
        Builder<SSORole>
            .CreateNew()
            .With(i => i.Id = id)
            .Build();
}