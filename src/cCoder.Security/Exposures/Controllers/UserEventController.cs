// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Processings.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Exposures.Controllers;

public class UserEventController(IUserEventProcessingService userEventProcessingService)
    : Controller
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<UserEvent> queryOptions) =>
        Ok(value: userEventProcessingService.GetAllUserEvents());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] Guid key)
    {
        IQueryable<UserEvent> result = userEventProcessingService
            .GetAllUserEvents()
            .Where(predicate: i => i.Id == key);

        return result.Any()
            ? Ok(value: SingleResult.Create(queryable: result))
            : NotFound();
    }
}