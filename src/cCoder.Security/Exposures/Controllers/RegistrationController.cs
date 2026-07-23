// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class RegistrationController(ISSOUserAggregationService ssoUserAggregationService)
    : Controller
{
    [HttpPost("Register")]
    public async ValueTask<IActionResult> PostRegister([FromBody] RegisterUser newRegisterUser)
    {
        if (!ModelState.IsValid)
        { return BadRequest(modelState: ModelState); }

        (SSOUser user, string confirmationToken) = await ssoUserAggregationService.RegisterUserAsync(
            registerForm: newRegisterUser);

        return Ok(value: new
        {
            User = user,
            Token = confirmationToken
        });
    }

    [HttpPost("ConfirmRegistration")]
    public async ValueTask<IActionResult> PostConfirmRegistration(string confirmationToken)
    {
        await ssoUserAggregationService.ConfirmRegistration(
            tokenId: confirmationToken);
        return Ok();
    }

    [HttpPost("Invite")]
    public async ValueTask<IActionResult> PostInvite([FromBody] RegisterUser newRegisterUser)
    {
        if (!ModelState.IsValid)
        { return BadRequest(modelState: ModelState); }

        (SSOUser user, string invitationToken) = await ssoUserAggregationService.InviteRegisterUserAsync(
            registerForm: newRegisterUser);

        return Ok(value: new
        {
            User = user,
            Token = invitationToken
        });
    }

    [HttpPost("ResendInvite")]
    public async ValueTask<IActionResult> PostResendInvite([FromQuery] string userId)
    {
        string invitationToken = await ssoUserAggregationService
            .RegenerateUserInviteToken(userId: userId);
        return Ok(value: new { Token = invitationToken });
    }

    [HttpPost("AcceptInvite")]
    public async ValueTask<IActionResult> PostAcceptInvite(
        [FromQuery] string userId,
        [FromQuery] string inviteToken,
        [FromBody] RegisterUser newRegisterUser)
    {
        if (!ModelState.IsValid)
        { return BadRequest(modelState: ModelState); }

        await ssoUserAggregationService.AcceptRegisterUserInviteAsync(
            registerForm: newRegisterUser,
            userId: userId,
            tokenId: inviteToken);

        return Ok();
    }
}