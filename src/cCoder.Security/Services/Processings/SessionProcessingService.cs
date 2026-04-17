using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;
using cCoder.Security.Services.Processings.Interfaces;

namespace cCoder.Security.Services.Processings;
internal class SessionProcessingService(ISessionService sessionService)
        : ISessionProcessingService
{
    public string GetString(string key) => 
        sessionService.GetString(key);

    public SSOUser GetUser() =>
        sessionService.GetUser();

    public void SetString(string key, string value) =>
        sessionService.SetString(key, value);

    public void SetUser(SSOUser user)
    {
        if (sessionService.GetString("ssoUser") != null)
            sessionService.RemoveKey("ssoUser");

        sessionService.SetUser(user);
    }

    public void Remove(string key) => 
        sessionService.RemoveKey(key);

    public void Clear() =>
        sessionService.Clear();
}

