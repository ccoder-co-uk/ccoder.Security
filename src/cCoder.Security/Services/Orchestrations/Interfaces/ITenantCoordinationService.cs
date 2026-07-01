using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations.Interfaces;
public interface ITenantCoordinationService
{
    IQueryable<Tenant> GetAllTenants();

    ValueTask<Tenant> AddTenantAsync(Tenant item);

    ValueTask<Tenant> UpdateTenantAsync(Tenant item);

    ValueTask DeleteTenantAsync(Tenant item);
}