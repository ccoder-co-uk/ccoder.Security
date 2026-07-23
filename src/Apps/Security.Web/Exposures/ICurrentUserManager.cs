// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace Security.Web.Exposures;

public interface ICurrentUserManager
{
    string GetCurrentUserId();
}