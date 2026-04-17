using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;
internal interface ITokenService
{
    ValueTask<Token> AddTokenAsync(string userId, int reasonCode = 0, int? timeout = null);
    ValueTask DeleteTokenAsync(Token item);
    IQueryable<Token> GetAllTokens(bool ignoreFilters = false);
}

