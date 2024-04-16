using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;

namespace cCoder.Security.Data.EF;

// This Wrapper in case we want to take a different approach to the EF factory for our context instancing 
// or just want a single place to call the ctor to avoid affecting a lot of files when the ctor changes.
public class SecurityDbContextFactory(ISecurityModelBuildProvider modelBuildProvider) 
    : ISecurityDbContextFactory
{
    public Func<ISSOAuthInfo> GetAuthInfo { get; set; }

    public SecurityDbContext CreateDbContext(bool withAuthInfo = true)
    {
        var authInfo = withAuthInfo
            ? GetAuthInfo()
            : new SSOAuthInfo { SSOUserId = "Guest" };

        return new(authInfo, modelBuildProvider);
    }
}