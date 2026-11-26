// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Linq;

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

using cCoder.CodeAnalysis.Exposures;
using cCoder.Security.Brokers.Logging;
using cCoder.Security.Exposures;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace cCoder.Security.Exposures.Controllers;

public class TenantController(
    ITenantOrchestrationService tenantOrchestrationService,
    ILoggingBroker loggingBroker)
    : Controller, ICompositionExposure
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<Tenant> queryOptions)
    {
        try
        {
            IActionResult successResponse = Ok(value: tenantOrchestrationService.GetAllTenants());

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
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The tenant request is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
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
            IQueryable<Tenant> result = tenantOrchestrationService
                .GetAllTenants()
                .Where(predicate: tenant => tenant.Id == key);

            Tenant tenant = result.FirstOrDefault();

            if (tenant is null)
            {
                loggingBroker.LogWarning(message: "Controller resource was not found.");

                return NotFound();
            }

            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return Ok(value: tenant);
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
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The tenant request is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
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
    public async ValueTask<IActionResult> Post([FromBody] Tenant newTenant)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "Controller request model was invalid.");

                return BadRequest(modelState: ModelState);
            }

            IActionResult successResponse = StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await tenantOrchestrationService.AddTenantAsync(item: newTenant));

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
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The tenant request is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
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
    public async ValueTask<IActionResult> Put(
        [FromRoute] string key,
        [FromBody] Tenant updatedTenant)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "Controller request model was invalid.");

                return BadRequest(modelState: ModelState);
            }

            updatedTenant.Id = key;

            IActionResult successResponse = Ok(value: await tenantOrchestrationService.UpdateTenantAsync(
                item: updatedTenant));

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
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The tenant request is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
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
    public async ValueTask<IActionResult> Delete([FromRoute] string key)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                loggingBroker.LogWarning(message: "Controller request model was invalid.");

                return BadRequest(modelState: ModelState);
            }

            Tenant tenant = tenantOrchestrationService
                .GetAllTenants()
                .FirstOrDefault(predicate: tenant => tenant.Id == key);

            if (tenant is null)
            {
                loggingBroker.LogWarning(message: "Controller resource was not found.");

                return NotFound();
            }

            await tenantOrchestrationService.DeleteTenantAsync(item: tenant);

            IActionResult successResponse = NoContent();

            loggingBroker.LogInformation(message: "Controller request succeeded.");

            return successResponse;
        }
        catch (TenantDeletionRejectedException exception)
        {
            string reason = exception.Reason switch
            {
                TenantDeletionRejection.LinkedApplications => "Cannot delete a tenant with Apps",
                TenantDeletionRejection.LinkedBucketTree => "Cannot delete a tenant with a bucket tree.",
                _ => "The tenant still owns dependent resources."
            };

            loggingBroker.LogWarning(message: reason);

            return BadRequest(error: reason);
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
        catch (SecurityAggregationValidationException exception)
        {
            loggingBroker.LogWarning(message: exception.Message);

            return BadRequest(error: "The tenant request is invalid.");
        }
        catch (SecurityAggregationDependencyException exception)
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