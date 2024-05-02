using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Services.Processing;

public class SSOPrivilegeProcessingService 
    : ISSOPrivilegeProcessingService
{
    private readonly ISSOPrivilegeService privService;

    public SSOPrivilegeProcessingService(ISSOPrivilegeService privService)
    {
        this.privService = privService;
    }

    public IQueryable<SSOPrivilege> GetAllSSOPrivileges() => 
        privService.GetAllSSOPrivileges();
}
