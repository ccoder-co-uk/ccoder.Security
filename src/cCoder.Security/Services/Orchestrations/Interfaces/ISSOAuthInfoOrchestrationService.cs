using cCoder.Security.Objects;

namespace cCoder.Security.Services.Orchestrations.Interfaces;
internal interface ISSOAuthInfoOrchestrationService
{
    ValueTask<ISSOAuthInfo> GetSSOAuthInfoAsync();
}

