using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundation.Interfaces;

public interface ISSOPrivilegeService
{
    IQueryable<SSOPrivilege> GetAllSSOPrivileges();
}