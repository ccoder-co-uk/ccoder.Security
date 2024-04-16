using cCoder.Security.Objects.Entities;
using System.Linq;

namespace cCoder.Security.Services.Processing.Interfaces
{
    public interface ISSOPrivilegeProcessingService
    {
        public IQueryable<SSOPrivilege> GetAllSSOPrivileges();
    }
}