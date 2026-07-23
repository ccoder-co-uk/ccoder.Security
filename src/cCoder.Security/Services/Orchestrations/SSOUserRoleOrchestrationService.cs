// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Orchestrations;

internal class SSOUserRoleOrchestrationService(
    ISSOUserRoleProcessingService userRoleProcessingService,
    ISSOAuthorizationBroker authBroker)
        : ISSOUserRoleOrchestrationService
{
    public IQueryable<SSOUserRole> GetAllSSOUserRoles()
    {
        authBroker.UserHasPrivilege(privilege: "userrole_read");

        return userRoleProcessingService.GetAllSSOUserRoles();
    }

    public async ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole newSSOUserRole)
    {
        if (userRoleProcessingService
            .GetAllSSOUserRoles()
            .Any())
        { authBroker.UserIsPortalAdminWithPrivilege(privilege: "userrole_create"); }

        return await userRoleProcessingService.AddSSOUserRoleAsync(newSSOUserRole: newSSOUserRole);
    }

    public async ValueTask DeleteSSOUserRoleAsync(SSOUserRole deletedSSOUserRole)
    {
        authBroker.UserIsPortalAdminWithPrivilege(privilege: "userrole_delete");

        await userRoleProcessingService.DeleteSSOUserRoleAsync(deletedSSOUserRole: deletedSSOUserRole);
    }
}