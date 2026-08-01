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

public class SSOUserController(ISSOUserManager ssoUserAggregationService)
        : Controller
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSOUser> queryOptions)
    {
        try
        {
            return Ok(value: ssoUserAggregationService.GetAllSSOUsers());
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The user request is invalid.");
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
            IQueryable<SSOUser> result = ssoUserAggregationService
                .GetAllSSOUsers()
                .Where(predicate: user => user.Id == key);

            return result.Any()
                ? Ok(value: SingleResult.Create(queryable: result))
                : NotFound();
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The user request is invalid.");
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
    [EnableQuery]
    public virtual async ValueTask<IActionResult> Put(
        [FromRoute] string key,
        [FromBody] SSOUser updatedSSOUser)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            return Ok(value: await ssoUserAggregationService.UpdateSSOUserAsync(
                username: key,
                updatedSSOUser: updatedSSOUser));
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The user request is invalid.");
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
    public virtual async ValueTask<IActionResult> Delete([FromRoute] string key, string reference = null)
    {
        try
        {
            SSOUser originalUser = ssoUserAggregationService
                .GetAllSSOUsers()
                .FirstOrDefault(predicate: user => user.Id == key);

            if (originalUser is null)
            {
                return NotFound();
            }

            await ssoUserAggregationService.DeleteSSOUserAsync(
                deletedSSOUser: originalUser);

            return NoContent();
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The user request is invalid.");
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