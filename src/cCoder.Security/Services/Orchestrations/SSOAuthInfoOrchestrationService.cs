// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Security.Brokers.Requests;
using cCoder.Security.Objects;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using System.Text;

namespace cCoder.Security.Services.Orchestrations;

internal class SSOAuthInfoOrchestrationService(
    ISessionProcessingService sessionService,
    ISSOUserProcessingService userService,
    ITokenProcessingService tokenService,
    IHttpRequestBroker httpRequestBroker)
    : ISSOAuthInfoOrchestrationService
{
    public async ValueTask<ISSOAuthInfo> GetSSOAuthInfoAsync()
    {
        ISSOAuthInfo auth = (await GetFromAuthenticationHeaderAsync()) ?? GetFromSession();
        return auth ?? new SSOAuthInfo { SSOUserId = "Guest" };
    }

    private async ValueTask<ISSOAuthInfo> GetFromAuthenticationHeaderAsync()
    {
        string authHeaderValue = httpRequestBroker.Header(key: "Authorization");

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
        Objects.Entities.SSOUser user = sessionService.GetUser();

        if (user == null)
        { return null; }

        return new SSOAuthInfo { SSOUserId = user.Id };
    }

    ISSOAuthInfo GetBearerAuthentication(string authHeaderValue)
    {
        string tokenId = GetBearerToken(auth: authHeaderValue);

        if (tokenId == null)
        { return null; }

        Objects.Entities.Token token = tokenService.GetAllTokens(ignoreFilters: true)
            .FirstOrDefault(predicate: t => t.Id == tokenId);

        if (token == null)
        { return null; }

        return new SSOAuthInfo { SSOUserId = token.UserName };
    }

    async ValueTask<ISSOAuthInfo> GetBasicAuthenticationAsync(string authHeaderValue)
    {
        if (authHeaderValue.ToLowerInvariant().StartsWith(value: "basic"))
        { return await AuthenticateBasicAuthAsync(auth: authHeaderValue); }

        return null;
    }

    async ValueTask<ISSOAuthInfo> AuthenticateBasicAuthAsync(string auth)
    {
        (string username, string password) = ParseBasicAuthDetails(auth: auth);

        Objects.Entities.SSOUser user = await userService
            .FindByUserAndPasswordAsync(username: username, password: password);

        return new SSOAuthInfo { SSOUserId = user.Id };
    }

    static (string, string) ParseBasicAuthDetails(string auth)
    {
        string base64AuthString = auth[6..];
        byte[] authBytes = Convert.FromBase64String(s: base64AuthString);
        string authString = Encoding.UTF8.GetString(bytes: authBytes);

        return (
            (authString.Contains(value: '&')
                ? authString.Split(separator: "&")[0]
                : authString.Split(separator: ":")[0]).Replace(oldValue: "username=", newValue: ""),
            (authString.Contains(value: '&')
                ? authString.Split(separator: "&")[1]
                : authString.Split(separator: ":")[1]).Replace(oldValue: "password=", newValue: "")
        );
    }

    static string GetBearerToken(string auth)
    {
        if (!auth.ToLowerInvariant().StartsWith(value: "bearer"))
        { return null; }

        return auth.Split(separator: " ").LastOrDefault();
    }
}