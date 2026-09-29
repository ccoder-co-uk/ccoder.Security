// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Security.Data.Models;

namespace cCoder.Security.Exposures;

public interface ITenantManager
{
    ValueTask SetupAsync(SetupDetails setupDetails);
}