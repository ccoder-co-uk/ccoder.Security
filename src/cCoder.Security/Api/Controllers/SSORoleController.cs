using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Api.Controllers;

public class SSORoleController(IServiceProvider serviceProvider)
    : SecurityController<SSORole>
{
    private ISSORoleOrchestrationService SsoRoleService =>
        serviceProvider.GetRequiredService<ISSORoleOrchestrationService>();

    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSORole> queryOptions) =>
        Ok(SsoRoleService.GetAllSSORoles());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] Guid key)
    {
        IQueryable<SSORole> result = SsoRoleService
            .GetAllSSORoles()
            .Where(i => i.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }

    public virtual async ValueTask<IActionResult> Post([FromBody] SSORole role) =>
        Ok(await SsoRoleService.AddSSORoleAsync(role));

    public virtual async ValueTask<IActionResult> Put([FromRoute] Guid key, [FromBody] SSORole role) =>
        Ok(await SsoRoleService.UpdateSSORoleAsync(role));

    [HttpDelete]
    public async ValueTask<IActionResult> Delete([FromRoute] Guid key)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var role = SsoRoleService
            .GetAllSSORoles()
            .FirstOrDefault(r => r.Id == key);

        if (role is null)
            return NotFound();

        await SsoRoleService.DeleteSSORoleAsync(role);

        return Ok();
    }
}

