// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Configuration;
using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class TokenService(
    ITokenBroker tokenBroker,
    ISecurityConfigurationBroker configurationBroker)
    : ITokenService
{
    public ValueTask<Token> AddTokenAsync(
        string userId,
        TokenUse tokenUse,
        int? timeout = null) =>
        TryCatch<Token>(operation: async () =>
        {
            ValidateTokenOnAdd(
                userId: userId,
                tokenUse: tokenUse,
                timeout: timeout);

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
            {
                value = value[1..] + "a";
            }

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
        });

    public ValueTask DeleteTokenAsync(Token deletedToken) =>
        TryCatch(operation: async () =>
        {
            ValidateTokenOnDelete(deletedToken: deletedToken);

            await tokenBroker.DeleteTokenAsync(token: deletedToken);
        });

    public ValueTask<int> DeleteExpiredAsync(CancellationToken cancellationToken = default) =>
        TryCatch<int>(operation: async () =>
        {
            ValidateExpiredOnDelete(cancellationToken: cancellationToken);

            return await tokenBroker.DeleteExpiredAsync(
                expiresBefore: DateTimeOffset.UtcNow,
                cancellationToken: cancellationToken);
        });

    public IQueryable<Token> GetAllTokens(bool ignoreFilters = false) =>
        TryCatch(operation: () =>
        {
            ValidateAllTokensOnGet(ignoreFilters: ignoreFilters);

            return ignoreFilters
                ? tokenBroker.SelectAllTokensIgnoringFilters()
                : tokenBroker.SelectAllTokens();
        });

    private int GetTokenTimeout()
    {
        string configuredTimeout = configurationBroker.GetValue(
            section: "Settings",
            key: "TokenTimeout");

        if (int.TryParse(s: configuredTimeout, result: out int timeout))
        {
            return timeout;
        }

        return 45;
    }
}