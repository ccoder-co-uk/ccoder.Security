// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.DTOs;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class AuthenticationController(
    IAuthenticationManager authenticationAggregationService)
        : Controller
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
        catch (SecurityAggregationValidationException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status401Unauthorized,
                value: "The supplied credentials are invalid.");
        }
        catch (SecurityAggregationDependencyException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                value: "The security service is unavailable.");
        }
        catch (SecurityAggregationServiceException exception)
            when (ContainsSecurityException(exception: exception))
        {
            return StatusCode(
                statusCode: StatusCodes.Status401Unauthorized,
                value: "The supplied credentials are invalid.");
        }
        catch (Exception)
        {
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
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The logout request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                value: "The security service is unavailable.");
        }
        catch (Exception)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The security operation failed.");
        }
    }

    private static bool ContainsSecurityException(Exception exception) =>
        exception is SecurityException
        || exception.InnerException is not null
            && ContainsSecurityException(exception: exception.InnerException);
}