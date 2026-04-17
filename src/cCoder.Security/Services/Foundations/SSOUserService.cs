using cCoder.Security.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundations.Interfaces;

namespace cCoder.Security.Services.Foundations;
internal class SSOUserService(ISSOUserBroker ssoUserBroker) 
    : ISSOUserService
{
    public async ValueTask<SSOUser> AddSSOUserAsync(SSOUser newUser) =>
        await ssoUserBroker.AddSSOUserAsync(newUser);

    public async ValueTask DeleteSSOUserAsync(SSOUser item) => 
        await ssoUserBroker.DeleteSSOUserAsync(item);

    public async ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser item) => 
        await ssoUserBroker.UpdateSSOUserAsync(item);

    public IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false) => 
        ssoUserBroker.GetAllSSOUsers(ignoreFilters);

    public SSOUser Me() => 
        ssoUserBroker.Me();
}



