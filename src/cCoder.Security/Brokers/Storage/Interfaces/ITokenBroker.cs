using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Brokers.Storage.Interfaces;
internal interface ITokenBroker
{
    ValueTask<Token> AddTokenAsync(Token token);
    ValueTask DeleteTokenAsync(Token token);
    IQueryable<Token> GetAllTokens(bool ignoreFilters = false);
    ValueTask<Token> UpdateTokenAsync(Token token);
}

