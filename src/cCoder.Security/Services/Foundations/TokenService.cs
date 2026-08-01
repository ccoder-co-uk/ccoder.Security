// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Configuration;
using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Brokers.Encryption.Interfaces;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;

internal sealed partial class TokenService(
    ITokenBroker tokenBroker,
    ISecurityConfigurationBroker configurationBroker,
    ITokenGenerationBroker tokenGenerationBroker,
    IPasswordHashingBroker passwordHashingBroker)
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

            string selector = tokenGenerationBroker.GenerateSelector();
            string secret = tokenGenerationBroker.GenerateSecret();

            string value = tokenGenerationBroker.Combine(
                selector: selector,
                secret: secret);

            Token token = new()
            {
                Id = value,
                Expires = DateTimeOffset.Now.AddMinutes(minutes: timeout ?? tokenTimeout),
                Reason = (int)tokenUse,
                UserName = userId
            };

            Token storageToken = new()
            {
                Id = selector,
                Expires = token.Expires,
                Reason = token.Reason,
                UserName = token.UserName,
                SecretHash = passwordHashingBroker.HashTokenSecret(
                    secret: secret)
            };

            Token result = await tokenBroker.InsertTokenAsync(token: storageToken);
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