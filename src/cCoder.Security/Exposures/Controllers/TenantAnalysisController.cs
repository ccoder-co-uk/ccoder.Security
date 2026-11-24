// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;

using Microsoft.AspNetCore.Http;
using System;

using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Processings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

using cCoder.Security.Brokers.Logging;

namespace cCoder.Security.Exposures.Controllers;

public class TenantAnalysisController(ITenantAnalysisProcessingService tenantAnalysisProcessingService,
    ILoggingBroker loggingBroker)
    : Controller
{
    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get()
    {
        try
        {
            IActionResult successResponse = Ok(value: tenantAnalysisProcessingService.GetAllTenantAnalyses());

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

            return BadRequest(error: "The tenant analysis request is invalid.");
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
    public virtual IActionResult Get([FromRoute] Guid key)
    {
        try
        {
            IQueryable<TenantAnalysis> result = tenantAnalysisProcessingService
                .GetAllTenantAnalyses()
                .Where(predicate: i => i.Id == key);

            TenantAnalysis tenantAnalysis = result.FirstOrDefault();

            if (tenantAnalysis is null)
            {
                loggingBroker.LogWarning(message: "Controller resource was not found.");

                return NotFound();
            }

            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return Ok(value: tenantAnalysis);
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

            return BadRequest(error: "The tenant analysis request is invalid.");
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