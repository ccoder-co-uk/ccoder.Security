using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using Microsoft.Extensions.Configuration;

namespace cCoder.Security.Services.Foundations;

internal class TokenService(ITokenBroker tokenBroker, IConfiguration configuration)
    : ITokenService
{
    public async ValueTask<Token> AddTokenAsync(string userId, TokenUse tokenUse, int? timeout = null)
    {
        int tokenTimeout = GetTokenTimeout();
        string value = Guid.NewGuid().ToString().Replace("-", "") + Guid.NewGuid().ToString().Replace("-", "");

        if (value.StartsWith('a'))
            value = value[1..] + "a";

        Token token = new()
        {
            Id = value,
            Expires = DateTimeOffset.Now.AddMinutes(timeout ?? tokenTimeout),
            Reason = (int)tokenUse,
            UserName = userId
        };

        Token storageToken = new()
        {
            Id = token.Id,
            Expires = token.Expires,
            Reason = token.Reason,
            UserName = token.UserName
        };

        Token result = await tokenBroker.AddTokenAsync(storageToken);
        token.Id = result.Id;
        token.Expires = result.Expires;
        token.Reason = result.Reason;
        token.UserName = result.UserName;
        return token;
    }

    public async ValueTask DeleteTokenAsync(Token item) => 
        await tokenBroker.DeleteTokenAsync(item);

    public async ValueTask<int> DeleteExpiredAsync(CancellationToken cancellationToken = default) =>
        await tokenBroker.DeleteExpiredAsync(DateTimeOffset.UtcNow, cancellationToken);

    public IQueryable<Token> GetAllTokens(bool ignoreFilters = false) => 
        tokenBroker.GetAllTokens(ignoreFilters);

    private int GetTokenTimeout()
    {
        if (int.TryParse(configuration?.GetSection("Settings")["TokenTimeout"], out int timeout))
            return timeout;

        return 45;
    }
}
