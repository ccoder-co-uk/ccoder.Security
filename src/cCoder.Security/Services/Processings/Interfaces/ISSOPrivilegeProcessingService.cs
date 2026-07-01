using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;
public interface ISSOPrivilegeProcessingService
{
    public IQueryable<SSOPrivilege> GetAllSSOPrivileges();
}