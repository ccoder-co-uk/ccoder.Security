// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;

internal sealed partial class SSOPrivilegeProcessingService(ISSOPrivilegeService privService)
        : ISSOPrivilegeProcessingService
{
    public IQueryable<SSOPrivilege> GetAllSSOPrivileges() =>
        TryCatch(operation: () =>
        {
            ValidateSSOPrivilegesOnGet();

            return privService.GetAllSSOPrivileges();
        });
}