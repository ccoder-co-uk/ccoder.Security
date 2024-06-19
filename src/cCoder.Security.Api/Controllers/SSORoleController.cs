using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Api.Controllers;

public class SSORoleController(
    ISSORoleOrchestrationService ssoRoleOrchestrationService) 
        : SecurityController<SSORole>
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSORole> queryOptions) =>
        Ok(ssoRoleOrchestrationService.GetAllSSORoles());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] Guid key)
    {
        IQueryable<SSORole> result = ssoRoleOrchestrationService
            .GetAllSSORoles()
            .Where(i => i.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }

    [HttpPost]
    [EnableQuery]
    public virtual async ValueTask<IActionResult> Post([FromRoute] Guid key, [FromBody] SSORole ssoRole) =>
        ModelState.IsValid
            ? Get((await ssoRoleOrchestrationService.UpdateSSORoleAsync(ssoRole)).Id)
            : BadRequest(ModelState);

    [HttpPut]
    [EnableQuery]
    public virtual async ValueTask<IActionResult> Put([FromRoute] Guid key, [FromBody] SSORole ssoRole) =>
        ModelState.IsValid
            ? Get((await ssoRoleOrchestrationService.UpdateSSORoleAsync(ssoRole)).Id)
            : BadRequest(ModelState);

    [HttpDelete]
    public virtual async ValueTask<IActionResult> Delete([FromRoute] Guid key)
    {
        SSORole origentity = ssoRoleOrchestrationService
            .GetAllSSORoles()
            .FirstOrDefault(i => i.Id == key);

        if (origentity == null)
            return NotFound();

        await ssoRoleOrchestrationService.DeleteSSORoleAsync(origentity);
        return Ok();
    }
}