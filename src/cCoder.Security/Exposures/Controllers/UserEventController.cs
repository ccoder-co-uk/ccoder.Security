using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Processings.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Security.Exposures.Controllers;

public class UserEventController(IServiceProvider serviceProvider)
    : SecurityController<UserEvent>
{
    private IUserEventProcessingService UserEventProcessingService =>
        serviceProvider.GetRequiredService<IUserEventProcessingService>();

    [HttpGet()]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get(ODataQueryOptions<UserEvent> queryOptions) =>
        Ok(UserEventProcessingService.GetAllUserEvents());

    [HttpGet]
    [EnableQuery(MaxExpansionDepth = 3, MaxAnyAllExpressionDepth = 3)]
    public virtual IActionResult Get([FromRoute] Guid key)
    {
        IQueryable<UserEvent> result = UserEventProcessingService
            .GetAllUserEvents()
            .Where(i => i.Id == key);

        return result.Any()
            ? Ok(SingleResult.Create(result))
            : NotFound();
    }
}

