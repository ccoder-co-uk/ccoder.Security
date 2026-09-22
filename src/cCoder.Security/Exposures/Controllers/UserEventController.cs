// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models.Entities;
using cCoder.Security.Models.Exceptions;
using cCoder.Security.Services.Processings.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace cCoder.Security.Exposures.Controllers;

public class UserEventController(IUserEventProcessingService userEventProcessingService)
    : Controller
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<UserEvent> queryOptions)
    {
        try
        {
            return Ok(value: userEventProcessingService.GetAllUserEvents());
        }
        catch (SecurityProcessingValidationException)
        {
            return BadRequest(error: "The user event request is invalid.");
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
            IQueryable<UserEvent> result = userEventProcessingService
                .GetAllUserEvents()
                .Where(predicate: i => i.Id == key);

            UserEvent userEvent = result.FirstOrDefault();

            return userEvent is null
                ? NotFound()
                : Ok(value: userEvent);
        }
        catch (SecurityProcessingValidationException)
        {
            return BadRequest(error: "The user event request is invalid.");
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