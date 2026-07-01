using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Processings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Exposures.Controllers;

public class TenantAnalysisController(IServiceProvider serviceProvider)
    : SecurityController<TenantAnalysis>
{
    private ITenantAnalysisProcessingService TenantAnalysisProcessingService =>
        serviceProvider.GetRequiredService<ITenantAnalysisProcessingService>();

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get() =>
        Ok(TenantAnalysisProcessingService.GetAllTenantAnalysis());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] Guid key)
    {
        IQueryable<TenantAnalysis> result = TenantAnalysisProcessingService
            .GetAllTenantAnalysis()
            .Where(i => i.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }
}

