using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;

namespace cCoder.Security.Services.Processing;

public class TenantProcessingService(ITenantService tenantService)
        : ITenantProcessingService
{
    public ValueTask<Tenant> AddTenantAsync(Tenant item) => 
        tenantService.AddTenantAsync(item);

    public ValueTask DeleteTenantAsync(Tenant item) => 
        tenantService.DeleteTenantAsync(item);

    public IQueryable<Tenant> GetAllTenants() => 
        tenantService.GetAllTenants();

    public ValueTask<Tenant> UpdateTenantAsync(Tenant item) => 
        tenantService.UpdateTenantAsync(item);
}
