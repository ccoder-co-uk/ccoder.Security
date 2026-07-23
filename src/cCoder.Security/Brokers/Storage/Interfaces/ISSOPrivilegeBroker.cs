// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ISSOPrivilegeBroker
{
    IQueryable<SSOPrivilege> SelectPrivileges();
}