using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Security.Objects.Entities;
using Security.Services.Processing.Interfaces;

namespace Security.Api.Controllers;

public class TenantController : ODataController
{
    private readonly ITenantProcessingService tenantProcessingService;

    public TenantController(ITenantProcessingService tenantProcessingService) =>
        this.tenantProcessingService = tenantProcessingService;

    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<Tenant> queryOptions) =>
        Ok(tenantProcessingService.GetAllTenants());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] string key)
    {
        IQueryable<Tenant> result = tenantProcessingService
            .GetAllTenants()
            .Where(i => i.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }

    [HttpPut]
    [EnableQuery]
    public virtual async ValueTask<IActionResult> Put([FromRoute] string key, [FromBody] Tenant ssoUser)
        => ModelState.IsValid
            ? Get((await tenantProcessingService.UpdateTenantAsync(ssoUser)).Id)
            : BadRequest(ModelState);

    [AcceptVerbs("PATCH", "MERGE")]
    public async ValueTask<IActionResult> Patch([FromRoute] string key, Delta<Tenant> delta)
    {
        // don't like this, would rather pass the delta to the service layer but that would force a 
        // dependency on the OData framework in the business layer.
        Tenant origentity = tenantProcessingService.GetAllTenants().FirstOrDefault(i => i.Id == key);

        if (origentity == null)
            return NotFound();

        delta.Patch(origentity);
        return Ok(await tenantProcessingService.UpdateTenantAsync(origentity));
    }

    [HttpPost]
    public virtual async ValueTask<IActionResult> Post([FromBody] Tenant ssoUser) =>
        ModelState.IsValid
            ? Ok(await tenantProcessingService.AddTenantAsync(ssoUser))
            : BadRequest(ModelState);

    [HttpDelete]
    public virtual async ValueTask<IActionResult> Delete([FromRoute] string key, string reference = null)
    {
        Tenant origentity = tenantProcessingService
            .GetAllTenants()
            .FirstOrDefault(i => i.Id == key);

        if (origentity == null)
            return NotFound();

        await tenantProcessingService.DeleteTenantAsync(origentity);
        return Ok();
    }
}