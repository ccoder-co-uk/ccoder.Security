// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Models;
using cCoder.Security.Models.Configurations;
using cCoder.Security.Models.Entities;
using cCoder.Security.Services.Aggregations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using System.Text;

namespace cCoder.Security.Services.Aggregations;

internal sealed partial class SSOAuthInfoAggregationService(
    ISessionProcessingService sessionService,
    ISSOUserProcessingService userService,
    ITokenProcessingService tokenService,
    IRequestProcessingService requestProcessingService)
        : ISSOAuthInfoAggregationService
{
    public ValueTask<ISSOAuthInfo> GetSSOAuthInfoAsync() =>
        TryCatch<ISSOAuthInfo>(operation: async () =>
        {
            ValidateSSOAuthInfoOnGet();

            string authHeaderValue = requestProcessingService.GetHeader(
                key: "Authorization");

            ISSOAuthInfo authInfo = await GetFromAuthenticationHeaderAsync(
                authHeaderValue: authHeaderValue);

            if (!string.IsNullOrEmpty(value: authHeaderValue)
                && authInfo is null)
            {
                return new SSOAuthInfo
                {
                    AuthenticationFailed = true
                };
            }

            authInfo ??= GetFromSession();

            return authInfo ?? new SSOAuthInfo { SSOUserId = "Guest" };
        });

    private async ValueTask<ISSOAuthInfo> GetFromAuthenticationHeaderAsync(
        string authHeaderValue)
    {
        if (string.IsNullOrEmpty(value: authHeaderValue))
        { return null; }

        if (authHeaderValue.StartsWith(value: "bearer", comparisonType: StringComparison.InvariantCultureIgnoreCase))
        { return GetBearerAuthentication(authHeaderValue: authHeaderValue); }

        if (authHeaderValue.StartsWith(value: "basic", comparisonType: StringComparison.InvariantCultureIgnoreCase))
        { return await GetBasicAuthenticationAsync(authHeaderValue: authHeaderValue); }

        return null;
    }

    private ISSOAuthInfo GetFromSession()
    {
        Models.Entities.SSOUser user = sessionService.GetUser();
        string tokenId = sessionService.GetString(key: "token");

        if (user == null || string.IsNullOrEmpty(value: tokenId))
        { return null; }

        Token token = tokenService.GetTokenById(tokenId: tokenId);

        if (token is null
            || token.Reason != (int)TokenUse.Auth
            || token.UserName != user.Id)
        { return null; }

        return new SSOAuthInfo { SSOUserId = user.Id };
    }

    ISSOAuthInfo GetBearerAuthentication(string authHeaderValue)
    {
        string tokenId = GetBearerToken(auth: authHeaderValue);

        if (tokenId == null)
        { return null; }

        Models.Entities.Token token = tokenService.GetTokenById(tokenId: tokenId);

        if (token == null || token.Reason != (int)TokenUse.Auth)
        { return null; }

        return new SSOAuthInfo { SSOUserId = token.UserName };
    }

    async ValueTask<ISSOAuthInfo> GetBasicAuthenticationAsync(string authHeaderValue)
    {
        if (authHeaderValue
            .ToLowerInvariant()
            .StartsWith(value: "basic"))
        { return await AuthenticateBasicAuthAsync(auth: authHeaderValue); }

        return null;
    }

    async ValueTask<ISSOAuthInfo> AuthenticateBasicAuthAsync(string auth)
    {
        (string username, string password) = ParseBasicAuthDetails(auth: auth);

        Models.Entities.SSOUser user = await userService
            .FindByUserAndPasswordAsync(username: username, password: password);

        return new SSOAuthInfo { SSOUserId = user.Id };
    }

    static (string, string) ParseBasicAuthDetails(string auth)
    {
        string base64AuthString = auth[6..];
        byte[] authBytes = Convert.FromBase64String(s: base64AuthString);
        string authString = Encoding.UTF8.GetString(bytes: authBytes);

        string separator = authString.Contains(value: '&')
            ? "&"
            : ":";

        string[] authParts = authString.Split(separator: separator);

        string username = authParts[0]
            .Replace(oldValue: "username=", newValue: "");

        string password = authParts[1]
            .Replace(oldValue: "password=", newValue: "");

        return (username, password);
    }

    static string GetBearerToken(string auth)
    {
        if (!auth
            .ToLowerInvariant()
            .StartsWith(value: "bearer"))
        { return null; }

        return auth
            .Split(separator: " ")
            .LastOrDefault();
    }
}