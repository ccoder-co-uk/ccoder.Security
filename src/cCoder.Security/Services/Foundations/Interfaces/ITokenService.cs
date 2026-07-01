using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;
internal interface ITokenService
{
    ValueTask<Token> AddTokenAsync(string userId, TokenUse tokenUse, int? timeout = null);

    ValueTask DeleteTokenAsync(Token item);

    ValueTask<int> DeleteExpiredAsync(CancellationToken cancellationToken = default);

    IQueryable<Token> GetAllTokens(bool ignoreFilters = false);
}

