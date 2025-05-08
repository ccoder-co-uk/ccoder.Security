using System.ComponentModel.DataAnnotations;
using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Services.Orchestration;

public class SSOUserRoleOrchestrationService(
    ISSOUserRoleProcessingService userRoleProcessingService,
    ISSOAuthorizationBroker authBroker)
        : ISSOUserRoleOrchestrationService
{
    public IQueryable<SSOUserRole> GetAllSSOUserRoles()
    {
        authBroker.UserHasPrivilege("userrole_read");

        return userRoleProcessingService.GetAllSSOUserRoles();
    }

    public async ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole userRole)
    {
        authBroker.UserIsPortalAdminWithPrivilege("userrole_create");

        return await userRoleProcessingService.AddSSOUserRoleAsync(userRole);
    }

    public async ValueTask DeleteSSOUserRoleAsync(SSOUserRole userRole)
    {
        authBroker.UserIsPortalAdminWithPrivilege("userrole_delete");

        await userRoleProcessingService.DeleteSSOUserRoleAsync(userRole);
    }
}