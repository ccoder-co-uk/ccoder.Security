using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Orchestration.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Api.Controllers;

public class TenantSecretController(
    ITenantSecretOrchestrationService tenantSecretOrchestrationService)
    : SecurityController<TenantSecret>
{
    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public IActionResult Get(ODataQueryOptions<TenantSecret> queryOptions) =>
        Ok(tenantSecretOrchestrationService.GetAllTenantSecrets());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public IActionResult Get([FromRoute] Guid key)
    {
        IQueryable<TenantSecret> result = tenantSecretOrchestrationService
            .GetAllTenantSecrets()
            .Where(secret => secret.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }

    [HttpPost]
    public async ValueTask<IActionResult> Post([FromBody] TenantSecret tenantSecret) =>
        ModelState.IsValid
            ? Ok(await tenantSecretOrchestrationService.AddTenantSecretAsync(tenantSecret))
            : BadRequest(ModelState);

    [HttpPut]
    public async ValueTask<IActionResult> Put([FromRoute] Guid key, [FromBody] TenantSecret tenantSecret)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        TenantSecret updatedTenantSecret = await tenantSecretOrchestrationService
            .UpdateTenantSecretAsync(key, tenantSecret);

        return updatedTenantSecret is null
            ? NotFound()
            : Ok(updatedTenantSecret);
    }

    [HttpDelete]
    public async ValueTask<IActionResult> Delete([FromRoute] Guid key)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        IQueryable<TenantSecret> existingTenantSecret = tenantSecretOrchestrationService
            .GetAllTenantSecrets()
            .Where(secret => secret.Id == key);

        if (!existingTenantSecret.Any())
            return NotFound();

        await tenantSecretOrchestrationService.DeleteTenantSecretAsync(key);

        return Ok();
    }
}
