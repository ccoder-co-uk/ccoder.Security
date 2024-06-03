using System.Security;
using cCoder.Security.Api.Interfaces;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Api.Controllers;

[Route("Api/Security/UserInvite")]
public class UserInviteController(
    IAccountManager ssoUserProcessingService,
    ISSOUserOrchestrationService ssoUserOrchestrationService) : Controller
{
    [HttpPost("Accept")]
    public virtual async ValueTask<IActionResult> AcceptInvite([FromQuery] string userId, [FromQuery] string inviteToken, [FromBody] RegisterUser inviteForm)
    {
        SSOUser user = ssoUserOrchestrationService
            .GetAllSSOUsers()
            .FirstOrDefault(i => i.Id == userId);

        if (user == null)
            throw new SecurityException("Access Denied!");

        await ssoUserProcessingService.AcceptInviteAsync(inviteForm, userId, inviteToken);

        return Ok();
    }
}