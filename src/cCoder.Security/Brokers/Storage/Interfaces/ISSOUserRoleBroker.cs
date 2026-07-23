// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ISSOUserRoleBroker
{
    ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole userRole);
    ValueTask DeleteSSOUserRoleAsync(SSOUserRole userRole);

    IQueryable<SSOUserRole> GetAllSSOUserRoles();
}