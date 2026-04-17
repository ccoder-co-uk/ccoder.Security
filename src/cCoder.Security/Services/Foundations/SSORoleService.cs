using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;
internal class SSORoleService(
    ISSORoleBroker roleBroker) 
        : ISSORoleService
{
    public IQueryable<SSORole> GetAllSSORoles(bool ignoreFilters = false) =>
        roleBroker.GetAllSSORoles(ignoreFilters);

    public async ValueTask<SSORole> AddSSORoleAsync(SSORole item) =>
        await roleBroker.AddSSORoleAsync(item);

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole item) =>
        await roleBroker.UpdateSSORoleAsync(item);

    public async ValueTask DeleteSSORoleAsync(SSORole item) =>
        await roleBroker.DeleteSSORoleAsync(item);
}



