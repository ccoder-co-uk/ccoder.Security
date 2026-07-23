// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ISSOUserRoleBroker
{
    ValueTask<SSOUserRole> InsertSSOUserRoleAsync(SSOUserRole userRole);
    ValueTask DeleteSSOUserRoleAsync(SSOUserRole userRole);

    IQueryable<SSOUserRole> SelectAllSSOUserRoles();
}