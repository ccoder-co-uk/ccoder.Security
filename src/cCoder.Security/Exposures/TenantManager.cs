using cCoder.Security.Data.Models;
using cCoder.Security.Services.Foundations.Events;

namespace cCoder.Security.Exposures;

internal class TenantManager(ITenantSetupEventService tenantSetupEventService) : ITenantManager
{
    public ValueTask SetupAsync(SetupDetails setupDetails) =>
        tenantSetupEventService.RaiseSetupAsync(setupDetails);
}
