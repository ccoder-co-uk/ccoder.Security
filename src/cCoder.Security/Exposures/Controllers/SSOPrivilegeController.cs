// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;

using Microsoft.AspNetCore.Http;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Foundations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using cCoder.Security.Brokers.Logging;

namespace cCoder.Security.Exposures.Controllers;

public class SSOPrivilegeController(ISSOPrivilegeService privilegeProcessingService,
    ILoggingBroker loggingBroker)
    : Controller
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSOPrivilege> queryOptions)
    {
        try
        {
            return Ok(value: privilegeProcessingService.GetAllSSOPrivileges());
        }
        catch (SecurityProcessingValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The privilege request is invalid.");
        }
        catch (SecurityProcessingDependencyException exception)
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
    public virtual IActionResult Get([FromRoute] string key)
    {
        try
        {
            IQueryable<SSOPrivilege> result = privilegeProcessingService
                .GetAllSSOPrivileges()
                .Where(predicate: i => i.Id == key);

            SSOPrivilege ssoPrivilege = result.FirstOrDefault();

            return ssoPrivilege is null
                ? NotFound()
                : Ok(value: ssoPrivilege);
        }
        catch (SecurityProcessingValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The privilege request is invalid.");
        }
        catch (SecurityProcessingDependencyException exception)
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