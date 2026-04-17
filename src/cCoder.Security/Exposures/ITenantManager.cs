using cCoder.Security.Data.Models;

namespace cCoder.Security.Exposures;

public interface ITenantManager
{
    ValueTask SetupAsync(SetupDetails setupDetails);
}
