using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Objects;

namespace cCoder.Security.Data.EF.SQLite;

// This Wrapper in case we want to take a different approach to the EF factory for our context instancing
// or just want a single place to call the ctor to avoid affecting a lot of files when the ctor changes.
public class SQLiteSecurityDbContextFactory(string connectionString) : ISecurityDbContextFactory
{
    public Func<bool, ISSOAuthInfo> GetAuthInfo { get; set; } =
        _ => new SSOAuthInfo { SSOUserId = "Guest" };

    public SecurityDbContext CreateDbContext(bool ignoreAuthInfo = false) =>
        new(GetAuthInfo(ignoreAuthInfo), new SecuritySQLiteModelBuildProvider(connectionString));

    public SecurityDbContext CreateDbContext(string[] args) =>
        CreateDbContext();
}
