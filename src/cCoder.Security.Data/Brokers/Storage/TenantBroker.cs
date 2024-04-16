using System.Linq;
using System.Threading.Tasks;
using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage
{
    public class TenantBroker : ITenantBroker
    {
        ISecurityDbContextFactory contextFactory;

        public TenantBroker(ISecurityDbContextFactory contextFactory)
            => this.contextFactory = contextFactory;

        public async ValueTask<Tenant> AddTenantAsync(Tenant tenant)
        {
            using var context = contextFactory.CreateDbContext();

            var entityEntry = await context.Tenants.AddAsync(tenant);
            await context.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async ValueTask<Tenant> UpdateTenantAsync(Tenant tenant)
        {
            using var context = contextFactory.CreateDbContext();

            var entityEntry = context.Tenants.Update(tenant);
            await context.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async ValueTask DeleteTenantAsync(Tenant tenant)
        {
            using var context = contextFactory.CreateDbContext();

            var entityEntry = context.Tenants.Remove(tenant);
            await context.SaveChangesAsync();
        }

        public IQueryable<Tenant> GetAllTenants()
        {
            var context = contextFactory.CreateDbContext();
            return context.Tenants;
        }
    }
}