// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Exposures.Controllers;

public class TenantController(ITenantAdministrationManager tenantAggregationService)
    : Controller
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<Tenant> queryOptions)
    {
        try
        {
            return Ok(value: tenantAggregationService.GetAllTenants());
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The tenant request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
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

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] string key)
    {
        try
        {
            IQueryable<Tenant> result = tenantAggregationService
                .GetAllTenants()
                .Where(predicate: tenant => tenant.Id == key);

            return result.Any()
                ? Ok(value: SingleResult.Create(queryable: result))
                : NotFound();
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The tenant request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
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
    public async ValueTask<IActionResult> Post([FromBody] Tenant newTenant)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            return StatusCode(
                statusCode: StatusCodes.Status201Created,
                value: await tenantAggregationService.AddTenantAsync(item: newTenant));
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The tenant request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
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

    [HttpPut]
    public async ValueTask<IActionResult> Put(
        [FromRoute] string key,
        [FromBody] Tenant updatedTenant)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            updatedTenant.Id = key;

            return Ok(value: await tenantAggregationService.UpdateTenantAsync(
                item: updatedTenant));
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The tenant request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
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
    public async ValueTask<IActionResult> Delete([FromRoute] string key)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            Tenant tenant = tenantAggregationService
                .GetAllTenants()
                .FirstOrDefault(predicate: tenant => tenant.Id == key);

            if (tenant is null)
            {
                return NotFound();
            }

            await tenantAggregationService.DeleteTenantAsync(item: tenant);

            return NoContent();
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The tenant request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
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