using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestration.Interfaces;

public interface ITenantRelationsOrchestrationService
{
    ValueTask DeleteTenantRelationsAsync(Tenant item);
}