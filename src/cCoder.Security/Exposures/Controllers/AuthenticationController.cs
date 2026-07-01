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
            ? Ok(await authenticationOrchestrationService.LoginAsync(auth.User, auth.Pass))
            : BadRequest(ModelState);

    [HttpPost("Logout")]
    public async ValueTask<IActionResult> Logout()
    {
        await authenticationOrchestrationService.LogoutAsync();
        return Ok();
    }

    [HttpGet("Me")]
    public IActionResult Me() =>
        Ok(authenticationOrchestrationService.Me());

    [HttpPost("ForgotPassword")]
    public async ValueTask<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await authenticationOrchestrationService.ForgotPasswordAsync(request.Email);
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
            return BadRequest(ModelState);

        await authenticationOrchestrationService.ConfirmForgotPasswordAsync(
            request.Token,
            request.UserId,
            request.NewPassword,
            request.ConfirmPassword);

        return Ok();
    }
}
