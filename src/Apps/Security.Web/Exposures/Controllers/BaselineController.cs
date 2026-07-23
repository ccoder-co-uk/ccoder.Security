// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
namespace Security.Web.Exposures.Controllers;

[ApiController]
[Route("Api/Security/Baseline")]
public sealed class BaselineController(
    IUIBaselineManager uiBaselineManager)
        : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(value: uiBaselineManager.GetPackages());
}