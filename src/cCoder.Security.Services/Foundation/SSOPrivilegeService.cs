using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;
using System.Linq;

namespace cCoder.Security.Services.Foundation
{
    public class SSOPrivilegeService : ISSOPrivilegeService
    {
        readonly ISSOPrivilegeBroker privBroker;

        public SSOPrivilegeService(ISSOPrivilegeBroker privBroker)
            => this.privBroker = privBroker;

        public IQueryable<SSOPrivilege> GetAllSSOPrivileges()
            => privBroker.GetPrivileges();
    }
}