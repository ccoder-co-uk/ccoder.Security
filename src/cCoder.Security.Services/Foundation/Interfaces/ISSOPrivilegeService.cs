using cCoder.Security.Objects.Entities;
using System.Linq;

namespace cCoder.Security.Services.Foundation.Interfaces
{
    public interface ISSOPrivilegeService
    {
        IQueryable<SSOPrivilege> GetAllSSOPrivileges();
    }
}