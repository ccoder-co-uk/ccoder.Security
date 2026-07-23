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

    public async ValueTask<SSORole> AddSSORoleAsync(SSORole ssoRole)
    {
        if (roleProcessingService
            .GetAllSSORoles()
            .Any())
        { authBroker.UserIsPortalAdminWithPrivilege(privilege: "tenant_admin"); }

        return await roleProcessingService.AddSSORoleAsync(item: ssoRole);
    }

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole ssoRole)
    {
        authBroker.UserIsPortalAdminWithPrivilege(privilege: "tenant_admin");

        return await roleProcessingService.UpdateSSORoleAsync(item: ssoRole);
    }

    public async ValueTask DeleteSSORoleAsync(SSORole ssoRole)
    {
        authBroker.UserIsPortalAdminWithPrivilege(privilege: "tenant_admin");

        await roleProcessingService.DeleteSSORoleAsync(item: ssoRole);
    }
}