using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Processings.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Api.Controllers;

public class SSOPrivilegeController(IServiceProvider serviceProvider)
    : SecurityController<SSOPrivilege>
{
    private ISSOPrivilegeProcessingService SsoPrivilegeProcessingService =>
        serviceProvider.GetRequiredService<ISSOPrivilegeProcessingService>();

    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<SSOPrivilege> queryOptions) =>
        Ok(SsoPrivilegeProcessingService.GetAllSSOPrivileges());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] string key)
    {
        IQueryable<SSOPrivilege> result = SsoPrivilegeProcessingService
            .GetAllSSOPrivileges()
            .Where(i => i.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }
}

