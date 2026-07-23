// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Exposures.Controllers;

public class TenantController(ITenantAggregationService tenantAggregationService)
    : Controller
{
    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<Tenant> queryOptions) =>
        Ok(value: tenantAggregationService.GetAllTenants());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] string key)
    {
        IQueryable<Tenant> result = tenantAggregationService
            .GetAllTenants()
            .Where(predicate: i => i.Id == key);

        return result.Any()
            ? Ok(value: SingleResult.Create(queryable: result))
            : NotFound();
    }

    [HttpPost]
    public async ValueTask<IActionResult> Post([FromBody] Tenant newTenant) =>
        ModelState.IsValid
            ? Ok(value: await tenantAggregationService.AddTenantAsync(item: newTenant))
            : BadRequest(modelState: ModelState);

    [HttpPut]
    public async ValueTask<IActionResult> Put(
        [FromRoute] string key,
        [FromBody] Tenant updatedTenant) =>
        ModelState.IsValid
            ? Ok(value: await tenantAggregationService.UpdateTenantAsync(item: updatedTenant))
            : BadRequest(modelState: ModelState);

    [HttpDelete]
    public async ValueTask<IActionResult> Delete([FromRoute] string key)
    {
        if (!ModelState.IsValid)
        { return BadRequest(modelState: ModelState); }

        var tenant = tenantAggregationService
            .GetAllTenants()
            .First(predicate: t => t.Id == key);

        if (tenant is null)
        { return NotFound(); }

        await tenantAggregationService.DeleteTenantAsync(item: tenant);

        return Ok();
    }
}