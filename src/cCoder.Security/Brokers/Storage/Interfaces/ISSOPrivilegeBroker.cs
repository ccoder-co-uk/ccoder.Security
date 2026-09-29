// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;

using cCoder.Security.Models.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;

internal interface ISSOPrivilegeBroker
{
    IQueryable<SSOPrivilege> SelectPrivileges();
}