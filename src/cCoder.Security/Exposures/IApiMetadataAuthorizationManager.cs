// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Security.Exposures;

public interface IApiMetadataAuthorizationManager
{
    void EnsureUserCanReadApiMetadata();
}