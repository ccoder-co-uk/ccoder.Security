// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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

        string value = Guid
            .NewGuid()
            .ToString()
            .Replace(oldValue: "-", newValue: "")
            + Guid
                .NewGuid()
                .ToString()
                .Replace(oldValue: "-", newValue: "");

        if (value.StartsWith(value: 'a'))
        { value = value[1..] + "a"; }

        Token token = new()
        {
            Id = value,
            Expires = DateTimeOffset.Now.AddMinutes(minutes: timeout ?? tokenTimeout),
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

        Token result = await tokenBroker.InsertTokenAsync(token: storageToken);
        token.Id = result.Id;
        token.Expires = result.Expires;
        token.Reason = result.Reason;
        token.UserName = result.UserName;
        return token;
    }

    public ValueTask DeleteTokenAsync(Token deletedToken) =>
        tokenBroker.DeleteTokenAsync(token: deletedToken);

    public ValueTask<int> DeleteExpiredAsync(CancellationToken cancellationToken = default) =>
        tokenBroker.DeleteExpiredAsync(expiresBefore: DateTimeOffset.UtcNow, cancellationToken: cancellationToken);

    public IQueryable<Token> GetAllTokens(bool ignoreFilters = false) =>
        ignoreFilters
            ? tokenBroker.SelectAllTokensIgnoringFilters()
            : tokenBroker.SelectAllTokens();

    private int GetTokenTimeout()
    {
        if (int.TryParse(s: configuration?.GetSection(key: "Settings")["TokenTimeout"], result: out int timeout))
        { return timeout; }

        return 45;
    }
}