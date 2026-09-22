// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;

namespace cCoder.Security.Exposures;

internal sealed class SSORoleManager(
    ISSORoleOrchestrationService roleOrchestrationService)
        : ISSORoleManager
{
    public ValueTask<SSORole> AddSSORoleAsync(SSORole newSSORole) =>
        roleOrchestrationService.AddSSORoleAsync(item: newSSORole);

    public ValueTask DeleteSSORoleAsync(SSORole deletedSSORole) =>
        roleOrchestrationService.DeleteSSORoleAsync(item: deletedSSORole);

    public IQueryable<SSORole> GetAllSSORoles() =>
        roleOrchestrationService.GetAllSSORoles();

    public ValueTask<SSORole> UpdateSSORoleAsync(SSORole updatedSSORole) =>
        roleOrchestrationService.UpdateSSORoleAsync(item: updatedSSORole);
}