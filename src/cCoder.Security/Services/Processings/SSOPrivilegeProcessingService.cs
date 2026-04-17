using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;
internal class SSOPrivilegeProcessingService(ISSOPrivilegeService privService)
        : ISSOPrivilegeProcessingService
{
    public IQueryable<SSOPrivilege> GetAllSSOPrivileges() => 
        privService.GetAllSSOPrivileges();
}


