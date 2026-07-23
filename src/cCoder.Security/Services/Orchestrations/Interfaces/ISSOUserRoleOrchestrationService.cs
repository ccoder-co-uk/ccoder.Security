// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Orchestrations.Interfaces;

public interface ISSOUserRoleOrchestrationService
{
    IQueryable<SSOUserRole> GetAllSSOUserRoles();

    ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole userRole);

    ValueTask DeleteSSOUserRoleAsync(SSOUserRole userRole);
}