// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.Security.Brokers.Logging;
using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class AuthenticationController(
    IAuthenticationAggregationService authenticationAggregationService,
    ILoggingBroker loggingBroker)
        : Controller, ICompositionExposure
{
    [HttpPost("Login")]
    public async ValueTask<IActionResult> PostLogin([FromBody] Auth newAuth)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            return Ok(value: await authenticationAggregationService.LoginAsync(
                username: newAuth.User,
                password: newAuth.Pass));
        }
        catch (SecurityAggregationAuthenticationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return Challenge();
        }
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The login request is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                value: "The security service is unavailable.");
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

    [HttpPost("Logout")]
    public async ValueTask<IActionResult> PostLogout()
    {
        try
        {
            await authenticationAggregationService.LogoutAsync();

            return Ok();
        }
        catch (SecurityAggregationAuthenticationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return Challenge();
        }
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The logout request is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                value: "The security service is unavailable.");
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

    [HttpPost("ChangePassword")]
    public async ValueTask<IActionResult> PostChangePassword(
        [FromBody] ChangePasswordRequest newChangePasswordRequest)
    {
        try
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            if (!string.Equals(
                a: newChangePasswordRequest.NewPassword,
                b: newChangePasswordRequest.ConfirmPassword,
                comparisonType: StringComparison.Ordinal))
            {
                return BadRequest(error: "The password confirmation does not match.");
            }

            await authenticationAggregationService.ChangeCurrentUserPasswordAsync(
                oldPassword: newChangePasswordRequest.OldPassword,
                newPassword: newChangePasswordRequest.NewPassword);

            return Ok();
        }
        catch (SecurityAggregationAuthenticationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return Challenge();
        }
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The password change is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
        {
            loggingBroker.LogError(
                exception: exception,
                message: "Controller request failed.");

            return StatusCode(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                value: "The security service is unavailable.");
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