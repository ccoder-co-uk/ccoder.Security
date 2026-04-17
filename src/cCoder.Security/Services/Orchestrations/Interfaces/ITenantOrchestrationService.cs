using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations.Interfaces;
internal interface ITenantOrchestrationService
{
    ValueTask<Tenant> AddTenantAsync(Tenant item);
    ValueTask DeleteTenantAsync(Tenant item);
    IQueryable<Tenant> GetAllTenants();
    ValueTask<Tenant> UpdateTenantAsync(Tenant item);
}

