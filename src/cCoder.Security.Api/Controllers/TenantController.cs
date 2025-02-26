using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Api.Controllers;

public class TenantController(
    ITenantOrchestrationService tenantOrchestrationService,
    ITenantCoordinationService tenantCoordinationService) 
    : SecurityController<Tenant>
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<Tenant> queryOptions) =>
        Ok(tenantOrchestrationService.GetAllTenants());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] string key)
    {
        IQueryable<Tenant> result = tenantOrchestrationService
            .GetAllTenants()
            .Where(i => i.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }

    [HttpPost]
    public async ValueTask<IActionResult> Post([FromBody] Tenant tenant) =>
        ModelState.IsValid
            ? Ok(await tenantOrchestrationService.AddTenantAsync(tenant))
            : BadRequest(ModelState);

    [HttpPut]
    public async ValueTask<IActionResult> Put([FromRoute] Guid key, [FromBody] Tenant tenant) =>
        ModelState.IsValid
            ? Ok(await tenantOrchestrationService.UpdateTenantAsync(tenant))
            : BadRequest(ModelState);

    [HttpDelete]
    public async ValueTask<IActionResult> Delete([FromRoute] string key)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var tenant = tenantOrchestrationService
            .GetAllTenants()
            .First(t => t.Id == key);

        if (tenant is null)
            return NotFound();

        await tenantCoordinationService.DeleteTenantAsync(tenant);

        return Ok();
    }
}
