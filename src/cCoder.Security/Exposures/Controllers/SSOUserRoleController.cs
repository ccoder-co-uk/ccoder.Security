// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using cCoder.Security.Brokers.Logging;

namespace cCoder.Security.Exposures.Controllers;

public class SSOUserRoleController(ISSOUserRoleOrchestrationService userRoleOrchestrationService,
    ILoggingBroker loggingBroker)
        : Controller
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSOUserRole> queryOptions)
    {
        try
        {
            return Ok(value: userRoleOrchestrationService.GetAllSSOUserRoles());
        }
        catch (SecurityOrchestrationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The user role request is invalid.");
        }
        catch (SecurityOrchestrationDependencyException exception)
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

    [HttpPost]
    public async ValueTask<IActionResult> Post([FromBody] SSOUserRole newSSOUserRole)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await userRoleOrchestrationService.AddSSOUserRoleAsync(
                    userRole: newSSOUserRole));
        }
        catch (SecurityOrchestrationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The user role request is invalid.");
        }
        catch (SecurityOrchestrationDependencyException exception)
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

    [HttpDelete]
    public async ValueTask<IActionResult> Delete([FromQuery] string userId, [FromQuery] Guid roleId)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            SSOUserRole userRole = userRoleOrchestrationService
                .GetAllSSOUserRoles()
                .FirstOrDefault(predicate: userRole =>
                    userRole.UserId == userId && userRole.RoleId == roleId);

            if (userRole is null)
            {
                return NotFound();
            }

            await userRoleOrchestrationService.DeleteSSOUserRoleAsync(
                userRole: userRole);

            return NoContent();
        }
        catch (SecurityOrchestrationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The user role request is invalid.");
        }
        catch (SecurityOrchestrationDependencyException exception)
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