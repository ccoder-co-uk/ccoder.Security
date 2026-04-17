using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;
internal interface ITenantProcessingService
{
    ValueTask<Tenant> AddTenantAsync(Tenant item);
    ValueTask<Tenant> UpdateTenantAsync(Tenant item);
    ValueTask DeleteTenantAsync(Tenant item);
    IQueryable<Tenant> GetAllTenants();
}


