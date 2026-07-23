// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Orchestrations;

internal class SSORoleOrchestrationService(
    ISSORoleProcessingService roleProcessingService,
    ISSOAuthorizationBroker authBroker)
        : ISSORoleOrchestrationService
{
    public IQueryable<SSORole> GetAllSSORoles() =>
        roleProcessingService.GetAllSSORoles();

    public async ValueTask<SSORole> AddSSORoleAsync(SSORole newSSORole)
    {
        if (roleProcessingService
            .GetAllSSORoles()
            .Any())
        { authBroker.UserIsPortalAdminWithPrivilege(privilege: "tenant_admin"); }

        return await roleProcessingService.AddSSORoleAsync(newSSORole: newSSORole);
    }

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole updatedSSORole)
    {
        authBroker.UserIsPortalAdminWithPrivilege(privilege: "tenant_admin");

        return await roleProcessingService.UpdateSSORoleAsync(updatedSSORole: updatedSSORole);
    }

    public async ValueTask DeleteSSORoleAsync(SSORole deletedSSORole)
    {
        authBroker.UserIsPortalAdminWithPrivilege(privilege: "tenant_admin");

        await roleProcessingService.DeleteSSORoleAsync(deletedSSORole: deletedSSORole);
    }
}