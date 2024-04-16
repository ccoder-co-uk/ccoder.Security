using System.Linq;
using System.Threading.Tasks;
using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Data.Brokers.Storage.Interfaces
{
    public interface ITokenBroker
    {
        ValueTask<Token> AddTokenAsync(Token token);
        ValueTask DeleteTokenAsync(Token token);
        IQueryable<Token> GetAllTokens(bool ignoreFilters = false);
        ValueTask<Token> UpdateTokenAsync(Token token);
    }
}