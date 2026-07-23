// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations.Interfaces;

public interface ISSORoleOrchestrationService
{
    ValueTask<SSORole> AddSSORoleAsync(SSORole newSSORole);
    ValueTask DeleteSSORoleAsync(SSORole deletedSSORole);
    IQueryable<SSORole> GetAllSSORoles();
    ValueTask<SSORole> UpdateSSORoleAsync(SSORole updatedSSORole);
}