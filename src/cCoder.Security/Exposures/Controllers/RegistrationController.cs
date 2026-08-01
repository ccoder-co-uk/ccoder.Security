// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class RegistrationController(
    IRegistrationManager registrationAggregationService)
    : Controller
{
    [HttpPost("Register")]
    public async ValueTask<IActionResult> PostRegister([FromBody] RegisterUser newRegisterUser)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            RegisterUser registeredUser =
                await registrationAggregationService.RegisterUserAsync(
                    registerForm: newRegisterUser);

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: new
                {
                    registeredUser.User,
                    registeredUser.Token
                });
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The registration request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
        {
            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The security operation failed.");
        }
    }

    [HttpPost("ConfirmRegistration")]
    public async ValueTask<IActionResult> PostConfirmRegistration(string confirmationToken)
    {
        try
        {
            await registrationAggregationService.ConfirmRegistration(
                tokenId: confirmationToken);

            return Ok();
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The confirmation request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
        {
            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The security operation failed.");
        }
    }

    [HttpPost("Invite")]
    public async ValueTask<IActionResult> PostInvite([FromBody] RegisterUser newRegisterUser)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            RegisterUser invitedUser =
                await registrationAggregationService.InviteRegisterUserAsync(
                    registerForm: newRegisterUser);

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: new
                {
                    invitedUser.User,
                    invitedUser.Token
                });
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The invitation request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
        {
            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The security operation failed.");
        }
    }

    [HttpPost("ResendInvite")]
    public async ValueTask<IActionResult> PostResendInvite([FromQuery] string userId)
    {
        try
        {
            string invitationToken = await registrationAggregationService
                .RegenerateUserInviteToken(userId: userId);

            return Ok(value: new { Token = invitationToken });
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The invitation request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
        {
            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The security operation failed.");
        }
    }

    [HttpPost("AcceptInvite")]
    public async ValueTask<IActionResult> PostAcceptInvite(
        [FromQuery] string userId,
        [FromQuery] string inviteToken,
        [FromBody] RegisterUser newRegisterUser)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            await registrationAggregationService.AcceptRegisterUserInviteAsync(
                registerForm: newRegisterUser,
                userId: userId,
                tokenId: inviteToken);

            return Ok();
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The invitation request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
        {
            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The security operation failed.");
        }
    }
}