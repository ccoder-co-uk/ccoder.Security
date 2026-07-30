// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Exposures;

public interface ISSOPrivilegeManager
{
    public IQueryable<SSOPrivilege> GetAllSSOPrivileges();
}