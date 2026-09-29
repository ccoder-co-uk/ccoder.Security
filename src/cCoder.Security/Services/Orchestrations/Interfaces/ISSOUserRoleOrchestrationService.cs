// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Services.Orchestrations.Interfaces;

public interface ISSOUserRoleOrchestrationService
{
    IQueryable<SSOUserRole> GetAllSSOUserRoles();

    ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole userRole);

    ValueTask DeleteSSOUserRoleAsync(SSOUserRole userRole);
}