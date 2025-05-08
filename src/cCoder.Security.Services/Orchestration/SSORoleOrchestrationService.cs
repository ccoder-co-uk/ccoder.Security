using cCoder.Security.Data.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Services.Orchestration;

public class SSORoleOrchestrationService(
    ISSORoleProcessingService roleProcessingService,
    ISSOAuthorizationBroker authBroker)
        : ISSORoleOrchestrationService
{
    public IQueryable<SSORole> GetAllSSORoles() =>
        roleProcessingService.GetAllSSORoles();

    public async ValueTask<SSORole> AddSSORoleAsync(SSORole ssoRole)
    {
        authBroker.UserIsPortalAdminWithPrivilege("tenant_admin");

        return await roleProcessingService.AddSSORoleAsync(ssoRole);
    }

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole ssoRole)
    {
        authBroker.UserIsPortalAdminWithPrivilege("tenant_admin");

        return await roleProcessingService.UpdateSSORoleAsync(ssoRole);
    }

    public async ValueTask DeleteSSORoleAsync(SSORole ssoRole)
    {
        authBroker.UserIsPortalAdminWithPrivilege("tenant_admin");

        await roleProcessingService.DeleteSSORoleAsync(ssoRole);
    }
}