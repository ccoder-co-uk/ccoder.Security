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
    public async ValueTask<IActionResult> PostLogin([FromBody] Auth auth) =>
        ModelState.IsValid
            ? Ok(value: await authenticationOrchestrationService.LoginAsync(username: auth.User, password: auth.Pass))
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

    [HttpPost("ForgotPassword")]
    public async ValueTask<IActionResult> PostForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
        { return BadRequest(modelState: ModelState); }

        try
        {
            await authenticationOrchestrationService.ForgotPasswordAsync(email: request.Email);
        }
        catch
        {
        }

        return Ok();
    }

    [HttpPost("ConfirmForgotPassword")]
    public async ValueTask<IActionResult> PostConfirmForgotPassword(
        [FromBody] ConfirmForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
        { return BadRequest(modelState: ModelState); }

        await authenticationOrchestrationService.ConfirmForgotPasswordAsync(
            tokenId: request.Token,
            userId: request.UserId,
            newPassword: request.NewPassword,
            confirmNewPassword: request.ConfirmPassword);

        return Ok();
    }
}