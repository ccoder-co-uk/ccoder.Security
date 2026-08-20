// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class CurrentUserController(
    ISecurityCurrentUserManager currentUserAggregationService)
        : Controller
{
    [HttpGet("Me")]
    public IActionResult GetMe()
    {
        try
        {
            return Ok(value: currentUserAggregationService.GetCurrentUser());
        }
        catch (SecurityAggregationAuthenticationException)
        {
            return Challenge();
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

    [HttpPut("Me")]
    public async ValueTask<IActionResult> PutMe([FromBody] SSOUser updatedUser)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            return Ok(value: await currentUserAggregationService
                .UpdateCurrentSSOUserAsync(updatedUser: updatedUser));
        }
        catch (SecurityAggregationAuthenticationException)
        {
            return Challenge();
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The profile update is invalid.");
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
}