// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;

internal interface ISSOUserRoleProcessingService
{
    IQueryable<SSOUserRole> GetAllSSOUserRoles();

    ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole newSSOUserRole);

    ValueTask DeleteSSOUserRoleAsync(SSOUserRole deletedSSOUserRole);
}