using cCoder.Security.Data.Models;

namespace cCoder.Security.Services.Processings.Events;
internal interface ITenantSetupEventProcessingService
{
    ValueTask SetupAsync(SetupDetails setupDetails);
}


