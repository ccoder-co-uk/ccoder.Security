// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Utility.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal class SSOUserRoleProcessingService(ISSOUserRoleService ssoUserRoleService)
    : ISSOUserRoleProcessingService
{
    public IQueryable<SSOUserRole> GetAllSSOUserRoles() =>
        ssoUserRoleService.GetAllSSOUserRoles();

    public ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole newSSOUserRole) =>
        ssoUserRoleService.AddSSOUserRoleAsync(item: newSSOUserRole);

    public ValueTask DeleteSSOUserRoleAsync(SSOUserRole deletedSSOUserRole) =>
        ssoUserRoleService.DeleteSSOUserRoleAsync(item: deletedSSOUserRole);
}