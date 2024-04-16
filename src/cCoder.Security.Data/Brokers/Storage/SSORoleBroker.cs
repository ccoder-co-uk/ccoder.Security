using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace cCoder.Security.Data.Brokers.Storage
{
    public class SSORoleBroker : ISSORoleBroker
    {
        ISecurityDbContextFactory contextFactory;

        public SSORoleBroker(ISecurityDbContextFactory contextFactory)
            => this.contextFactory = contextFactory;

        public async ValueTask<SSORole> AddSSORoleAsync(SSORole role)
        {
            using var context = contextFactory.CreateDbContext();

            var entityEntry = await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async ValueTask<SSORole> UpdateSSORoleAsync(SSORole role)
        {
            using var context = contextFactory.CreateDbContext();

            var entityEntry = context.Roles.Update(role);
            await context.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async ValueTask DeleteSSORoleAsync(SSORole role)
        {
            using var context = contextFactory.CreateDbContext();

            var entityEntry = context.Roles.Remove(role);
            await context.SaveChangesAsync();
        }

        public IQueryable<SSORole> GetAllSSORoles()
        {
            var context = contextFactory.CreateDbContext();
            return context.Roles;
        }
    }
}