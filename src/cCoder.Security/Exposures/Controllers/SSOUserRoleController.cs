// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace cCoder.Security.Exposures.Controllers;

public class SSOUserRoleController(ISSOUserRoleOrchestrationService userRoleOrchestrationService)
        : Controller
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSOUserRole> queryOptions) =>
        Ok(value: userRoleOrchestrationService.GetAllSSOUserRoles());

    [HttpPost]
    public async ValueTask<IActionResult> Post([FromBody] SSOUserRole newSSOUserRole) =>
        ModelState.IsValid
            ? Ok(value: await userRoleOrchestrationService.AddSSOUserRoleAsync(
                userRole: newSSOUserRole))
            : BadRequest(modelState: ModelState);

    [HttpDelete]
    public async ValueTask<IActionResult> Delete([FromQuery] string userId, [FromQuery] Guid roleId)
    {
        if (!ModelState.IsValid)
        { return BadRequest(modelState: ModelState); }

        var userRole = userRoleOrchestrationService
            .GetAllSSOUserRoles()
            .FirstOrDefault(predicate: ur => ur.UserId == userId && ur.RoleId == roleId);

        if (userRole is null)
        { return NotFound(); }

        await userRoleOrchestrationService.DeleteSSOUserRoleAsync(userRole: userRole);

        return Ok();
    }
}