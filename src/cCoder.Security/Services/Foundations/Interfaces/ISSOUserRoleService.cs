// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;

internal interface ISSOUserRoleService
{
    IQueryable<SSOUserRole> GetAllSSOUserRoles();

    ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole item);
    ValueTask DeleteSSOUserRoleAsync(SSOUserRole item);
}