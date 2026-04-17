using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Foundations.Interfaces;
internal interface ISessionService
{
    void SetString(string key, string value);
    string GetString(string key);
    SSOUser GetUser();
    void SetUser(SSOUser user);
    void RemoveKey(string key);
    void Clear();
}

