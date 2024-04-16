using System;
using System.Linq;
using FizzWare.NBuilder;
using Moq;
using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Tests.Foundation
{
    public partial class SSOPrivilegeServiceTests
    {
        readonly Mock<ISSOPrivilegeBroker> privBrokerMock;
        readonly ISSOPrivilegeService privService;

        public SSOPrivilegeServiceTests()
        {
            privBrokerMock = new Mock<ISSOPrivilegeBroker>();
            privService = new SSOPrivilegeService(privBrokerMock.Object);
        }

        static IQueryable<SSOPrivilege> RandomSSOPrivileges()
            => Enumerable.Range(0, new Random().Next(100))
                .Select(i => RandomSSOPrivilege())
                .AsQueryable();

        static SSOPrivilege RandomSSOPrivilege()
            => Builder<SSOPrivilege>
                .CreateNew()
                .Build();
    }
}