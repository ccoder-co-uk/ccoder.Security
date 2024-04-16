using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Foundation;

public class SSOUserService(ISSOUserBroker ssoUserBroker) 
    : ISSOUserService
{
    public async ValueTask<SSOUser> AddSSOUserAsync(SSOUser item) => 
        await ssoUserBroker.AddSSOUserAsync(item);

    public async ValueTask DeleteSSOUserAsync(SSOUser item) => 
        await ssoUserBroker.DeleteSSOUserAsync(item);

    public async ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser item) => 
        await ssoUserBroker.UpdateSSOUserAsync(item);

    public IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false) => 
        ssoUserBroker.GetAllSSOUsers(ignoreFilters);

    public SSOUser Me() => 
        ssoUserBroker.Me();
}