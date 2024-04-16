using System.Linq;
using System.Threading.Tasks;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage.Interfaces
{
    public interface ITenantBroker
    {
        ValueTask<Tenant> AddTenantAsync(Tenant tenant);
        ValueTask DeleteTenantAsync(Tenant tenant);
        IQueryable<Tenant> GetAllTenants();
        ValueTask<Tenant> UpdateTenantAsync(Tenant tenant);
    }
}