using cCoder.Security.Data.Brokers.Storage.Interfaces;
using cCoder.Security.Objects.Entities;
using cCoder.Security.Services.Foundation.Interfaces;

namespace cCoder.Security.Services.Foundation;

public class SSOUserService(ISSOUserBroker ssoUserBroker) 
    : ISSOUserService
{
    public async ValueTask<SSOUser> AddSSOUserAsync(SSOUser newUser)
    {
        if (!ssoUserBroker.GetAllSSOUsers(ignoreFilters: true).Any())
            newUser.Roles =
                [
                    new SSOUserRole
                    {
                        Role = new SSORole
                        {
                            Name = "Portal Admins",
                            Description = "Default First User Admin Role",
                            UsersArePortalAdmins = true
                        }
                    }
                ];

        return await ssoUserBroker.AddSSOUserAsync(newUser);
    }

    public async ValueTask DeleteSSOUserAsync(SSOUser item) => 
        await ssoUserBroker.DeleteSSOUserAsync(item);

    public async ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser item) => 
        await ssoUserBroker.UpdateSSOUserAsync(item);

    public IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false) => 
        ssoUserBroker.GetAllSSOUsers(ignoreFilters);

    public SSOUser Me() => 
        ssoUserBroker.Me();
}