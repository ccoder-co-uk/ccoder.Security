// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace cCoder.Security.Exposures.Controllers;

public class SSOUserRoleController(ISSOUserRoleManager userRoleOrchestrationService)
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
        catch (SecurityOrchestrationValidationException)
        {
            return BadRequest(error: "The user role request is invalid.");
        }
        catch (SecurityOrchestrationDependencyException)
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
        catch (SecurityOrchestrationValidationException)
        {
            return BadRequest(error: "The user role request is invalid.");
        }
        catch (SecurityOrchestrationDependencyException)
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
        catch (SecurityOrchestrationValidationException)
        {
            return BadRequest(error: "The user role request is invalid.");
        }
        catch (SecurityOrchestrationDependencyException)
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