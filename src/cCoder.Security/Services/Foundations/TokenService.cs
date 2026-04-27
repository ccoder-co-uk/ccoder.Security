using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using Microsoft.Extensions.Configuration;

namespace cCoder.Security.Services.Foundations;
internal class TokenService : ITokenService
{
    private readonly ITokenBroker tokenBroker;
    private readonly int tokenTimeout = 45;

    public TokenService(ITokenBroker tokenBroker, IConfiguration cofiguration)
    {
        this.tokenBroker = tokenBroker;

        if (int.TryParse(cofiguration?.GetSection("Settings")["TokenTimeout"], out int timeout))
            tokenTimeout = timeout;
    }

    public async ValueTask<Token> AddTokenAsync(string userId, int reasonCode = 0, int? timeout = null)
    {
        string value = Guid.NewGuid().ToString().Replace("-", "") + Guid.NewGuid().ToString().Replace("-", "");

        if (value.StartsWith('a'))
            value = value[1..] + "a";

        Token token = new()
        {
            Id = value,
            Expires = DateTimeOffset.Now.AddMinutes(timeout ?? tokenTimeout),
            Reason = reasonCode,
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

    public IQueryable<Token> GetAllTokens(bool ignoreFilters = false) => 
        tokenBroker.GetAllTokens(ignoreFilters);
}


