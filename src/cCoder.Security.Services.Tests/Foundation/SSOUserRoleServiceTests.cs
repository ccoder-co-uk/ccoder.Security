using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation;
using cCoder.Security.Services.Foundation.Interfaces;
using FizzWare.NBuilder;
using Moq;

namespace cCoder.Security.Services.Tests.Foundation
{
    public partial class SSOUserRoleServiceTests
    {
        readonly Mock<ISSOUserRoleBroker> userRoleBrokerMock;
        readonly ISSOUserRoleService userRoleService;

        public SSOUserRoleServiceTests()
        {
            userRoleBrokerMock = new Mock<ISSOUserRoleBroker>();
            userRoleService = new SSOUserRoleService(userRoleBrokerMock.Object);
        }

        static IQueryable<SSOUserRole> RandomUserRoles() => 
            Enumerable.Range(0, new Random().Next(100))
                .Select(i => RandomUserRole())
                .AsQueryable();

        static SSOUserRole RandomUserRole() => 
            Builder<SSOUserRole>
                .CreateNew()
                .Build();
    }
}