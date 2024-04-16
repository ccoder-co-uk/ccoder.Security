using cCoder.Security.Objects.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace cCoder.Security.Services.Foundation.Interfaces
{
    public interface ISSORoleService
    {
        IQueryable<SSORole> GetAllSSORoles();

        ValueTask<SSORole> AddSSORoleAsync(SSORole item);
        ValueTask<SSORole> UpdateSSORoleAsync(SSORole item);
        ValueTask DeleteSSORoleAsync(SSORole item);
    }
}