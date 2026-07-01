using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations.Interfaces;
public interface ITenantCoordinationService
{
    ValueTask DeleteTenantAsync(Tenant item);
}