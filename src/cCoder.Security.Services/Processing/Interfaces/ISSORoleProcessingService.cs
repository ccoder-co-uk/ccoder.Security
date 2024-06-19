using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processing.Interfaces;

public interface ISSORoleProcessingService
{
    IQueryable<SSORole> GetAllSSORoles();

    ValueTask<SSORole> AddSSORoleAsync(SSORole item);

    ValueTask<SSORole> UpdateSSORoleAsync(SSORole item);

    ValueTask DeleteSSORoleAsync(SSORole item);
}