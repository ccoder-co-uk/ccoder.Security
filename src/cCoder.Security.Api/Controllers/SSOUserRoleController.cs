using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace cCoder.Security.Api.Controllers;

public class SSOUserRoleController(
    ISSOUserRoleOrchestrationService ssoUserRoleOrchestrationService)
        : SecurityController<SSOUserRole>
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSOUserRole> queryOptions) =>
        Ok(ssoUserRoleOrchestrationService.GetAllSSOUserRoles());

    [HttpPost]
    public async ValueTask<IActionResult> Post([FromBody] SSOUserRole userRole) =>
    ModelState.IsValid
        ? Ok(await ssoUserRoleOrchestrationService.AddSSOUserRoleAsync(userRole))
        : BadRequest(ModelState);

    [HttpDelete]
    public async ValueTask<IActionResult> Delete([FromQuery] string userId, [FromQuery] Guid roleId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userRole = ssoUserRoleOrchestrationService
            .GetAllSSOUserRoles()
            .FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (userRole is null)
            return NotFound();

        await ssoUserRoleOrchestrationService.DeleteSSOUserRoleAsync(userRole);

        return Ok();
    }
}