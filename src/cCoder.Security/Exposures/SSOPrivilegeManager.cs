// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Exposures;

internal sealed class SSOPrivilegeManager(
    ISSOPrivilegeService ssoPrivilegeService)
        : ISSOPrivilegeManager
{
    public IQueryable<SSOPrivilege> GetAllSSOPrivileges() =>
        ssoPrivilegeService.GetAllSSOPrivileges();
}