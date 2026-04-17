using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;
internal interface ISSOPrivilegeService
{
    IQueryable<SSOPrivilege> GetAllSSOPrivileges();
}

