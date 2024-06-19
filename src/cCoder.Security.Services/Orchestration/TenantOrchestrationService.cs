using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Services.Orchestration;

public class TenantOrchestrationService(
    ITenantProcessingService tenantProcessingService) : ITenantOrchestrationService
{
    public IQueryable<Tenant> GetAllTenants() =>
        tenantProcessingService.GetAllTenants();

    public async ValueTask<Tenant> AddTenantAsync(Tenant item)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<Tenant> UpdateTenantAsync(Tenant item)
    {
        throw new NotImplementedException();
    }

    public ValueTask DeleteTenantAsync(Tenant item)
    {
        throw new NotImplementedException();
    }
}