using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processing.Interfaces;

public interface ISSORoleProcessingService
{
    IQueryable<SSORole> GetAllSSORoles(bool ignoreFilters = false);

    ValueTask<SSORole> AddSSORoleAsync(SSORole item);

    ValueTask<SSORole> UpdateSSORoleAsync(SSORole item);

    ValueTask DeleteSSORoleAsync(SSORole item);
}
