// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Mvc;
using Security.Web.Exposures;

namespace Security.Web.Controllers;

[Route("CurrentUser")]
public class CurrentUserController(
    ICurrentUserManager currentUserManager)
        : Controller
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(value: currentUserManager.GetCurrentUserId());
}