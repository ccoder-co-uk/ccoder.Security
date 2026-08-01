// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;

namespace cCoder.Security.Dependencies.Authentication;

internal sealed class SecurityBearerAuthenticationDependency(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(
            options: options,
            logger: logger,
            encoder: encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync() =>
        Task.FromResult(result: AuthenticateResult.NoResult());

    protected override Task HandleChallengeAsync(
        AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.Headers.WWWAuthenticate =
            new AuthenticationHeaderValue(scheme: "Bearer").ToString();

        return Task.CompletedTask;
    }
}
