using cCoder.Security.Brokers.Requests;
using cCoder.Security.Objects;
using cCoder.Security.Services.Orchestrations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;
using System.Text;

namespace cCoder.Security.Services.Orchestrations;
internal class SSOAuthInfoOrchestrationService 
    : ISSOAuthInfoOrchestrationService
{
    private readonly ISessionProcessingService sessionService;
    private readonly ISSOUserProcessingService userService;
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

    public async ValueTask<ISSOAuthInfo> GetSSOAuthInfoAsync()
    {
        ISSOAuthInfo auth = (await GetFromAuthenticationHeaderAsync()) ?? GetFromSession();
        return auth ?? new SSOAuthInfo { SSOUserId = "Guest" };
    }

    private async ValueTask<ISSOAuthInfo> GetFromAuthenticationHeaderAsync()
    {
        string authHeaderValue = httpRequestBroker.Header("Authorization");

        if (string.IsNullOrEmpty(authHeaderValue))
            return null;

        if (authHeaderValue.StartsWith("bearer", StringComparison.InvariantCultureIgnoreCase))
            return GetBearerAuthentication(authHeaderValue);

        if(authHeaderValue.StartsWith("basic", StringComparison.InvariantCultureIgnoreCase))
            return await GetBasicAuthenticationAsync(authHeaderValue);

        return null;
    }

    private ISSOAuthInfo GetFromSession()
    {
        Objects.Entities.SSOUser user = sessionService.GetUser();

        if (user == null)
            return null;

        return new SSOAuthInfo { SSOUserId = user.Id };
    }

    ISSOAuthInfo GetBearerAuthentication(string authHeaderValue)
    {
        string tokenId = GetBearerToken(authHeaderValue);

        if (tokenId == null)
            return null;

        Objects.Entities.Token token = tokenService.GetAllTokens(ignoreFilters: true)
            .FirstOrDefault(t => t.Id == tokenId);

        if (token == null)
            return null;

        return new SSOAuthInfo { SSOUserId = token.UserName };
    }

    async ValueTask<ISSOAuthInfo> GetBasicAuthenticationAsync(string authHeaderValue)
    {
        if (authHeaderValue.ToLowerInvariant().StartsWith("basic"))
            return await AuthenticateBasicAuthAsync(authHeaderValue);

        return null;
    }

    async ValueTask<ISSOAuthInfo> AuthenticateBasicAuthAsync(string auth)
    {
        (string username, string password) = ParseBasicAuthDetails(auth);

        Objects.Entities.SSOUser user = await userService
            .FindByUserAndPasswordAsync(username, password);

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


