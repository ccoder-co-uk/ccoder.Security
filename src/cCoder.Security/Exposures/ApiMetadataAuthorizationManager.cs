// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Exposures;

internal sealed class ApiMetadataAuthorizationManager(
    IAuthorizationService authorizationService)
        : IApiMetadataAuthorizationManager
{
    public void EnsureUserCanReadApiMetadata() =>
        authorizationService.EnsureUserCanReadApiMetadata();
}