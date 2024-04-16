using cCoder.Security.Data.Brokers.Requests;
using cCoder.Security.Objects;
using cCoder.Security.Services.Orchestration.Interfaces;
using cCoder.Security.Services.Processing.Interfaces;
using System.Diagnostics;
using System.Text;

namespace cCoder.Security.Services.Orchestration;

public class SSOAuthInfoOrchestrationService 
    : ISSOAuthInfoOrchestrationService
{
    readonly ISessionProcessingService sessionService;
    readonly ISSOUserProcessingService userService;
    private readonly ITokenProcessingService tokenService;
    private readonly IHttpRequestBroker httpRequestBroker;

    public SSOAuthInfoOrchestrationService(
        ISessionProcessingService sessionService,
        ISSOUserProcessingService userService,
        ITokenProcessingService tokenService,
        IHttpRequestBroker httpRequestBroker)
    {
        this.sessionService = sessionService;
        this.userService = userService;
        this.tokenService = tokenService;
        this.httpRequestBroker = httpRequestBroker;
    }

    public ISSOAuthInfo GetSSOAuthInfo()
    {
        var auth = GetFromAuthenticationHeader() ?? GetFromSession();
        return auth ?? new SSOAuthInfo { SSOUserId = "Guest" };
    }

    private ISSOAuthInfo GetFromAuthenticationHeader()
    {
        string authHeaderValue = httpRequestBroker.Header("Authorization");

        if (string.IsNullOrEmpty(authHeaderValue))
            return null;

        if (authHeaderValue.StartsWith("bearer", StringComparison.InvariantCultureIgnoreCase))
            return GetBearerAuthentication(authHeaderValue);

        if(authHeaderValue.StartsWith("basic", StringComparison.InvariantCultureIgnoreCase))
            return GetBasicAuthentication(authHeaderValue);

        return null;
    }

    ISSOAuthInfo GetFromSession()
    {
        var user = sessionService.GetUser();

        if (user == null)
            return null;

        return new SSOAuthInfo { SSOUserId = user.Id };
    }

    ISSOAuthInfo GetBearerAuthentication(string authHeaderValue)
    {
        var tokenId = GetBearerToken(authHeaderValue);

        if (tokenId == null)
            return null;

        var token = tokenService.GetAllTokens(ignoreFilters: true)
            .FirstOrDefault(t => t.Id == tokenId);

        if (token == null)
            return null;

        return new SSOAuthInfo { SSOUserId = token.UserName };
    }

    ISSOAuthInfo GetBasicAuthentication(string authHeaderValue)
    {
        if (authHeaderValue.ToLowerInvariant().StartsWith("basic"))
            return AuthenticateBasicAuth(authHeaderValue);

        return null;
    }

    ISSOAuthInfo AuthenticateBasicAuth(string auth)
    {
        (string username, string password) = ParseBasicAuthDetails(auth);
        var user = userService.FindByUserAndPassword(username, password);
        return new SSOAuthInfo { SSOUserId = user.Id };
    }

    static (string, string) ParseBasicAuthDetails(string auth)
    {
        string base64AuthString = auth[6..];
        byte[] authBytes = Convert.FromBase64String(base64AuthString);
        string authString = Encoding.UTF8.GetString(authBytes);
        return (
            (authString.Contains('&')
                ? authString.Split("&")[0]
                : authString.Split(":")[0]).Replace("username=", ""),
            (authString.Contains('&')
                ? authString.Split("&")[1]
                : authString.Split(":")[1]).Replace("password=", "")
        );
    }

    static string GetBearerToken(string auth)
    {
        if (!auth.ToLowerInvariant().StartsWith("bearer"))
            return null;

        return auth.Split(" ").LastOrDefault();
    }
}