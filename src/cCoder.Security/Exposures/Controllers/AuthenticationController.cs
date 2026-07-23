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
    public async ValueTask<IActionResult> Login([FromBody] Auth auth) =>
        ModelState.IsValid
            ? Ok(value: await authenticationOrchestrationService.LoginAsync(auth.User, auth.Pass))
            : BadRequest(modelState: ModelState);

    [HttpPost("Logout")]
    public async ValueTask<IActionResult> Logout()
    {
        await authenticationOrchestrationService.LogoutAsync();
        return Ok();
    }

    [HttpGet("Me")]
    public IActionResult Me() =>
        Ok(value: authenticationOrchestrationService.Me());

    [HttpPost("ForgotPassword")]
    public async ValueTask<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
        { return BadRequest(modelState: ModelState); }

        try
        {
            await authenticationOrchestrationService.ForgotPasswordAsync(email: request.Email);
        }
        catch
        {
            // Deliberate: avoid revealing whether an account exists for the email.
        }

        return Ok();
    }

    [HttpPost("ConfirmForgotPassword")]
    public async ValueTask<IActionResult> ConfirmForgotPassword(
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