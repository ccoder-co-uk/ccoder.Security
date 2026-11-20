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
            IActionResult successResponse = Ok(value: privilegeProcessingService.GetAllSSOPrivileges());

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

            if (ssoPrivilege is null)
            {
                loggingBroker.LogWarning(message: "Controller resource was not found.");

                return NotFound();
            }

            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return Ok(value: ssoPrivilege);
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