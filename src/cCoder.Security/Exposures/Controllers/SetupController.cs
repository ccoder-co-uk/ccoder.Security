// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Data.Models;
using cCoder.Security.Exposures;
using cCoder.Security.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Setup")]
public sealed class SetupController(ITenantManager tenantManager)
    : Controller
{
    [HttpPost]
    public async ValueTask<IActionResult> PostSetup(
        [FromBody] SetupDetails newSetupDetails)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(modelState: ModelState);
        }

        try
        {
            await tenantManager.SetupAsync(setupDetails: newSetupDetails);
        }
        catch (SecurityAggregationValidationException exception)
        {
            return Conflict(error: exception.InnerException?.Message);
        }
        catch (SecurityAggregationDependencyException)
        {
            return Problem(
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }
        catch (SecurityAggregationServiceException)
        {
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }
}