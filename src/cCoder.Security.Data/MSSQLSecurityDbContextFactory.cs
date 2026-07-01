using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;

namespace cCoder.Security.Data.EF;

// This Wrapper in case we want to take a different approach to the EF factory for our context instancing 
// or just want a single place to call the ctor to avoid affecting a lot of files when the ctor changes.
public class MSSQLSecurityDbContextFactory()
    : ISecurityDbContextFactory
{
    private readonly string connectionString;

    public Func<bool, ISSOAuthInfo> GetAuthInfo { get; set; } =
        (x) => new SSOAuthInfo { SSOUserId = "Guest" };

    public MSSQLSecurityDbContextFactory(string connectionString) : this() =>
        this.connectionString = connectionString;

    public SecurityDbContext CreateDbContext(bool ignoreAuthInfo = false) =>
        new (GetAuthInfo(ignoreAuthInfo), new SecurityMSSQLModelBuildProvider(connectionString ?? "SSO"));

    public SecurityDbContext CreateDbContext(string[] args) =>
        CreateDbContext();
}
