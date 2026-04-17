using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Api.Controllers;

public class SSOUserRoleController(IServiceProvider serviceProvider)
        : SecurityController<SSOUserRole>
{
    private ISSOUserRoleOrchestrationService SsoUserRoleOrchestrationService =>
        serviceProvider.GetRequiredService<ISSOUserRoleOrchestrationService>();

    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSOUserRole> queryOptions) =>
        Ok(SsoUserRoleOrchestrationService.GetAllSSOUserRoles());

    [HttpPost]
    public async ValueTask<IActionResult> Post([FromBody] SSOUserRole userRole) =>
        ModelState.IsValid
            ? Ok(await SsoUserRoleOrchestrationService.AddSSOUserRoleAsync(userRole))
            : BadRequest(ModelState);

    [HttpDelete]
    public async ValueTask<IActionResult> Delete([FromQuery] string userId, [FromQuery] Guid roleId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userRole = SsoUserRoleOrchestrationService
            .GetAllSSOUserRoles()
            .FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId);

        if (userRole is null)
            return NotFound();

        await SsoUserRoleOrchestrationService.DeleteSSOUserRoleAsync(userRole);

        return Ok();
    }
}

