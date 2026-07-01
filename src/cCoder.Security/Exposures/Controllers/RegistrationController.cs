using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class RegistrationController(ISSOUserOrchestrationService ssoUserOrchestrationService) 
    : Controller
{
    [HttpPost("Register")]
    public async ValueTask<IActionResult> Register([FromBody] RegisterUser registerForm)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        (SSOUser user, string confirmationToken) = await ssoUserOrchestrationService.Register(registerForm);

        return Ok(new
        {
            User = user,
            Token = confirmationToken
        });
    }

    [HttpPost("ConfirmRegistration")]
    public async ValueTask<IActionResult> ConfirmRegistration(string confirmationToken)
    {
        await ssoUserOrchestrationService.ConfirmRegistration(confirmationToken);
        return Ok();
    }

    [HttpPost("Invite")]
    public async ValueTask<IActionResult> Invite([FromBody] RegisterUser inviteForm)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        (SSOUser user, string invitationToken) = await ssoUserOrchestrationService.InviteUserAsync(inviteForm);

        return Ok(new
        {
            User = user,
            Token = invitationToken
        });
    }

    [HttpPost("ResendInvite")]
    public async ValueTask<IActionResult> ResendInvite([FromQuery] string userId)
    {
        string invitationToken = await ssoUserOrchestrationService.RegenerateUserInviteToken(userId);
        return Ok(new { Token = invitationToken });
    }

    [HttpPost("AcceptInvite")]
    public async ValueTask<IActionResult> AcceptInvite(
        [FromQuery] string userId,
        [FromQuery] string inviteToken,
        [FromBody] RegisterUser inviteForm)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await ssoUserOrchestrationService.AcceptInviteAsync(inviteForm, userId, inviteToken);

        return Ok();
    }
}
