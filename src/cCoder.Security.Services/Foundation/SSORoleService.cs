using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Foundation;

public class SSORoleService(
    ISSORoleBroker roleBroker) 
        : ISSORoleService
{
    public IQueryable<SSORole> GetAllSSORoles() =>
        roleBroker.GetAllSSORoles();

    public async ValueTask<SSORole> AddSSORoleAsync(SSORole item) =>
        await roleBroker.AddSSORoleAsync(item);

    public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole item) =>
        await roleBroker.UpdateSSORoleAsync(item);

    public async ValueTask DeleteSSORoleAsync(SSORole item) =>
        await roleBroker.DeleteSSORoleAsync(item);
}