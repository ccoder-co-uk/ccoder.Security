using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;

namespace cCoder.Security.Data.EF.SQLite;

// This Wrapper in case we want to take a different approach to the EF factory for our context instancing 
// or just want a single place to call the ctor to avoid affecting a lot of files when the ctor changes.
public class SQLiteSecurityDbContextFactory
    : ISecurityDbContextFactory
{
    public Func<ISSOAuthInfo> GetAuthInfo { get; set; } =
        () => new SSOAuthInfo { SSOUserId = "Guest" };

    public SecurityDbContext CreateDbContext() =>
        new(GetAuthInfo(), new SecuritySQLiteModelBuildProvider("SSO"));

    public SecurityDbContext CreateDbContext(string[] args) =>
        CreateDbContext();
}