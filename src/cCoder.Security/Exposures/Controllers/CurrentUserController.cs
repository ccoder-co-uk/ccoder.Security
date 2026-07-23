// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace cCoder.Security.Exposures.Controllers;

[Route("Api/Account")]
public class CurrentUserController(
    ICurrentUserAggregationService currentUserAggregationService)
        : Controller
{
    [HttpGet("Me")]
    public IActionResult GetMe() =>
        Ok(value: currentUserAggregationService.GetCurrentUser());
}