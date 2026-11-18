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

public class SSORoleController(ISSORoleOrchestrationService roleOrchestrationService,
    ILoggingBroker loggingBroker)
    : Controller
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSORole> queryOptions)
    {
        try
        {
            return Ok(value: roleOrchestrationService.GetAllSSORoles());
        }
        catch (SecurityOrchestrationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The role request is invalid.");
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

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] Guid key)
    {
        try
        {
            IQueryable<SSORole> result = roleOrchestrationService
                .GetAllSSORoles()
                .Where(predicate: i => i.Id == key);

            SSORole ssoRole = result.FirstOrDefault();

            return ssoRole is null
                ? NotFound()
                : Ok(value: ssoRole);
        }
        catch (SecurityOrchestrationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The role request is invalid.");
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
    public virtual async ValueTask<IActionResult> Post([FromBody] SSORole newSSORole)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await roleOrchestrationService.AddSSORoleAsync(item: newSSORole));
        }
        catch (SecurityOrchestrationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The role request is invalid.");
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

    [HttpPut]
    public virtual async ValueTask<IActionResult> Put(
        [FromRoute] Guid key,
        [FromBody] SSORole updatedSSORole)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            updatedSSORole.Id = key;

            return Ok(value: await roleOrchestrationService.UpdateSSORoleAsync(
                item: updatedSSORole));
        }
        catch (SecurityOrchestrationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The role request is invalid.");
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
    public async ValueTask<IActionResult> Delete([FromRoute] Guid key)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            SSORole role = roleOrchestrationService
                .GetAllSSORoles()
                .FirstOrDefault(predicate: role => role.Id == key);

            if (role is null)
            {
                return NotFound();
            }

            await roleOrchestrationService.DeleteSSORoleAsync(item: role);

            return NoContent();
        }
        catch (SecurityOrchestrationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The role request is invalid.");
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