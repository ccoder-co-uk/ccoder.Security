// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class SSOPrivilegeService(ISSOPrivilegeBroker privBroker)
    : ISSOPrivilegeService
{
    public IQueryable<SSOPrivilege> GetAllSSOPrivileges() =>
        TryCatch(operation: () =>
        {
            ValidatePrivilegesOnGet();

            return privBroker.SelectPrivileges();
        });
}