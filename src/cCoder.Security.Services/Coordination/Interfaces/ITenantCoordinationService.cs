using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestration.Interfaces;
public interface ITenantCoordinationService
{
    ValueTask DeleteTenantAsync(Tenant item);
}