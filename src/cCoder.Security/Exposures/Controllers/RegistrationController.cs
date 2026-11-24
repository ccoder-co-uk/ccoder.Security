// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.Security.Brokers.Logging;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class RegistrationController(
    IRegistrationAggregationService registrationAggregationService,
    ILoggingBroker loggingBroker)
    : Controller, ICompositionExposure
{
    [HttpPost("Register")]
    public async ValueTask<IActionResult> PostRegister([FromBody] RegisterUser newRegisterUser)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "Controller request model was invalid.");

                return BadRequest(modelState: ModelState);
            }

            RegisterUser registeredUser =
                await registrationAggregationService.RegisterUserAsync(
                    registerForm: newRegisterUser);

            IActionResult successResponse = StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: new
                {
                    registeredUser.User,
                    registeredUser.Token
                });

            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return successResponse;
        }
        catch (ArgumentException)
        {
            loggingBroker.LogWarning(message: "Controller request was rejected.");

            return BadRequest(error: "The request is invalid.");
        }
        catch (System.Security.Authentication.AuthenticationException)
        {
            loggingBroker.LogWarning(message: "Controller authentication failed.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The registration request is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

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

            IActionResult successResponse = Ok();

            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return successResponse;
        }
        catch (ArgumentException)
        {
            loggingBroker.LogWarning(message: "Controller request was rejected.");

            return BadRequest(error: "The request is invalid.");
        }
        catch (System.Security.Authentication.AuthenticationException)
        {
            loggingBroker.LogWarning(message: "Controller authentication failed.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The confirmation request is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

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
                loggingBroker.LogWarning(message: "Controller request model was invalid.");

                return BadRequest(modelState: ModelState);
            }

            RegisterUser invitedUser =
                await registrationAggregationService.InviteRegisterUserAsync(
                    registerForm: newRegisterUser);

            IActionResult successResponse = StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: new
                {
                    invitedUser.User,
                    invitedUser.Token
                });

            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return successResponse;
        }
        catch (ArgumentException)
        {
            loggingBroker.LogWarning(message: "Controller request was rejected.");

            return BadRequest(error: "The request is invalid.");
        }
        catch (System.Security.Authentication.AuthenticationException)
        {
            loggingBroker.LogWarning(message: "Controller authentication failed.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The invitation request is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

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

            IActionResult successResponse = Ok(value: new { Token = invitationToken });

            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return successResponse;
        }
        catch (ArgumentException)
        {
            loggingBroker.LogWarning(message: "Controller request was rejected.");

            return BadRequest(error: "The request is invalid.");
        }
        catch (System.Security.Authentication.AuthenticationException)
        {
            loggingBroker.LogWarning(message: "Controller authentication failed.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The invitation request is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

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
                loggingBroker.LogWarning(message: "Controller request model was invalid.");

                return BadRequest(modelState: ModelState);
            }

            await registrationAggregationService.AcceptRegisterUserInviteAsync(
                registerForm: newRegisterUser,
                userId: userId,
                tokenId: inviteToken);

            IActionResult successResponse = Ok();

            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return successResponse;
        }
        catch (ArgumentException)
        {
            loggingBroker.LogWarning(message: "Controller request was rejected.");

            return BadRequest(error: "The request is invalid.");
        }
        catch (System.Security.Authentication.AuthenticationException)
        {
            loggingBroker.LogWarning(message: "Controller authentication failed.");

            return Unauthorized(value: "Authentication failed.");
        }
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The invitation request is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return Problem(statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (System.Security.SecurityException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return StatusCode(statusCode: StatusCodes.Status403Forbidden);
        }
        catch (Exception exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The security operation failed.");
        }
    }
}