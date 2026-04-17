using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestrations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Api.Controllers;

public class TenantController(IServiceProvider serviceProvider)
    : SecurityController<Tenant>
{
    private ITenantOrchestrationService TenantOrchestrationService =>
        serviceProvider.GetRequiredService<ITenantOrchestrationService>();

    private ITenantCoordinationService TenantCoordinationService =>
        serviceProvider.GetRequiredService<ITenantCoordinationService>();

    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<Tenant> queryOptions) =>
        Ok(TenantOrchestrationService.GetAllTenants());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] string key)
    {
        IQueryable<Tenant> result = TenantOrchestrationService
            .GetAllTenants()
            .Where(i => i.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }

    [HttpPost]
    public async ValueTask<IActionResult> Post([FromBody] Tenant tenant) =>
        ModelState.IsValid
            ? Ok(await TenantOrchestrationService.AddTenantAsync(tenant))
            : BadRequest(ModelState);

    [HttpPut]
    public async ValueTask<IActionResult> Put([FromRoute] Guid key, [FromBody] Tenant tenant) =>
        ModelState.IsValid
            ? Ok(await TenantOrchestrationService.UpdateTenantAsync(tenant))
            : BadRequest(ModelState);

    [HttpDelete]
    public async ValueTask<IActionResult> Delete([FromRoute] string key)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        var tenant = TenantOrchestrationService
            .GetAllTenants()
            .First(t => t.Id == key);

        if (tenant is null)
            return NotFound();

        await TenantCoordinationService.DeleteTenantAsync(tenant);

        return Ok();
    }
}

