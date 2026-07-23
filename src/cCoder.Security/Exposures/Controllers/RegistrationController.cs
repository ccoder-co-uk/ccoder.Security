// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.DTOs;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class RegistrationController(
    IRegistrationAggregationService registrationAggregationService)
    : Controller
{
    [HttpPost("Register")]
    public async ValueTask<IActionResult> PostRegister([FromBody] RegisterUser newRegisterUser)
    {
        if (!ModelState.IsValid)
        { return BadRequest(modelState: ModelState); }

        RegisterUser registeredUser =
            await registrationAggregationService.RegisterUserAsync(
            registerForm: newRegisterUser);

        return Ok(value: new
        {
            registeredUser.User,
            registeredUser.Token
        });
    }

    [HttpPost("ConfirmRegistration")]
    public async ValueTask<IActionResult> PostConfirmRegistration(string confirmationToken)
    {
        await registrationAggregationService.ConfirmRegistration(
            tokenId: confirmationToken);

        return Ok();
    }

    [HttpPost("Invite")]
    public async ValueTask<IActionResult> PostInvite([FromBody] RegisterUser newRegisterUser)
    {
        if (!ModelState.IsValid)
        { return BadRequest(modelState: ModelState); }

        RegisterUser invitedUser =
            await registrationAggregationService.InviteRegisterUserAsync(
            registerForm: newRegisterUser);

        return Ok(value: new
        {
            invitedUser.User,
            invitedUser.Token
        });
    }

    [HttpPost("ResendInvite")]
    public async ValueTask<IActionResult> PostResendInvite([FromQuery] string userId)
    {
        string invitationToken = await registrationAggregationService
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

        await registrationAggregationService.AcceptRegisterUserInviteAsync(
            registerForm: newRegisterUser,
            userId: userId,
            tokenId: inviteToken);

        return Ok();
    }
}