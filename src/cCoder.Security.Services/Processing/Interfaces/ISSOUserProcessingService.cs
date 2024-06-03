using cCoder.Security.Objects.Entities;

namespace cCoder.Security.Services.Processing.Interfaces;

public interface ISSOUserProcessingService
{
    ValueTask<SSOUser> RegisterSSOUserAsync(SSOUser item);

    ValueTask<SSOUser> InviteSSOUserAsync(SSOUser user);

    ValueTask<SSOUser> UpdateSSOUserAsync(SSOUser item);

    ValueTask DeleteSSOUserAsync(SSOUser item);

    IQueryable<SSOUser> GetAllSSOUsers(bool ignoreFilters = false);

    SSOUser FindByUserAndPassword(string username, string password);

    SSOUser FindById(string id);

    SSOUser Me();

    void ValidatePassword(string password);
}