// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Services.Aggregations.Interfaces;
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
}