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
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(modelState: ModelState);
            }

            await tenantManager.SetupAsync(setupDetails: newSetupDetails);

            return Ok();
        }
        catch (SecurityAggregationValidationException)
        {
            return BadRequest(error: "The setup request is invalid.");
        }
        catch (SecurityAggregationDependencyException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status503ServiceUnavailable,
                value: "The security service is unavailable.");
        }
        catch (SecurityAggregationServiceException)
        {
            return StatusCode(
                statusCode: StatusCodes.Status500InternalServerError,
                value: "The security operation failed.");
        }
    }
}