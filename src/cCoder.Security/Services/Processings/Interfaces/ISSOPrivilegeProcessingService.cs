using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;
internal interface ISSOPrivilegeProcessingService
{
    public IQueryable<SSOPrivilege> GetAllSSOPrivileges();
}

