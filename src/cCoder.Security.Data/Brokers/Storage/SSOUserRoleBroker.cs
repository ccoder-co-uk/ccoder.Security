using System.Linq;
using System.Threading.Tasks;
using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage
{
    public class SSOUserRoleBroker : ISSOUserRoleBroker
    {
        ISecurityDbContextFactory contextFactory;

        public SSOUserRoleBroker(ISecurityDbContextFactory contextFactory)
            => this.contextFactory = contextFactory;

        public async ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole userRole)
        {
            using var context = contextFactory.CreateDbContext();

            var entityEntry = await context.UserRoles.AddAsync(userRole);
            await context.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async ValueTask DeleteSSOUserRoleAsync(SSOUserRole userRole)
        {
            using var context = contextFactory.CreateDbContext();

            var entityEntry = context.UserRoles.Remove(userRole);
            await context.SaveChangesAsync();
        }

        public IQueryable<SSOUserRole> GetAllSSOUserRoles()
        {
            var context = contextFactory.CreateDbContext();
            return context.UserRoles;
        }
    }
}