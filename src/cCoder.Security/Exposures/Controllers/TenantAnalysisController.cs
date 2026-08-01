// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Processings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Exposures.Controllers;

public class TenantAnalysisController(ITenantAnalysisManager tenantAnalysisProcessingService)
    : Controller
{
    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get()
    {
        try
        {
            return Ok(value: tenantAnalysisProcessingService.GetAllTenantAnalysis());
        }
        catch (SecurityProcessingValidationException)
        {
            return BadRequest(error: "The tenant analysis request is invalid.");
        }
        catch (SecurityProcessingDependencyException)
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
    public virtual IActionResult Get([FromRoute] Guid key)
    {
        try
        {
            IQueryable<TenantAnalysis> result = tenantAnalysisProcessingService
                .GetAllTenantAnalysis()
                .Where(predicate: i => i.Id == key);

            return result.Any()
                ? Ok(value: SingleResult.Create(queryable: result))
                : NotFound();
        }
        catch (SecurityProcessingValidationException)
        {
            return BadRequest(error: "The tenant analysis request is invalid.");
        }
        catch (SecurityProcessingDependencyException)
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