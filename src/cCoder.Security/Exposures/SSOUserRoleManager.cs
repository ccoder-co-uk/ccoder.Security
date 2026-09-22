// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;

namespace cCoder.Security.Exposures;

internal sealed class SSOUserRoleManager(
    ISSOUserRoleOrchestrationService userRoleOrchestrationService)
        : ISSOUserRoleManager
{
    public IQueryable<SSOUserRole> GetAllSSOUserRoles() =>
        userRoleOrchestrationService.GetAllSSOUserRoles();

    public ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole newSSOUserRole) =>
        userRoleOrchestrationService.AddSSOUserRoleAsync(
            userRole: newSSOUserRole);

    public ValueTask DeleteSSOUserRoleAsync(SSOUserRole deletedSSOUserRole) =>
        userRoleOrchestrationService.DeleteSSOUserRoleAsync(
            userRole: deletedSSOUserRole);
}