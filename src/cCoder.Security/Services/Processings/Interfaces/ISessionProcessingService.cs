using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processings.Interfaces;
internal interface ISessionProcessingService
{
    void SetString(string key, string value);
    string GetString(string key);
    SSOUser GetUser();
    void SetUser(SSOUser user);
    void Clear();
    void Remove(string key);
}

