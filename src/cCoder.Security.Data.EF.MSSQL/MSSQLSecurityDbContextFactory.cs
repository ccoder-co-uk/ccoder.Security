using cCoder.Security.Data.EF.Interfaces;
using cCoder.Security.Data.EF.MSSQL;
using cCoder.Security.Objects;

namespace cCoder.Security.Data.EF;

// This Wrapper in case we want to take a different approach to the EF factory for our context instancing 
// or just want a single place to call the ctor to avoid affecting a lot of files when the ctor changes.
public class MSSQLSecurityDbContextFactory
    : ISecurityDbContextFactory
{
    public Func<ISSOAuthInfo> GetAuthInfo { get; set; } =
        () => new SSOAuthInfo { SSOUserId = "Guest" };

    public SecurityDbContext CreateDbContext()
    {
        Console.WriteLine("Using MSSQLSecurityDbContextFactory to create DbContext");
        return new(GetAuthInfo(), new SecurityMSSQLModelBuildProvider("SSO"));
    }

    public SecurityDbContext CreateDbContext(string[] args) =>
        CreateDbContext();
}