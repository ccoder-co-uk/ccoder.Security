using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Processing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;

namespace cCoder.Security.Api.Controllers;

public class TenantAnalysisController(
    ITenantAnalysisProcessingService tenantAnalysisProcessingService) 
        : Controller
{ 
    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 0, MaxAnyAllExpressionDepth = 0)]
    public virtual IActionResult Get() =>
        Ok(tenantAnalysisProcessingService.GetAllTenantAnalysis());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 0, MaxAnyAllExpressionDepth = 0)]
    public virtual IActionResult Get([FromRoute] Guid key)
    {
        IQueryable<TenantAnalysis> result = tenantAnalysisProcessingService
            .GetAllTenantAnalysis()
            .Where(i => i.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }
}