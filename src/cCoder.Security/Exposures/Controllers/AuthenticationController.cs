// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class AuthenticationController(IAuthenticationOrchestrationService authenticationOrchestrationService) : Controller
{
    [HttpPost("Login")]
    public async ValueTask<IActionResult> PostLogin([FromBody] Auth newAuth) =>
        ModelState.IsValid
            ? Ok(value: await authenticationOrchestrationService.LoginAsync(
                username: newAuth.User,
                password: newAuth.Pass))
            : BadRequest(modelState: ModelState);

    [HttpPost("Logout")]
    public async ValueTask<IActionResult> PostLogout()
    {
        await authenticationOrchestrationService.LogoutAsync();
        return Ok();
    }

    [HttpGet("Me")]
    public IActionResult GetMe() =>
        Ok(value: authenticationOrchestrationService.Me());

}