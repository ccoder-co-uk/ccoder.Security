using System.Linq;
using System.Threading.Tasks;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage.Interfaces
{
    public interface ISSOUserRoleBroker
    {
        ValueTask<SSOUserRole> AddSSOUserRoleAsync(SSOUserRole userRole);
        ValueTask DeleteSSOUserRoleAsync(SSOUserRole userRole);
        IQueryable<SSOUserRole> GetAllSSOUserRoles();
    }
}