// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage;

internal class SSOPrivilegeBroker(ISecurityDbContextFactory contextFactory)
    : ISSOPrivilegeBroker
{
    public IQueryable<SSOPrivilege> SelectPrivileges() =>
        contextFactory
            .CreateDbContext()
            .GetPrivileges()
            .AsQueryable();
}