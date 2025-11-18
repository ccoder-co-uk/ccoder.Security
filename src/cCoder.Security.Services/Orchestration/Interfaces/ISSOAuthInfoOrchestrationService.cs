using cCoder.Security.Objects;

namespace cCoder.Security.Services.Orchestration.Interfaces;

public interface ISSOAuthInfoOrchestrationService
{
    ValueTask<ISSOAuthInfo> GetSSOAuthInfoAsync();
}