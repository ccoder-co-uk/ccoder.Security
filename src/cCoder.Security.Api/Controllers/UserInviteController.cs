using System.Security;
using cCoder.Core.Objects.Extensions;
using cCoder.Security.Api.Interfaces;
using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Processing.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Api.Controllers;

[Route("Api/Security/UserInvite")]
public class UserInviteController(
    IAccountManager accountManager,
    ISSOUserProcessingService ssoUserProcessingService,
    ILogger<UserInviteController> log) : Controller
{
    [HttpPost("Accept")]
    public virtual async ValueTask<IActionResult> AcceptInvite([FromQuery] string userId, [FromQuery] string inviteToken, [FromBody] RegisterUser inviteForm)
    {
        SSOUser user = ssoUserProcessingService
            .GetAllSSOUsers(ignoreFilters: true)
            .FirstOrDefault(i => i.Id == userId);

        log.LogInformation($"userId: {userId} | inviteToken: {inviteToken} | inviteForm: {inviteForm.ToJson()}");

        if (user == null)
            throw new SecurityException("Access Denied! user not found controller");

        await accountManager.AcceptInviteAsync(inviteForm, userId, inviteToken);

        return Ok();
    }
}