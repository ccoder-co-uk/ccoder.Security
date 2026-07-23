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

    public ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole item) =>
        ssoUserRoleService.AddSSOUserRoleAsync(item: item);

    public ValueTask DeleteSSOUserRoleAsync(SSOUserRole item) =>
        ssoUserRoleService.DeleteSSOUserRoleAsync(item: item);
}